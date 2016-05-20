﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
	/*
	 * NOTE: for some reason, this AI is VERY VERY dependent upon unit move speed
	 */ 
	public class AI : Player
	{
		public int TimeUntilNextMove = 60;
		int buildTickInterval;
		int mainTickInterval;
		
		public enum NodeOwner { me, enemy, noone, anyone, notMe}
		public enum RoadExistence { yes, no, any}
		
		public AI(string name, Color color, int buildTick, int mainTick, Rectangle gameWorld)
			: base(name, color, gameWorld)
		{
			this.buildTickInterval = buildTick;
			this.mainTickInterval = mainTick;
			base.is_AI = true;
			Random r;
			for (int i = 0; i < 3; i++) {
				r = new Random();
				int c = r.Next(1, 5);
				while (strategySpots.Contains(c))
					c = r.Next(1, 5);
				strategySpots.Add(c);
			}
		}
		List<Player> players;
		List<int> strategySpots = new List<int>();
		List<Node> allFacs; //all factories in the game
		bool strategy_setup = false;
		List<Road> roads;
		//my armies only

		
		
		#region center-, left-, right-, top-, bottom- most allFacs
		
		//---------------------------------------------these are not the absolute closest factory
		//to the certain location strategic point, they have been purposely "fuzzied up"
		public Node CenterMostFac
		{
			get
			{
				Point p = new Point(gameWorld.Width / 2, gameWorld.Height / 2);
				return UTILS.GetClosest(p, allFacs);
			}
		}
		public Node LeftMostFac {
			get
			{
				Point p = new Point(0, gameWorld.Height / 2);
				return UTILS.GetClosest(p, allFacs);
			}
		}
		public Node RightMostFac {
			get
			{
				Point p = new Point(gameWorld.Width, gameWorld.Height / 2);
				return UTILS.GetClosest(p, allFacs);
			}
		}
		public Node TopMostFac {
			get
			{
				Point p = new Point(gameWorld.Width / 2, 0);
				return UTILS.GetClosest(p, allFacs);
			}
		}
		public Node BottomMostFac {
			get
			{
				Point p = new Point(gameWorld.Width / 2, gameWorld.Height);
				return UTILS.GetClosest(p, allFacs);
			}
		}
		
		#endregion
		
		public void Do(List<Player> players, List<Node> allFacs, List<Road> roads)
		{
			
			//update game info
			this.players = players;
			this.roads = roads;
			this.allFacs = allFacs;
			
			//one move per second
			TimeUntilNextMove--;
			if (TimeUntilNextMove <= 0) {
				TimeUntilNextMove = 20;
				
				foreach (Node fac in allFacs) {
					
					if (!strategy_setup) {
						if (TopMostFac.owner == null && strategySpots.Contains(1))
							TryBuildNewRoad(ClosestNodeTo(TopMostFac, RoadExistence.no,
								NodeOwner.me), TopMostFac, roads);
						else if (BottomMostFac.owner == null && strategySpots.Contains(2)) {
							TryBuildNewRoad(ClosestNodeTo(BottomMostFac, RoadExistence.no,
								NodeOwner.me), BottomMostFac, roads);
						} else if (LeftMostFac.owner == null && strategySpots.Contains(3)) {
							TryBuildNewRoad(ClosestNodeTo(LeftMostFac, RoadExistence.no,
								NodeOwner.me), LeftMostFac, roads);
						} else if (RightMostFac.owner == null && strategySpots.Contains(4)) {
							TryBuildNewRoad(ClosestNodeTo(RightMostFac, RoadExistence.no,
								NodeOwner.me), RightMostFac, roads);
						} else if (CenterMostFac.owner == null && strategySpots.Contains(5)) {
							TryBuildNewRoad(ClosestNodeTo(CenterMostFac, RoadExistence.no,
								NodeOwner.me), CenterMostFac, roads);
						} else
							strategy_setup = true;
						
					}
					

					//my closest factory to "fac" that has no roads between them
					Node x = ClosestNodeTo(fac, RoadExistence.no, NodeOwner.me);
					
					//the closest enemy factory to "fac", roads or not
					Node y = ClosestNodeTo(fac, RoadExistence.any, NodeOwner.enemy);


					//expand && capture neutral allFacs
					if (fac.owner == null && strategy_setup
					    && x != null && y != null) {
						if (NetIncoming(x) <= 0) { //if i'm not under attack
							//make sure that i can capture and hold that neutral fac
							if (RoadBetween(y, fac) != null && x.armyStrength - roadCost > y.armyStrength
							    || RoadBetween(x, fac) == null && x.armyStrength + 6 > y.armyStrength)
								TryBuildNewRoad(x, fac, roads);
						}
						
						//allow AI to get the 1st move
						if (y.owner.numFactories == 1)
							TryBuildNewRoad(x, fac, roads);
					}
					
					
					//prioritize attack on enemy centers closest to it
					if (fac.owner == this) {
						List<Node> f_temporary = UTILS.GetClosestList(fac.loc, allFacs);
						foreach (Node f in f_temporary) {
							if (f.owner == this || f.owner == null) //makes sure target is an enemy factory
								continue;
							
							int n = ArmiesToSend(fac, f);
							if (n <= 0)
								continue;
							
							if (RoadBetween(fac, f) != null)
								SendArmy(fac, f, n);
							else if (TryBuildNewRoad(fac, f, roads)) {
								SendArmy(fac, f, n);
								
							}
						}
						
						//"big clumps" quickly build & upgrade quick roads to attack enemies
						if(fac.armyStrength >= 400)
						{
							int sum = 0;
							foreach (Node n in fac.nodesConnected) 
							{
								if (n.owner == fac.owner)
									sum += n.armyStrength;
							}
							if(fac.armyStrength > 5 * sum)
							{
								Node enemy = ClosestNodeTo(fac, RoadExistence.any, NodeOwner.enemy);
								if(enemy!=null 
								   && fac.armyStrength *.8 > enemy.armyStrength //if strong enough
								   && UTILS.DistSquared(fac.loc, enemy.loc) < 300 * 300) //if nearby
								{
									TryBuildNewRoad(fac, enemy, roads);
									SendArmy(fac, enemy, 4);
								}
							}
						}
						
						//TODO: make use of ArmiesToHelp() method!!!! for reinforcing
						
						
						Harvest(fac); //harvests from fac
					}
				}
				
				//UPGRADING
				Road r = ShouldUpgrade();
				if (r != null) {
					//NEVER strengthen a road connecting you and enemy
					TryUpgradeRoad(r.endpoints[0], r.endpoints[1], r);
					
				}
				
				//NEVER DESTROY ROADS
				
			}
		}
		


		/// <summary>
		/// sends N armies from start to end
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="num"></param>
		public void SendArmy(Node start, Node end, int num)
		{	
			Road r = RoadBetween(start, end);
			
			if (r == null 
			    || start.armyStrength < 80 //don't send small armies
			    || start == end)
				return;
			
			if (num <= 3)
				for (int i = 0; i < num; i++) {
					start.SplitHalf(end, r);
				}
			else
				start.SendAll(end, r);
		}
		
		/// <summary>
		/// calculates how much help to send
		/// </summary>
		/// <param name="source"></param>
		/// <param name="sink"></param>
		/// <returns>number of armies to send (1/2, 1/4, 1/8, etc)</returns>
		public int ArmiesToHelp(Node source, Node sink)
		{
			int NEED = NetIncoming(sink) - sink.armyStrength;

			//can only reinforce other team members,
			if (source.owner == this
			    && sink.owner == this
			    && source != sink
			    //minimum =(19+1)/2=10 army size reinforcement
			    && source.armyStrength > 19

			    //don't reinforce things too far away
			    && NewUnits(source, sink) <= 40)
			{

				bool danger = false;
				foreach (Node f in source.nodesConnected)
				{
					//check if i'm in danger --> self-preservation first
					if (f.owner != this && UTILS.DistSquared(f.loc, source.loc) <
					    UTILS.DistSquared(source.loc, sink.loc)
					    && 7 * f.armyStrength / 8 + NetIncoming(source) > source.armyStrength)
						
						danger = true;
				}

				//if need to send reinforcement ==============FOR COMBAT
				if (!danger)
				{
					//prioritize combat before harvesting!!
					if (NEED > 0)
					{
						if (source.armyStrength / 2 > NEED)
							return 2;
						else if (3 * source.armyStrength / 4 > NEED)
							return 3;
						else if (7 * source.armyStrength / 8 > NEED)
							return 4;
						else return 0;
					}

					float sourceEnemies = 0;
					float sinkEnemies = 0;
					foreach (Node f in source.nodesConnected)
					{
						//calculate power of possible attack
						float a = NewUnits(f, source);
						if (f.owner != this) sourceEnemies += f.armyStrength - a;
					}
					foreach (Node f in sink.nodesConnected)
					{
						//calculate power of possible attack
						float a = NewUnits(f, source);
						if (f.owner != this) sinkEnemies += f.armyStrength - a;
					}

					if (sinkEnemies > sourceEnemies)
					{
						//prevention of enemy attack by coagulating at possible intersects of attack
						return 2;

					}
					
				}
			}
			return 0;
		}
		
		/// <summary>
		/// mobilizes forces to the "front lines"
		/// </summary>
		/// <param name="me"></param>
		public void Harvest(Node me)
		{
			
			Node friend = null;
			Node enemyofFriend = null; //the enemy closest/connected to friend
			
			//selects a connected friendly factory that meet certain criteria:
			float distSquared = 500 * 500;
			foreach (Node fac in me.nodesConnected) {
				Node temp = ClosestNodeTo(fac, RoadExistence.yes, NodeOwner.enemy);
				if (temp != null) {
					//if friend is directly connected to enemy, immediately send help
					friend = fac;
					enemyofFriend = temp;
					break;
				} else {
					//if no friends directly connected to enemy, get the one that is closest to enemy
					temp = ClosestNodeTo(fac, RoadExistence.no, NodeOwner.enemy);
					if (temp == null)
						continue;
					float d = UTILS.DistSquared(fac.loc, temp.loc);
					
					if (distSquared > d) {
						distSquared = d;
						friend = fac;
						enemyofFriend = temp;
					}
				}
			}
			if (friend == null)
				return;
			
			Node enemyOfMe;
			enemyOfMe = ClosestNodeTo(me, RoadExistence.yes, NodeOwner.enemy);
			
			if (enemyOfMe == null) //only harvest if i'm NOT connected to enemy
				enemyOfMe = ClosestNodeTo(me, RoadExistence.no, NodeOwner.enemy);
			else
				return; //if i'm connected to enemy, can't harvest
			
			if (enemyOfMe == null) //if its STILL null (no enemies)
				return;
				
			float dist = UTILS.DistSquared(enemyOfMe.loc, me.loc);
			
			if (dist > UTILS.DistSquared(enemyofFriend.loc, friend.loc)) {
				SendArmy(me, friend, 4); //sendAll
			}
		}
		public int NetIncoming(Node f)
		{
			//returns net incoming armies, friendlies & enemies balance out
			int attacker_pop = 0;
			foreach (Road r in f.roadsConnected) {
				foreach (Army a in r.armies) {
					if (a.target == f.loc) {
						if (a.owner == this)
							attacker_pop += a.num;
						else
							attacker_pop -= a.num;
					}
				}
			}
			return attacker_pop;
		}
		

		public Road ShouldUpgrade()
		{
			//list of all of MY nodes with >= 3 connections
			foreach (Node f in this.factoriesOwned)
			{
				if (f.roadsConnected.Count >= 3
				    && NetIncoming(f) <= f.armyStrength - roadCost)
				{
					foreach (Node f2 in f.nodesConnected)
					{
						//upgrade proportionally end roads leading to each end factory
						//only upgrade if not under attack
						if (f2.roadsConnected.Count >= 3
						    && f2.owner == this
						    && (int)RoadBetween(f, f2).rdtype <
						    (f.roadsConnected.Count+f2.roadsConnected.Count)/3 -1)

							return RoadBetween(f, f2);
					}
				}
			}
			return null;
		}
		/// <summary>
		/// should i attack?
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="toAttack"></param>
		/// <returns></returns>
		public int ArmiesToSend(Node attacker, Node toAttack)
		{

			if (attacker.owner == this
			    && toAttack.owner != this
			    && NetIncoming(attacker) <= 0 //make sure I'm not being attacked
			    && NewUnits(attacker, toAttack) <= 80) //don't attack too far things
			{
				int reinforcement_possible= 0;

				foreach (Node f in toAttack.nodesConnected)
				{
					//if reinforcement can come on time
					if(f.owner!=this && NumTravelTicks(f, toAttack)<=NumTravelTicks(attacker, toAttack))
						reinforcement_possible += f.armyStrength / 4;
				}
				
				
				int road_expense = 0;
				//take into account cost of building a new road
				if (RoadBetween(attacker, toAttack) == null) road_expense = roadCost;

				//enemy strength sum
				float N = toAttack.armyStrength + 
					NewUnits(attacker, toAttack) + 
					reinforcement_possible + 
					road_expense +
					NetIncoming(toAttack);

				//if already being attacked successfully
				if (N < 0 ) return 0;
				else
				{
					//only attack if i am nearly sure to win
					if (.75 * attacker.armyStrength > N)
						return 4; //sendAll
				}
			}
			return 0;
		}
		
		public float NumTravelTicks(Node start, Node end)
		{
			//so if used in reinforcement senses, no reinforcements will come
			if (RoadBetween(start, end) != null)
				return UTILS.Distance(start.loc, end.loc) / RoadBetween(start, end).travelSpeed;
			else
			{
				//imagine road was built, calculate odds --> will build later if good choice?
				Road r = new Road(start, end, RoadTypes.Dirt);
				return UTILS.Distance(start.loc, end.loc) / r.travelSpeed;
			}
		}
		/// <summary>
		/// calculates how many new units will be produced by the time help arrives
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public float NewUnits(Node start, Node end)
		{
			return NumTravelTicks(start, end) * mainTickInterval / buildTickInterval;
		}
		
		/// <summary>
		/// returns the road between the two allFacs, if any
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public Road RoadBetween(Node start, Node end)
		{
			foreach (Road r in start.roadsConnected) {
				if (r.endpoints.Contains(end))
					return r;
			}
			return null;
		}
		
		public Node ClosestNodeTo(Node fromThis, RoadExistence mustHaveRoad, NodeOwner f_owner)
		{
			//slight optimization
			List<Node> f_temp;
			if(mustHaveRoad == RoadExistence.yes)
			{
				f_temp = UTILS.GetClosestList(fromThis.loc, fromThis.nodesConnected);
			}		
			else
				f_temp = UTILS.GetClosestList(fromThis.loc, allFacs);
			
			foreach (Node f in f_temp) {
				if (f == fromThis) //closest fac can't be itself!
					continue;
				if (f_owner == NodeOwner.me && f.owner == this ||
				    f_owner == NodeOwner.enemy && f.owner != this && f.owner != null ||
				    f_owner == NodeOwner.noone && f.owner == null ||
				    f_owner == NodeOwner.anyone ||
				    f_owner == NodeOwner.notMe && f.owner != this) {
					
					if (mustHaveRoad == RoadExistence.no && RoadBetween(fromThis, f) == null
					   || mustHaveRoad == RoadExistence.any) {
										
						return f;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// finds the closest factory of a certain owner or group of owners from ANY one of
		/// my own allFacs
		/// </summary>
		/// <param name="f_owner"></param>
		/// <param name="mustHaveRoad"></param>
		/// <returns></returns>
		public Node ClosestNodeOf(NodeOwner f_owner, RoadExistence mustHaveRoad)
		{
			float d = 2000*2000; Node closestFac=null;

			foreach (Node myFac in this.factoriesOwned) {
				foreach (Node f in allFacs) {
					if (mustHaveRoad == RoadExistence.yes && RoadBetween(myFac, f) != null
					    || mustHaveRoad == RoadExistence.no && RoadBetween(myFac, f) == null
					    || mustHaveRoad == RoadExistence.any) {
						
						if (f_owner == NodeOwner.me && f.owner == this ||
						    f_owner == NodeOwner.enemy && f.owner != this && f.owner != null ||
						    f_owner == NodeOwner.noone && f.owner == null ||
						    f_owner == NodeOwner.anyone ||
						    f_owner == NodeOwner.notMe && f.owner != this) {
							
							if (UTILS.DistSquared(f.loc, myFac.loc) <= d) {
								d = UTILS.DistSquared(f.loc, myFac.loc);
								closestFac = f;
							}
						}
					}
				}
				
			}
			return closestFac;
		}
		
		
		
	}
}
