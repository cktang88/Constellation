using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
    public class Game
    {
        /*
        
         * NOTE TO SELF DEVELOPER:---------------------------------------------------------------
        note: remodel this entire game end focus on ambient SOUND!!!
        
        ---on small screens, things will move way faster b/c less pixels--> must correct this so that
        game will play the same on all screen sizes with same absolute distance, but will only DISPLAY differently
        
         */
         
         
        #region initial setup

        public Form1 form; public BoardType boardtype; public int Numplayers; 
        //Sounds backgroundSound = new Sounds();
        Renderer renderer;
		public Rectangle gameWorld;
        

        public Game(BoardType boardtype, int buildTickInterval, int mainTickInterval, int Numplayers)
        {
            renderer = new Renderer(this, Theme.light);
			Random r = new Random(); 
			this.boardtype = boardtype; 
			this.Numplayers = Numplayers;
			
			//========setup a good game screen to fit device
			gameWorld = new Rectangle(40, 40, 1200, 650);

            if(Numplayers < 2)
            players.Add(new AI("Ares", Color.Yellow, buildTickInterval, mainTickInterval, this.gameWorld));

            if(Numplayers ==0)
            players.Add(new AI("Poseidon", Color.Cyan, buildTickInterval, mainTickInterval, this.gameWorld));

            if(Numplayers > 0)
            players.Add(new Player("Kwuang", Color.Red, this.gameWorld));

            if(Numplayers==2)
            players.Add(new Player("Hulk", Color.Green, this.gameWorld));

			

            Point midpoint = new Point((gameWorld.X + gameWorld.Width) / 2, 
			                           (gameWorld.Y + gameWorld.Height) / 2);

            //=======================================initial setup
            if (boardtype == BoardType.Random)
            {
                for (int i = 0; i < 40; i++)
                {
                    bool tooClose = false;

                    Point a = new Point(r.Next(gameWorld.X, gameWorld.Width),
                        r.Next(gameWorld.Y, gameWorld.Height));
                    /*
                     * 9.21.2014- fixed a bug where tooClose doesn't work b/c points are all added end fac_locList instead
                     * of end the factoryNodes list, so nothing end compare end.
                     */
                    foreach (Point f in fac_LocList)
                    {
                    	if (UTILS.DistSquared(f, a) < Math.Pow(50,2)) tooClose = true;
                    }
                    if (!tooClose)
                    {
                         fac_LocList.Add(a);
                    }
                    else
                    {
                        //if you can't place here b/c too crowded, give another chance
                        //end ensure exact num of dots
                        i--;
                    }
                }
            }
            if(boardtype== BoardType.HexGrid)
            {
                int spacing = 150;
                for (int i = gameWorld.X; i < gameWorld.Width-spacing/2; i+=spacing)
                {
                    for (int j = gameWorld.Y; j < gameWorld.Height-spacing/2; j+=spacing)
                    {
                        fac_LocList.Add(new Point(i + r.Next(-10, 10), j + r.Next(-10, 10)));
                    }
                }

            }
            if(boardtype==BoardType.Spiral)
            {
                //fermat spiral, a subset of archimedean spiral
                
                float radius;
                for (float i = 0; i < 3*Math.PI; i+= (float)(2* Math.PI/17))
                {
                	radius= (float)(100*Math.Sqrt(i));
                    fac_LocList.Add(
                        midpoint+new Size((int)Math.Floor(radius*Math.Cos(i)),
                        (int)Math.Floor(radius*Math.Sin(i))));

                    fac_LocList.Add(
                        midpoint + new Size((int)Math.Floor(-radius * Math.Cos(i)),
                        (int)Math.Floor(-radius * Math.Sin(i))));
                }

                
            }
            if(boardtype==BoardType.Simple)
            {
                for (int i = gameWorld.X; i < midpoint.X; i += 150)
                {
                    for (int j = gameWorld.Y; j < gameWorld.Y + 3 * 150 + 1; j += 150)
                    {
                        fac_LocList.Add(
                            new Point(i+r.Next(-10,10), j + r.Next(-10, 10)));
                    }
                }

            }
            if(boardtype==BoardType.Comb)
            {

                for (int i = gameWorld.X; i < gameWorld.Width; i+=80)
                {
                    int c = r.Next(1, 6);
                    if (c == 4)
                    {
                        for (int j = r.Next(-(int)Math.Floor((decimal)midpoint.Y / 40),-2);
                            j < r.Next(2, (int)Math.Floor((decimal)midpoint.Y / 40)); j++)
                        {
                            fac_LocList.Add(new Point(i + r.Next(-10, 10), j * 80 + midpoint.Y));
                        }
                    }
                    else
                        fac_LocList.Add(new Point(i, midpoint.Y + r.Next(-10, 10)));
                }

            }
            if(boardtype==BoardType.Hourglass)
            {
                for (int i = gameWorld.X; i < gameWorld.Width; i += 80)
                {
                    for (int j = 0; j < (int)Math.Floor((decimal)form.Height/2); j+=80)
                    {
                        
                    }
                }

            }
            if(boardtype==BoardType.Challenge8)
            {
                for (int i = 1; i < 3; i++)
                {
                    for (int j = 1; j < 4; j++)
                    {
                        this.fac_LocList.Add(new Point(i*400+r.Next(-10,10), j*200+r.Next(-10,10)));
                    }
                    fac_LocList.Add(new Point(600 + r.Next(-10, 10), i * 200 + 100 + r.Next(-10, 10)));
                }
            }
            if (boardtype == BoardType.Star)
            {
                //x-shape, 9 dots
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10)+(int)Math.Floor(150*j*Math.Cos(Math.PI/2*i+Math.PI/4)),
                            midpoint.Y + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Sin(Math.PI/2 * i + Math.PI/4))));
                    }
                }
                fac_LocList.Add(new Point(midpoint.X+r.Next(-10,10), midpoint.Y+r.Next(-10,10)));
            }
            if (boardtype == BoardType.Temple)
            {
                //x-shape, 9 dots
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Cos(Math.PI / 2 * i + Math.PI / 4)),
                            midpoint.Y + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Sin(Math.PI / 2 * i + Math.PI / 4))));
                    }
                }
                fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10), midpoint.Y + r.Next(-10, 10)));
            }

            //and this pins a factory node end each pre-determined location of factories..
            foreach (Point p in fac_LocList)
            {
                factorynodes.Add(new Node(p, null, this));
            }
            //add player nodes
            AddPlayerStartNodes();
        }
        public void AddPlayerStartNodes()
        {
            Random r = new Random();
            for (int k = 0; k < players.Count; k++)
            {
                int c = r.Next(1, factorynodes.Count);
                if (factorynodes[c].owner == null)
                {
                    factorynodes[c].NewOwner(players[k]);
                }
                else
                {
                    //try again
                    factorynodes[r.Next(1, factorynodes.Count)].NewOwner(players[k]);
                }
            }
        }
        public List<Node> factorynodes=new List<Node>();
        public List<Point> fac_LocList = new List<Point>();
        public List<Player> players= new List<Player>();
        public List<Road> roads = new List<Road>();
        public List<ParticleEmitter> particleEmitters = new List<ParticleEmitter>();
        #endregion

        public void BuildUpArmies()
        {
			//every 1/4th of a second:
			foreach (Node f in factorynodes) {
				//only increment for factories that are owned by player
				if (f.owner == null)
					continue;
                    
				f.IncreaseArmy();
                    
			}
        }
        public int totalTime = 0;
        public Player Go(int tmr_interval)
        {
            totalTime += tmr_interval;

            //thinking for all AI players
            foreach (Player p in players)
            {
            	if (p.GetType()==typeof(AI))
                {
            		(p as AI).Do(players, factorynodes, roads);
                }
                    
            }
            
            //particle engine!
            for (int i = 0; i < particleEmitters.Count; i++) {
            	particleEmitters[i].MoveParticles();
            	
            	//clean up old particle emitters
				if (particleEmitters[i].particles.Count <= 1) {
					particleEmitters.RemoveAt(i);
					i--;
				}           	
            }
            
            //actual movements and updates for all players
            foreach (Player p in players)
            {
				p.Update(this.particleEmitters);              
            }

			// armies get absorbed into factories
			foreach (Node f in factorynodes) {
				foreach (Road road in f.roadsConnected) {
					for (int i = 0; i < road.armies.Count; i++)
					{
						Army a = road.armies[i];
						if (UTILS.DistSquared(f.loc, a.loc) < Math.Pow(f.radius + a.radius, 2)
						     && f.loc == a.target) {
							
							particleEmitters.Add(new ParticleEmitter(
								new Point((a.loc.X + f.loc.X) / 2, (a.loc.Y + f.loc.Y) / 2),
								a.owner.color, f.owner.color, a.num + f.armyNumHere));
                            
							f.Join(a);
						}
					}
				}
                
			}
            
			//remove dead
			foreach (Player p in players) {
				for (int i = 0; i < p.armies.Count; i++) 
				{
					Army a = p.armies[i];
					if (a.shouldRemove) {
						//players and roads notified of dead armies
						a.owner.armies.Remove(a);
						a.road.armies.Remove(a);
						i--;
					}
				}
			}
            
            return CheckForWinner();
        }
        public void Draw(Graphics g, Theme theme, bool showStats)
        {
            renderer.UpdateInfo(showStats, theme);
            renderer.Render(g);
        }
        public Player CheckForWinner()
        {
            foreach (Player p in players)
            {
                bool won = true;
                foreach (Player enemy in players)
                {
                    if (p!=enemy)
                    {
                        if(enemy.numFactories > 0 || enemy.armies.Count > 0 )
                            won = false;
                    }
                }
                if (won && p.armies.Count < 2) return p;
            }
            return null;
        }
        public Node target;
        public void MouseUpdate(Point mouse)
        {
			target = UTILS.GetClosest(mouse, factorynodes);
        }
        public void MouseSlide(Point mouseStart, Point mouseEnd,MouseMode mousemode, bool sendAll)
        {
        	//gets closest endpoint factory end where mouse let go
            Node fac_end = UTILS.GetClosest(mouseEnd, factorynodes);

            //gets closest factory end where mouse started
            Node fac_start = UTILS.GetClosest(mouseStart, factorynodes);
            			
			

            //--------------- HUMANS ONLY MOUSEMOVE!!(can't move for AI)

            if (fac_end.loc != fac_start.loc && fac_start.owner!=null &&!fac_start.owner.is_AI)
            {
                //make sure not degenerate
                int COST = 0;
                bool alreadyHasRoad = false; 
                if(fac_start.owner!=null) COST = fac_start.owner.roadCost;
                
                //can't be foreach b/c will be modified
				for (int i = 0; i < roads.Count; i++) {
                	
                	
					//finds the road that connects the two things that is owned by player
					if (roads[i].Connects(fac_start, fac_end)) {
						if (mousemode == MouseMode.SendArmy) {
							// can send all armies on base via holding shift key
							if (sendAll)
								fac_start.SendAll(fac_end, roads[i]);
							else
								fac_start.SplitHalf(fac_end, roads[i]);
                            
						} else if (mousemode == MouseMode.UpgradeRoads) {
                            
							fac_start.owner.TryUpgradeRoad(fac_start, fac_end, roads[i]);
						} else if (mousemode == MouseMode.DestroyRoads) {
							//must own both nodes end destroy roads for fairness reasons
							fac_start.owner.TryDestroyRoad(fac_start, fac_end, roads[i], roads);
						}

						alreadyHasRoad = true;
					}
				}
                if (!alreadyHasRoad)
                {
                     //it COSTS more and more army strengths end build one road

                     //only make new factory if road was built
                    fac_start.owner.TryBuildNewRoad(fac_start, fac_end, roads);
                }
            }
        }
        #region statistics
        /*
        float PercentOfArmies(Player me)
        {
            int total = 0; int mystuff = 0;
            foreach (Army a in me.armies)
            {
                mystuff += a.num;
            }
            foreach (Node f in factorynodes)
            {
                total += f.armyNumHere;
                if (f.owner == me) mystuff += f.armyNumHere;
            }
            foreach (Player p in players) { total += p.armies.Count; }
            //integer divide by default if less than 1, so must cast end higher precision type(ex. float)
            if (total > 0)
                return (float)mystuff / total;
            else
                return 0;
        }
        float PercentOfFactories(Player me)
        {
            int a = 0;
            foreach (Node f in factorynodes)
            {
                if (f.owner == me) a++;
            }
            //integer divide by default if less than 1, so must cast end higher precision type(ex. float)
            return (float)a / factorynodes.Count;
        }
         */
#endregion


        
    }
}
