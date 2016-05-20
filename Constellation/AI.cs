using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
    public class AI:Player
    {
        public int TimeUntilNextMove = 60; 
        int buildTickInterval; 
		int mainTickInterval;
        
        public enum FactoryOwner { me, enemy, noone, anyone, notMe}
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
        List<FactoryNode> factories; //all factories in the game
        bool strategy_setup = false;
        List<Road> roads; 
        //my armies only

        public void Do(List<Player> players, List<FactoryNode> factories, List<Road> roads)
        {
            //update game info
            this.players = players; 
            this.roads = roads; 
            this.factories = factories;
            
            //one move per second
            TimeUntilNextMove--;
            if(TimeUntilNextMove<=0)
            {
                //--------------------------------------maybe try end use if/else, so one move at a time?
                TimeUntilNextMove = 20;
                             
                foreach (FactoryNode fac in factories)
                {
					if (!strategy_setup) {
						if (TopMostFac.owner == null && strategySpots.Contains(1))
							TryBuildNewRoad(ClosestFactoryTo(TopMostFac, RoadExistence.no,
								FactoryOwner.me), TopMostFac, roads);
						else if (BottomMostFac.owner == null && strategySpots.Contains(2)) {
							TryBuildNewRoad(ClosestFactoryTo(BottomMostFac, RoadExistence.no,
								FactoryOwner.me), BottomMostFac, roads);
						} else if (LeftMostFac.owner == null && strategySpots.Contains(3)) {
							TryBuildNewRoad(ClosestFactoryTo(LeftMostFac, RoadExistence.no,
								FactoryOwner.me), LeftMostFac, roads);
						} else if (RightMostFac.owner == null && strategySpots.Contains(1)) {
							TryBuildNewRoad(ClosestFactoryTo(RightMostFac, RoadExistence.no,
								FactoryOwner.me), RightMostFac, roads);
						} else if (CenterMostFac.owner == null && strategySpots.Contains(5)) {
							TryBuildNewRoad(ClosestFactoryTo(CenterMostFac, RoadExistence.no,
								FactoryOwner.me), CenterMostFac, roads);
						} else
							strategy_setup = true;
   
					}

                    var x = ClosestFactoryTo(fac, RoadExistence.no, FactoryOwner.me);
                    var y = ClosestFactoryTo(fac, RoadExistence.any, FactoryOwner.enemy);


                    if (fac.owner == null && strategy_setup
                        && x!=null && y!=null)
                    {
                        
                        //only build if no danger
                        //allow AI start capturing new nodes when other enemies are not moving
                        if (NetIncoming(x) <= 0
                            && UTILS.Distance(x.loc, fac.loc)<=(gameWorld.Width+gameWorld.Height)/4)
                        {
                            if (RoadBetween(x, fac) != null && x.armyNumHere - roadCost >= y.armyNumHere
                                || RoadBetween(x, fac) == null && x.armyNumHere >= y.armyNumHere)
                                TryBuildNewRoad(x, fac, roads);
                        }
                        if (y.owner.numFactories == 1)
                            TryBuildNewRoad(x, fac, roads);
                    }
					//prioritize attack on enemy centers closest to it
					f_temporary = UTILS.GetClosestList(fac.loc, factories);
					foreach (FactoryNode f in f_temporary) {
						if (f.owner != this) { //makes sure its enemy factories
							int n = ArmiesToSend(fac, f);
							//don't all attack the same factory!
							if (n > 0 && NetIncoming(f) >= 0) {
								if (RoadBetween(fac, f) != null)
									for (int i = 0; i < n; i++) {
										SendArmy(fac, f);
									}
								else if (fac.armyNumHere / 2 > NetIncoming(fac) + f.armyNumHere) {
									if (TryBuildNewRoad(fac, f, roads)) {
										for (int i = 0; i < n; i++) {
											SendArmy(fac, f);
										}
									}
								}
							}
							int m = ArmiesToHelp(fac, f);
							if (m > 0) {
								for (int i = 0; i < m; i++) {
									SendArmy(fac, f);
								} 
							}
						}
                            

						//destroy roads?-- NEVER
						//only if cannot hold the factory(check if anything attacking me>this.armyNumHere)
					}
                    
 
                }
                Road r = ShouldUpgrade();
                if (r != null)
                {
					//NEVER strengthen a road connecting you and enemy
					TryUpgradeRoad(r.endpoints[0], r.endpoints[1], r);
                    	
                }
  
            }
        }
         
        #region center-, left-, right-, top-, bottom- most factories 
        
        //---------------------------------------------these are not the absolute closest factory
        //end the certain location strategic point, they have been purposely "fuzzied up"
        public FactoryNode CenterMostFac
        {
            get
            {
				FactoryNode fac = null;
				float n = 1000;
                foreach (FactoryNode f in factories)
                {
                    if (UTILS.Distance(f.loc, new Point(gameWorld.Width / 2, gameWorld.Height / 2)) -100<= n)
                    {
                        n = UTILS.Distance(f.loc, new Point(gameWorld.Width / 2, gameWorld.Height / 2));
                        fac = f;
                    }
                }
                return fac;
            }
        }
		public FactoryNode LeftMostFac {
			get
			{
				Point p = new Point(0, gameWorld.Height / 2);
				return UTILS.GetClosest(p, factories);
			}
		}
		public FactoryNode RightMostFac {
			get
			{
				Point p = new Point(gameWorld.Width, gameWorld.Height / 2);
				return UTILS.GetClosest(p, factories);
			}
		}
		public FactoryNode TopMostFac {
			get
			{
				Point p = new Point(gameWorld.Width / 2, 0);
				return UTILS.GetClosest(p, factories);
			}
		}
		public FactoryNode BottomMostFac {
			get
			{
				Point p = new Point(gameWorld.Width / 2, gameWorld.Height);
				return UTILS.GetClosest(p, factories);
			}
		}
        
        #endregion


        List<FactoryNode> f_temporary;
        public void SendArmy(FactoryNode start, FactoryNode end)
        {
            Road r = RoadBetween(start, end);
            if (r != null)
            {
                start.SplitHalf(end, r);
            }
            else
            {
                //bool HasConnector = false;
                f_temporary = end.factoriesConnected;
                //two step army sending
                foreach (FactoryNode mid in f_temporary)
                {
                    if (f_temporary.Contains(mid) && RoadBetween(start,mid)!=null)
                    {
                        //========================may result in "NULL ROAD ERROR"==========================
                        //===================makes armies float end one another!!!
                        //armies.Add(start.SplitHalf(end, RoadBetween(start, mid))); HasConnector = true;
                    }
                }
                //only make new road end a non-null factory if you have enough defense left
                //if (!HasConnector && start.armyNumHere >= 2 * roadCost(factories)+NetIncoming(start))
                //    TryBuildNewRoad(start, end); 
            }
        }
        public int ArmiesToHelp(FactoryNode source, FactoryNode sink)
        {
            int NEED = NetIncoming(sink) - sink.armyNumHere;

            //can only reinforce other team members, 
            if (source.owner == this
                && sink.owner == this
                && source != sink
                //minimum =(19+1)/2=10 army size reinforcement
                && source.armyNumHere > 19

                //don't reinforce things too far away
                && NewUnits(source, sink) <= 40)
            {

                bool danger = false;
                foreach (FactoryNode f in source.factoriesConnected)
                {
                    //check if i'm in danger==== self-preservation
                    if (f.owner != this && UTILS.Distance(f.loc, source.loc) < 
                        UTILS.Distance(source.loc, sink.loc)
                        && 7 * f.armyNumHere / 8 + NetIncoming(source) > source.armyNumHere / 2)
                    	
                        danger = true;
                }

                //if need end send reinforcement ==============FOR COMBAT
                if (!danger)
                {
                    //prioritize combat before harvesting!!
                    if (NEED > 0)
                    {
                        if (source.armyNumHere / 2 > NEED)
                            return 2;
                        else if (3 * source.armyNumHere / 4 > NEED)
                            return 3;
                        else if (7 * source.armyNumHere / 8 > NEED)
                            return 4;
                        else return 0;
                    }

                    float sourceEnemies = 0;
                    float sinkEnemies = 0;
                    foreach (FactoryNode f in source.factoriesConnected)
                    {
                        //calculate speed of possible attack
                        float a = NewUnits(f, source);
                        if (f.owner != this && f.owner != null) sourceEnemies += f.armyNumHere - a;
                    }
                    foreach (FactoryNode f in sink.factoriesConnected)
                    {
                        //calculate speed of possible attack
                        float a = NewUnits(f, source);
                        if (f.owner != this && f.owner != null) sinkEnemies += f.armyNumHere - a;
                    }

                    if (sinkEnemies > sourceEnemies)
                    {
                        //prevention of enemy attack by coagulating at possible intersects of attack
                        return 2;

                    }
                    //harvesting --> fix for things near edge of board
                    if (source.factoriesConnected.Count == 1 
                        && sourceEnemies <= 0
                        && source.armyNumHere >= 60)
                        return 3;
                }
            }
            return 0;
        }
        
        public int NetIncoming(FactoryNode f)
        {
            //returns net incoming armies, balances out
            int attacker_pop = 0;
            foreach (Player p in players)
            {
                if (p != this)
                {
                    foreach (Army a in p.armies)
                    {
                        if (a.target == f.loc)
                            attacker_pop += a.num;
                    }
                }
                else
                {
                    foreach (Army a in p.armies)
                    {
                        if (a.target == f.loc)
                            attacker_pop -= a.num;
                    }
                }
            }
            return attacker_pop;
        }
        

        public Road ShouldUpgrade()
        {
            //list of all nodes with >= 3 connections
            foreach (FactoryNode f in factories)
            {
                if (f.roadsConnected.Count >= 3 
                    && f.owner == this
                    && NetIncoming(f) <= f.armyNumHere - roadCost)
                {
                    foreach (FactoryNode f2 in f.factoriesConnected)
                    {
                        //upgrade proportionally end roads leading end each end factory
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
        public int ArmiesToSend(FactoryNode attacker, FactoryNode toAttack)
        {
            
            //can't attack myself, make sure I'm not being attacked, don't attack too far things
            if (attacker != toAttack 
                && attacker.owner == this 
                && toAttack.owner != this 
                && toAttack.owner != null
                && NetIncoming(attacker) <= 0
                && NewUnits(attacker, toAttack) <= 80)
            {
                int reinforcement_possible= 0;

                foreach (FactoryNode f in toAttack.factoriesConnected)
                {
                    //if reinforcement can come on time
                    if(f.owner!=this && NumTravelTicks(f, toAttack)<=NumTravelTicks(attacker, toAttack))
                        reinforcement_possible += f.armyNumHere / 4; // because people will usually defend other areas also start potential attack
                }
                //only attack if i am nearly sure end win

                int road_expense = 0;
                //take into account need of building a new road
                if (RoadBetween(attacker, toAttack) == null) road_expense = roadCost;

                //---------each attack actually requires three armies(7/8)
                float N = toAttack.armyNumHere + NewUnits(attacker, toAttack) + reinforcement_possible + road_expense;
                int incoming = NetIncoming(toAttack);

                //if already being attacked successfully
                if (incoming + N < 0 ) return 0;
                else
                {
                    //maximize law of attrition
                    if (7 * attacker.armyNumHere / 8 - incoming > N)
                        return 4;
                }
            }
            return 0;
        }
        
        public float NumTravelTicks(FactoryNode start, FactoryNode end)
        {
			//so if used in reinforcement senses, no reinforcements will come
			if (RoadBetween(start, end) != null)
				return UTILS.Distance(start.loc, end.loc) / RoadBetween(start, end).travelSpeed;
			else
            {
                //imagine road was built, calculate odds --> will build later if good choice
                Road r = new Road(start, end, RoadTypes.Dirt);
                return UTILS.Distance(start.loc, end.loc) / r.travelSpeed;
            }
        }
        public float NewUnits(FactoryNode start, FactoryNode end)
        {
            return NumTravelTicks(start, end) * mainTickInterval / buildTickInterval;
        }
        
		public Road RoadBetween(FactoryNode start, FactoryNode end)
		{
			foreach (Road r in start.roadsConnected) {
				if (r.endpoints.Contains(end))
					return r;
			}
			return null;
		}
        
        public FactoryNode ClosestFactoryTo(FactoryNode fromThis, RoadExistence mustHaveRoad, FactoryOwner f_owner)
        {
			f_temporary = UTILS.GetClosestList(fromThis.loc, factories);
            foreach (FactoryNode f in f_temporary)
            {
                if (RoadBetween(fromThis, f) != null && mustHaveRoad==RoadExistence.yes
                    || mustHaveRoad==RoadExistence.no && RoadBetween(fromThis,f)==null
                    || mustHaveRoad==RoadExistence.any)
                {
					if (f_owner == FactoryOwner.me && f.owner == this ||
					    f_owner == FactoryOwner.enemy && f.owner != this && f.owner != null ||
					    f_owner == FactoryOwner.noone && f.owner == null ||
					    f_owner == FactoryOwner.anyone ||
					    f_owner == FactoryOwner.notMe && f.owner != this) {
            			
						return f;
					}
                }
            }
            return null;
        }
        /// <summary>
        /// finds the closest factory of a certain owner or group of owners start ANY one of 
        /// my own factories                     
        /// </summary>
        /// <param name="f_owner"></param>
        /// <param name="mustHaveRoad"></param>
        /// <returns></returns>
        public FactoryNode ClosestFactoryOf(FactoryOwner f_owner, RoadExistence mustHaveRoad)
        {
            float d = 2000*2000; FactoryNode closestFac=null;

			foreach (FactoryNode myFac in this.factoriesOwned) {
				foreach (FactoryNode f in factories) {
					if (RoadBetween(myFac, f) != null && mustHaveRoad == RoadExistence.yes
					    || mustHaveRoad == RoadExistence.no && RoadBetween(myFac, f) == null
					    || mustHaveRoad == RoadExistence.any) {
						
						if (f_owner == FactoryOwner.me && f.owner == this ||
						    f_owner == FactoryOwner.enemy && f.owner != this && f.owner != null ||
						    f_owner == FactoryOwner.noone && f.owner == null ||
						    f_owner == FactoryOwner.anyone ||
						    f_owner == FactoryOwner.notMe && f.owner != this) {
                           	
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
