using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
	
	/*
	 * =================== NOTES: 
	 * 
     * 1. this AI is VERY VERY dependent upon unit move speed
     * 2. a lot of the range checks the AI uses are hard-coded
     * (ranges should be based on factory avg dist, environment size, etc. instead)
	 * 
	 */
	public class AI : Player
	{
		int TimeUntilNextMove = 0;
		
		int minArmySize = 60;
		//prevents many spammy tiny armies
		
		bool isPlayingHuman = false;
		//if its playing humans, play slower (no simultaneous moves, etc.)
		
		public AI(string name, Color color, Rectangle gameWorld, List<Player> players)
			: base(name, color, gameWorld)
		{
			base.is_AI = true;
			Random r = new Random(DateTime.Now.Millisecond);
			for (int i = 0; i < 3; i++) {
				int c = r.Next(1, 5);
				while (strategySpots.Contains(c))
					c = r.Next(1, 5);
				strategySpots.Add(c);
			}
			
			foreach (Player p in players) {
				if (!p.is_AI)
					isPlayingHuman = true;
			}
			
		}
		List<int> strategySpots = new List<int>();
		bool strategy_setup = false;
		
		
		public void Do()
		{
			
			if (isPlayingHuman) {
				//one move per second?
				TimeUntilNextMove--;
				if (TimeUntilNextMove > 0) //wait until i can move again, to prevent moving too much
				return;
			
				TimeUntilNextMove = 30; //reset timer to move
			}
			
			//initial strategy setup of spreading forces
			if (!strategy_setup) {
				this.MacroStrategy();
			}
			
			foreach (Node fac in Game.factorynodes) {	
				
				#region ===== CAPTURE NEW NODES =====
				
				//NOTE: many checks avoided b/c if factory owner is null, that means
				//that no player owns it, so no roads connected to it.
				if (fac.owner == null && strategy_setup) {
					//my closest factory to "fac" that has no roads between them
					Node x = ClosestNodeTo(fac, RoadExistence.no, NodeOwner.me);
					
					//the closest enemy factory to "fac", roads or not
					Node y = ClosestNodeTo(fac, RoadExistence.no, NodeOwner.enemy);
				
					if (x != null && y != null) {
						if (NetNearbyIncoming(x) <= 0) { //if i'm not under attack
							//if i'm stronger
							if (x.armyStrength - 3 * roadCost > y.armyStrength
							    //or if i'm closer and nearly equal or greater strength
							    || UTILS.DistSquared(x.loc, fac.loc) < UTILS.DistSquared(y.loc, fac.loc)
							    && x.armyStrength + 5 > y.armyStrength) {
								
								TryBuildNewRoad(x, fac);
								if (this.isPlayingHuman)
									return; //SLOWS DOWN FOR HUMANS
							}
						}
						
						//allow AI to get the 1st move
						if (y.owner.numFactories == 1)
							TryBuildNewRoad(x, fac);
					}
				}
				
				#endregion
					
				#region =====   ARMY MOVEMENT   =====
				if (fac.owner == this) {
					List<Node> f_temporary = UTILS.GetClosestList(fac.loc, Game.factorynodes);
					foreach (Node enemy in f_temporary) {
						
						//verify target is an enemy
						if (enemy.owner == this || enemy.owner == null)
							continue;
						
						//only directly attack nearby things
						if (UTILS.DistSquared(fac.loc, enemy.loc) > 300 * 300)
							continue;

						int n = ArmiesToSend(fac, enemy);
						if (n <= 0)
							continue;
							
						if (Network.RoadBetween(fac, enemy) != null) {//if road exists
							SendArmy(fac, enemy, n);
							if (this.isPlayingHuman)
								return; //SLOWS DOWN FOR HUMANS
						} else {
							/* would rather pass army through intermediate node to destination than build new road
							 */
							Node intermediate = Network.GetIntermediate(this, fac, enemy, NodeOwner.me);
							
							if (intermediate != null) {
								SendArmy(fac, intermediate, n);
								if (this.isPlayingHuman)
									return; //SLOWS DOWN FOR HUMANS
							} else if (TryBuildNewRoad(fac, enemy)) {
								SendArmy(fac, enemy, n);
								if (this.isPlayingHuman)
									return; //SLOWS DOWN FOR HUMANS
							}
								
						}
					}
						
					//"big clumps" quickly build & upgrade quick roads to attack enemies
					if (fac.armyStrength >= 200) {
						int sum = 0;
						foreach (Node n in fac.nodesConnected) {
							if (n.owner == fac.owner)
								sum += n.armyStrength;
						}
						if (fac.armyStrength > 4 * sum) {
							Node enemy = ClosestNodeTo(fac, RoadExistence.any, NodeOwner.enemy);
							if (enemy != null) {
								if (fac.armyStrength * .8 > enemy.armyStrength//if strong enough
								    //&& fac.nodesConnected.Count <= 2//it is isolated
								    && UTILS.DistSquared(fac.loc, enemy.loc) < 300 * 300) { //if nearby
									
									TryBuildNewRoad(fac, enemy);
									SendArmy(fac, enemy, 4);
									
									if (this.isPlayingHuman)
										return; //SLOWS DOWN FOR HUMANS
									
								} else if (fac.nodesConnected.Count == 1) { //if at end of a network

									//"bridge the gap" to nearest friend closer to frontlines								
									Node friend = ClosestNodeTo(fac, RoadExistence.no, NodeOwner.me);
									if (UTILS.DistSquared(fac.loc, enemy.loc) >
									    UTILS.DistSquared(friend.loc, enemy.loc)) {
										
										TryBuildNewRoad(fac, friend);
										SendArmy(fac, friend, 4);
										
										if (this.isPlayingHuman)
											return; //SLOWS DOWN FOR HUMANS
									}
								}
							}
						}
					}
						
					//TODO: make use of ArmiesToHelp() method!!!! for reinforcing
						
						
					if (Harvest(fac) && isPlayingHuman) //harvests from fac
						return; //SLOWS DOWN FOR HUMAN
				}
				#endregion
			}
			
			if (OptimizeRoads() && isPlayingHuman)
				return; //SLOWS DOWN FOR HUMANS
		}

		/// <summary>
		/// Calculates how much help to send
		/// </summary>
		/// <param name="source"></param>
		/// <param name="sink"></param>
		/// <returns>fraction of armies to send (1/2, 1/4, 1/8, etc)</returns>
		int ArmiesToHelp(Node source, Node sink)
		{
			int NEED = NetNearbyIncoming(sink) - sink.armyStrength;

			//can only reinforce other team members,
			if (source.owner == this
			    && sink.owner == this
			    && source != sink
			    //minimum =(19+1)/2=10 army size reinforcement
			    && source.armyStrength > 19

			    //don't reinforce things too far away
			    && Network.NewUnits(source, sink) <= 40) {

				bool danger = false;
				foreach (Node f in source.nodesConnected) {
					//check if i'm in danger --> self-preservation first
					if (f.owner != this && UTILS.DistSquared(f.loc, source.loc) <
					    UTILS.DistSquared(source.loc, sink.loc)
					    && 7 * f.armyStrength / 8 + NetNearbyIncoming(source) > source.armyStrength)
						danger = true;
				}

				//if need to send reinforcement (combat)
				if (!danger) {
					//prioritize combat before harvesting!!
					if (NEED > 0) {
						if (source.armyStrength / 2 > NEED)
							return 2;
						else if (3 * source.armyStrength / 4 > NEED)
							return 3;
						else if (7 * source.armyStrength / 8 > NEED)
							return 4;
						else
							return 0;
					}

					float sourceEnemies = 0;
					float sinkEnemies = 0;
					foreach (Node f in source.nodesConnected) {
						//calculate power of possible attack
						float a = Network.NewUnits(f, source);
						if (f.owner != this)
							sourceEnemies += f.armyStrength - a;
					}
					foreach (Node f in sink.nodesConnected) {
						//calculate power of possible attack
						float a = Network.NewUnits(f, source);
						if (f.owner != this)
							sinkEnemies += f.armyStrength - a;
					}

					if (sinkEnemies > sourceEnemies) {
						//prevent of enemy attack by coagulating at possible intersects of attack
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
		/// <returns>Whether a move was made (if playing vs human)</returns>
		bool Harvest(Node me)
		{
			
			Node friend = null;
			Node enemyofFriend = null; //the enemy closest/connected to friend
			
			//selects a connected friendly factory that meet certain criteria:
			float distSquared = 2000 * 2000;
			foreach (Node fac in me.nodesConnected) {
				Node temp = ClosestNodeTo(fac, RoadExistence.yes, NodeOwner.enemy);
				if (temp != null) {
					//if friend is directly connected to enemy, immediately send help
					friend = fac;
					enemyofFriend = temp;
					break;
				}
				
				
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
			if (friend == null)
				return false;
			
			Node enemyOfMe;
			enemyOfMe = ClosestNodeTo(me, RoadExistence.yes, NodeOwner.enemy);
			
			if (enemyOfMe == null) //only harvest if i'm NOT connected to enemy
				enemyOfMe = ClosestNodeTo(me, RoadExistence.no, NodeOwner.enemy);
			else
				return false; //if i'm connected to enemy, can't harvest
			
			if (enemyOfMe == null) //if its STILL null (no enemies)
				return false;
				
			float dist = UTILS.DistSquared(enemyOfMe.loc, me.loc);
			
			if (dist > UTILS.DistSquared(enemyofFriend.loc, friend.loc)) {
				SendArmy(me, friend, 4); //sendAll
				if (this.isPlayingHuman)
					return true; //SLOWS DOWN FOR HUMANS
			}
			return false;
		}
		/// <summary>
		/// upgrades useful roads and deletes inefficient ones
		/// </summary>
		/// <returns>Whether a move was made (if playing vs human)</returns>
		bool OptimizeRoads()
		{
			//list of all of MY nodes with >= 3 connections
			foreach (Node f in this.nodesOwned) {
				if (f.roadsConnected.Count < 3
				    || NetNearbyIncoming(f) > 0) //if not being attacked
				continue;
				
				
				for (int i = 0; i < f.nodesConnected.Count; i++) {
					Node f2 = f.nodesConnected[i];
					Road r = Network.RoadBetween(f, f2);
					
					if (f2.roadsConnected.Count <= 1//less requirements for destination
					    || f2.owner != this) //never upgrade to the enemy
						continue; 
					
					float lengthSquared = UTILS.DistSquared(f.loc, f2.loc);
					
					
					/*
					 * Deleting roads can be useful!
					 * 1. looks better
					 * 2. prevents AI from wasting computations
					 * 3. AI algorithms work better b/c will only use short roads to travel on
					 * 
					 */
					if (lengthSquared > 400 * 400
					    //never leave a node unconnected
					    && f.roadsConnected.Count > 1
					    && f2.roadsConnected.Count > 1) {
						if (TryDestroyRoad(f, f2, r)) {
							
							if (this.isPlayingHuman)
								return true; //SLOWS DOWN FOR HUMANS
							
							i--;
						}
					}
					
					//upgrade only heavily connected nodes
					if ((int)r.rdtype < (f.roadsConnected.Count + f2.roadsConnected.Count) / 3 - 1
					    && lengthSquared < 300 * 300//only upgrade nearby nodes
					    && .25 * f.armyStrength > roadCost) //upgrade only if lots of armies to spare
						
						TryUpgradeRoad(f, f2, r);
					if (this.isPlayingHuman)
						return true; //SLOWS DOWN FOR HUMANS
				}
				
			}
			
			return false;
		}
		/// <summary>
		/// should i attack?
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="toAttack"></param>
		/// <returns></returns>
		int ArmiesToSend(Node attacker, Node toAttack)
		{

			if (attacker.owner == this
			    && toAttack.owner != this
			    && NetNearbyIncoming(attacker) <= 0) { //make sure I'm not being attacked
				
				int road_expense = 0;
				//take into account cost of building a new road
				if (Network.RoadBetween(attacker, toAttack) == null)
					road_expense = roadCost;
				
				//prevents building road to enemy and not attacking
				if (attacker.armyStrength < road_expense + minArmySize)
					return 0;

				//enemy strength sum
				float N = toAttack.armyStrength +
				          Network.NewUnits(attacker, toAttack) +
				          road_expense +
				          NetNearbyIncoming(toAttack);

				//if already being attacked successfully
				if (N < 0)
					return 0;
				
				//only attack if i am nearly sure to win
				if (.75 * attacker.armyStrength > N)
					return 4; //sendAll
				
			}
			return 0;
		}
		
		
		/// <summary>
		/// sends N armies from start to end
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="num"></param>
		void SendArmy(Node start, Node end, int num)
		{	
			Road r = Network.RoadBetween(start, end);
			
			if (r == null
			    || start.armyStrength < minArmySize//don't send small armies
			    || start == end)
				return;
			
			if (num <= 3)
				for (int i = 0; i < num; i++) {
					start.SplitHalf(end, r);
				}
			else
				start.SendAll(end, r);
		}
		
		
		//wrapper methods
		private Node ClosestNodeTo(Node fromThis, RoadExistence mustHaveRoad, NodeOwner f_owner)
		{
			return Network.ClosestNodeTo(this, fromThis, mustHaveRoad, f_owner);
		}
		private int NetNearbyIncoming(Node f, int maxdist = 300)
		{
			return Network.NetNearbyIncoming(this, f, maxdist);
		}
		
		void MacroStrategy()
		{
			if (TopMostFac.owner == null && strategySpots.Contains(1))
				TryBuildNewRoad(ClosestNodeTo(TopMostFac, RoadExistence.no,
					NodeOwner.me), TopMostFac);
			else if (BottomMostFac.owner == null && strategySpots.Contains(2)) {
				TryBuildNewRoad(ClosestNodeTo(BottomMostFac, RoadExistence.no,
					NodeOwner.me), BottomMostFac);
			} else if (LeftMostFac.owner == null && strategySpots.Contains(3)) {
				TryBuildNewRoad(ClosestNodeTo(LeftMostFac, RoadExistence.no,
					NodeOwner.me), LeftMostFac);
			} else if (RightMostFac.owner == null && strategySpots.Contains(4)) {
				TryBuildNewRoad(ClosestNodeTo(RightMostFac, RoadExistence.no,
					NodeOwner.me), RightMostFac);
			} else if (CenterMostFac.owner == null && strategySpots.Contains(5)) {
				TryBuildNewRoad(ClosestNodeTo(CenterMostFac, RoadExistence.no,
					NodeOwner.me), CenterMostFac);
			} else
				strategy_setup = true;
				
			if (this.isPlayingHuman)
				return; //SLOWS DOWN FOR HUMANS
		}
		
		#region center-, left-, right-, top-, bottom- most Game.factorynodes
		Node CenterMostFac {
			get {
				Point p = new Point(gameWorld.Width / 2, gameWorld.Height / 2);
				return UTILS.GetClosest(p, Game.factorynodes);
			}
		}
		Node LeftMostFac {
			get {
				Point p = new Point(0, gameWorld.Height / 2);
				return UTILS.GetClosest(p, Game.factorynodes);
			}
		}
		Node RightMostFac {
			get {
				Point p = new Point(gameWorld.Width, gameWorld.Height / 2);
				return UTILS.GetClosest(p, Game.factorynodes);
			}
		}
		Node TopMostFac {
			get {
				Point p = new Point(gameWorld.Width / 2, 0);
				return UTILS.GetClosest(p, Game.factorynodes);
			}
		}
		Node BottomMostFac {
			get {
				Point p = new Point(gameWorld.Width / 2, gameWorld.Height);
				return UTILS.GetClosest(p, Game.factorynodes);
			}
		}
		
		#endregion
	}
}
