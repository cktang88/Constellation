using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RoLL
{
    public class AI:Player
    {
        public int TimeUntilNextMove = 20; int buildTickInterval; int mainTickInterval; Game game;
        public AI(string name, Color color, int buildTick, int mainTick, Game game):base(name, color, game)
        {
            this.buildTickInterval = buildTick; this.mainTickInterval = mainTick; this.game = game;
            base.is_AI = true;
            for (int i = 0; i < 3; i++)
            {
                Random r= new Random();
                int c = r.Next(1, 5);
                while (strategySpots.Contains(c))
                    c = r.Next(1, 5);
                strategySpots.Add(c);
            }
        }
        List<Player> players; List<int> strategySpots = new List<int>();
        List<FactoryNode> factories; bool strategy_setup = false;
        List<Road> roads; 
        //my armies only

        public void Do(List<Player> players, List<FactoryNode> factories, List<Road> roads)
        {
            //update game info
            this.players = players; this.roads = roads; this.factories = factories;
            //one move per second
            TimeUntilNextMove--;
            if(TimeUntilNextMove==0)
            {
                //--------------------------------------maybe try to use if/else, so one move at a time?
                TimeUntilNextMove = 20;
                             
                foreach (FactoryNode fac in factories)
                {
                    if (!strategy_setup)
                    {
                        if (TopMostFactory.owner == null && strategySpots.Contains(1))
                            AttemptNewRoad(ClosestFactoryTo(TopMostFactory, RoadExistence.no,
                                FactoryOwner.me), TopMostFactory);
                        else
                        {
                            if (BottomMostFactory.owner == null && strategySpots.Contains(2))
                            {
                                AttemptNewRoad(ClosestFactoryTo(BottomMostFactory, RoadExistence.no,
                                  FactoryOwner.me), BottomMostFactory);
                            }
                            else
                            {
                                if (LeftMostFactory.owner == null && strategySpots.Contains(3))
                                {
                                    AttemptNewRoad(ClosestFactoryTo(LeftMostFactory, RoadExistence.no,
                                        FactoryOwner.me), LeftMostFactory);
                                }
                                else
                                {
                                    if (RightMostFactory.owner == null && strategySpots.Contains(1))
                                    {
                                        AttemptNewRoad(ClosestFactoryTo(RightMostFactory, RoadExistence.no,
                                            FactoryOwner.me), RightMostFactory);
                                    }
                                    else
                                    {
                                        if (CenterMostFactory.owner == null && strategySpots.Contains(5))
                                        {
                                            AttemptNewRoad(ClosestFactoryTo(CenterMostFactory, RoadExistence.no,
                                              FactoryOwner.me), CenterMostFactory);
                                        }
                                        else
                                            strategy_setup = true;
                                    }
                                }
                            }
                        }
                    }

                    var x = ClosestFactoryTo(fac, RoadExistence.no, FactoryOwner.me);
                    var y = ClosestFactoryTo(fac, RoadExistence.any, FactoryOwner.enemy);

                    if (fac.owner == null && strategy_setup
                        && x!=null && y!=null)
                    {
                        
                        //only build if no danger
                        //allow AI from capturing new nodes when other enemies are not moving
                        if (NetIncoming(x) <= 0
                            && dist(x.loc, fac.loc)<=(game.form.Width+game.form.Height)/4)
                        {
                            if(RoadBetween(x, fac)!=null && x.armyNumHere - Roadcost(factories)>= y.armyNumHere
                                || RoadBetween(x, fac)==null && x.armyNumHere>=y.armyNumHere)
                                    AttemptNewRoad(x, fac);
                        }
                        if(y.owner.numFactories(factories) == 1)
                            AttemptNewRoad(x, fac);
                    }
                    //prioritize attack on enemy centers closest to it
                    f_temporary = factories.OrderBy(i => dist(i.loc, fac.loc)).ToList();
                    foreach (FactoryNode f in f_temporary)
                    {
                        int n = ArmiesToSend(fac, f);
                            //don't all attack the same factory!
                        if (n > 0 && NetIncoming(f) >= 0)
                        {
                            if(RoadBetween(fac,f)!=null)
                                for (int i = 0; i < n; i++) { SendArmy(fac, f); }
                            else
                                if (fac.armyNumHere / 2 > NetIncoming(fac) + f.armyNumHere)
                                {
                                    if(AttemptNewRoad(fac, f))
                                    {
                                        for (int i = 0; i < n; i++) { SendArmy(fac, f); }
                                    }
                                }
                        }
                            int m = ArmiesToHelp(fac, f);
                        if (m > 0)
                        {
                            for (int i = 0; i < m; i++) { SendArmy(fac, f); }
                        }
                            

                        //destroy roads?-- NEVER
                        //only if cannot hold the factory(check if anything attacking me>this.armyNumHere)
                    }
                    
 
                }
                Road r = ShouldUpgrade();
                if (r != null)
                {
                    //NEVER strengthen a road connecting you and enemy
                    AttemptUpgradeRoad(r.endpoints[0], r.endpoints[1],
                        r);
                }
  
            }
        }
         
        #region center-, left-, right-, top-, bottom- most factories 
        
        //---------------------------------------------these are not the absolute closest factory
        //to the certain location strategic point, they have been purposely "fuzzied up"
        public FactoryNode CenterMostFactory
        {
            get
            {
                FactoryNode fac = null; double n=1000;
                foreach (FactoryNode f in factories)
                {
                    if (dist(f.loc, new Point(game.form.Width / 2, game.form.Height / 2)) -100<= n)
                    {
                        n = dist(f.loc, new Point(game.form.Width / 2, game.form.Height / 2));
                        fac = f;
                    }
                }
                return fac;
            }
        }
        public FactoryNode LeftMostFactory
        {
            get
            {
                FactoryNode fac = null; double n = 1000;
                foreach (FactoryNode f in factories)
                {
                    if (dist(f.loc, new Point(0, game.form.Height / 2)) -100<= n)
                    {
                        n = dist(f.loc, new Point(0, game.form.Height / 2));
                        fac = f;
                    }
                }
                return fac;
            }
        }
        public FactoryNode RightMostFactory
        {
            get
            {
                FactoryNode fac = null; double n = 1000;
                foreach (FactoryNode f in factories)
                {
                    if (dist(f.loc, new Point(game.form.Width, game.form.Height / 2)) - 100 <= n)
                    {
                        n = dist(f.loc, new Point(game.form.Width, game.form.Height / 2));
                        fac = f;
                    }
                }
                return fac;
            }
        }
        public FactoryNode TopMostFactory
        {
            get
            {
                FactoryNode fac = null; double n = 1000;
                foreach (FactoryNode f in factories)
                {
                    if (dist(f.loc, new Point(game.form.Width / 2, 0)) - 100 <= n)
                    {
                        n = dist(f.loc, new Point(game.form.Width / 2, 0));
                        fac = f;
                    }
                }
                return fac;
            }
        }
        public FactoryNode BottomMostFactory
        {
            get
            {
                FactoryNode fac = null; double n = 1000;
                foreach (FactoryNode f in factories)
                {
                    if (dist(f.loc, new Point(game.form.Width / 2, game.form.Height)) - 100 <= n)
                    {
                        n = dist(f.loc, new Point(game.form.Width / 2, game.form.Height));
                        fac = f;
                    }
                }
                return fac;
            }
        }
        
        #endregion


        List<FactoryNode> f_temporary;
        public void SendArmy(FactoryNode from, FactoryNode to)
        {
            Road r = RoadBetween(from, to);
            if (r != null)
            {
                armies.Add(from.SplitHalf(to, r));
            }
            else
            {
                bool HasConnector = false;
                f_temporary = to.factoriesConnected;
                //two step army sending
                foreach (FactoryNode mid in f_temporary)
                {
                    if (f_temporary.Contains(mid) && RoadBetween(from,mid)!=null)
                    {
                        //========================may result in "NULL ROAD ERROR"==========================
                        //===================makes armies float to one another!!!
                        //armies.Add(from.SplitHalf(to, RoadBetween(from, mid))); HasConnector = true;
                    }
                }
                //only make new road to a non-null factory if you have enough defense left
                //if (!HasConnector && from.armyNumHere >= 2 * Roadcost(factories)+NetIncoming(from))
                //    AttemptNewRoad(from, to); 
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
                    if (f.owner != this && dist(f.loc, source.loc) < dist(source.loc, sink.loc)
                        && 7 * f.armyNumHere / 8 + NetIncoming(source) > source.armyNumHere / 2)
                        danger = true;
                }

                //if need to send reinforcement ==============FOR COMBAT
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

                    double sourceEnemies = 0;
                    double sinkEnemies = 0;
                    foreach (FactoryNode f in source.factoriesConnected)
                    {
                        //calculate speed of possible attack
                        double a = NewUnits(f, source);
                        if (f.owner != this && f.owner != null) sourceEnemies += f.armyNumHere - a;
                    }
                    foreach (FactoryNode f in sink.factoriesConnected)
                    {
                        //calculate speed of possible attack
                        double a = NewUnits(f, source);
                        if (f.owner != this && f.owner != null) sinkEnemies += f.armyNumHere - a;
                    }

                    if (sinkEnemies > sourceEnemies)
                    {
                        //prevention of enemy attack by coagulating at possible intersects of attack
                        return 2;

                    }
                    //harvesting
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
                if(p==this)
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
                    && NetIncoming(f) <= f.armyNumHere - Roadcost(factories))
                {
                    foreach (FactoryNode f2 in f.factoriesConnected)
                    {
                        //upgrade proportionally to roads leading to each end factory
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
        public int ArmiesToSend(FactoryNode attacker, FactoryNode toAttack)
        {
            //should I attack??
            
            //can't attack myself, make sure I'm not being attacked, don't attack too far things
            if (attacker != toAttack 
                && attacker.owner == this 
                && toAttack.owner != this 
                && toAttack.owner != null
                && NetIncoming(attacker) <= 0
                && NewUnits(attacker, toAttack) <= 80)
            {
                double reinforcement_possible= 0;

                foreach (FactoryNode f in toAttack.factoriesConnected)
                {
                    //if reinforcement can come on time
                    if(f.owner!=this && NumTravelTicks(f, toAttack)<=NumTravelTicks(attacker, toAttack))
                        reinforcement_possible += f.armyNumHere / 2;
                }
                //only attack if i am nearly sure to win

                int road_expense = 0;
                //take into account need of building a new road
                if (RoadBetween(attacker, toAttack) == null) road_expense = Roadcost(factories);

                //---------each attack actually requires three armies(7/8)
                double N = toAttack.armyNumHere + NewUnits(attacker, toAttack) + reinforcement_possible + road_expense;
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
        
        public double NumTravelTicks(FactoryNode from, FactoryNode to)
        {
            //so if used in reinforcement senses, no reinforcements will come
            if (RoadBetween(from, to) != null)
                return dist(from.loc, to.loc) / RoadBetween(from, to).travelSpeed;
            else
            {
                //imagine road was built, calculate odds
                Road r = new Road(from, to, RoadTypes.Dirt);
                return dist(from.loc, to.loc) / r.travelSpeed;
            }
        }
        public double NewUnits(FactoryNode from, FactoryNode to)
        {
            //=DB/rA
            return NumTravelTicks(from, to) * mainTickInterval / buildTickInterval;
        }
        
        public Road RoadBetween(FactoryNode from, FactoryNode to)
        {
            foreach (Road r in from.roadsConnected)
            {
                if (r.endpoints.Contains(to))
                    return r;
            }
                return null;
        }
        
        public enum FactoryOwner { me, enemy, noone, anyone, notMe}
        public enum RoadExistence { yes, no, any}
        
        public double dist(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
        }
        
        public FactoryNode ClosestFactoryTo(FactoryNode fromThis, RoadExistence mustHaveRoad, FactoryOwner f_owner)
        {
            f_temporary = factories.OrderBy(i => dist(i.loc, fromThis.loc)).ToList();
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
                                f_owner == FactoryOwner.notMe && f.owner != this)
                    {
                        return f;
                    }
                }
            }
            return null;
        }
       
        public FactoryNode ClosestFactoryOf(FactoryOwner f_owner, RoadExistence mustHaveRoad)
        {
            double distance = 5000; FactoryNode closestFac=null;

            foreach (FactoryNode myFac in factories)
            {
                if (myFac.owner == this)
                {
                    foreach (FactoryNode f in factories)
                    {
                        if (RoadBetween(myFac, f) != null && mustHaveRoad==RoadExistence.yes
                        || mustHaveRoad==RoadExistence.no && RoadBetween(myFac,f)==null
                        || mustHaveRoad==RoadExistence.any)
                        {
                            /*
                            finds the closest factory of a certain owner or group of owners from one of 
                            my own factories
                            */
                            if (f_owner == FactoryOwner.me && f.owner == this ||
                                f_owner == FactoryOwner.enemy && f.owner != this && f.owner != null ||
                                f_owner == FactoryOwner.noone && f.owner == null ||
                                f_owner == FactoryOwner.anyone ||
                                f_owner == FactoryOwner.notMe && f.owner != this)
                            {
                                if (dist(f.loc, myFac.loc) <= distance)
                                {
                                    distance = dist(f.loc, myFac.loc); closestFac = f;
                                }
                            }
                        }
                    }
                }
            }
            return closestFac;
        }
        
        
        
    }
}
