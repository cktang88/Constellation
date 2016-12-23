using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
	public class Game
	{
		/* TODO:
       	 * Currently, on small screens, things move faster b/c less pixels
		 * Make game speed resolution independent
         */
         
         
		#region initial setup

		Form1 form;
		BoardType boardtype;
		public static bool sendAll;
		
		public int Numplayers{ get; private set; }
		//Sounds backgroundSound = new Sounds();
		Renderer renderer;
		public Rectangle gameWorld{ get; private set; }
		
		public Random r{ get; private set; }
        
		public static int BUILDTICK;
		public static int MAINTICK;

		public Game(int Numplayers, BoardType boardtype)
		{
			Game.players = new List<Player>();
			Game.factorynodes = new List<Node>();
			Game.roads = new List<Road>();
        	
			renderer = new Renderer(Theme.light);
			r = new Random(); 
			
			//NOTE: if boardtype is boardtype.same, keep old board type
			if (boardtype != BoardType.Same)
				this.boardtype = boardtype;
			
			
			this.Numplayers = Numplayers;
			
			//========setup a good game screen to fit device
			gameWorld = new Rectangle(40, 40, 1200, 650);

			if (Numplayers < 2)
				players.Add(new AI("Ares", Color.Yellow, this.gameWorld));

			if (Numplayers == 0)
				players.Add(new AI("Poseidon", Color.Cyan, this.gameWorld));

			if (Numplayers > 0)
				players.Add(new Player("Kwuang", Color.Red, this.gameWorld));

			if (Numplayers == 2)
				players.Add(new Player("Hulk", Color.Green, this.gameWorld));
	
			// Create game board
			List<Point> fac_LocList = BoardMaker.MakeBoard(boardtype, this.gameWorld);
			//and this pins a factory node end each pre-determined location of factories..
			foreach (Point loc in fac_LocList) {
				factorynodes.Add(new Node(loc));
			}
			//add player nodes
			AddPlayerStartNodes();
		}
		public void AddPlayerStartNodes()
		{
			foreach (Player	p in players) {
				int c = r.Next(1, factorynodes.Count);
				if (factorynodes[c].owner == null) {
					factorynodes[c].NewOwner(p);
				} else {
					//try again
					factorynodes[r.Next(1, factorynodes.Count)].NewOwner(p);
				}
			}
		}
		public static List<Node> factorynodes;
		public static List<Player> players;
		public static List<Road> roads;
		
		#endregion

		public void BuildUpArmies()
		{
			foreach (Node f in factorynodes) {
				//only increment for factories that are owned by player
				if (f.owner == null)
					continue;
                    
				f.IncreaseArmy();
                    
			}
		}
		public int totalTime { get; private set; }
		public Player Go(int tmr_interval)
		{
			totalTime += tmr_interval;

			//all AI players move
			foreach (Player p in players) {
				if (p.GetType() == typeof(AI)) {
					p.Do();
				}
			}
            

			// armies get absorbed into factories
			foreach (Node f in factorynodes) {
				foreach (Road road in f.roadsConnected) {
					for (int i = 0; i < road.armies.Count; i++) {
						Army a = road.armies[i];
						if (UTILS.DistSquared(f.loc, a.loc) < Math.Pow(f.radius + a.radius, 2)
						    && f.loc == a.target) {
							
							ParticleEmitter pe = new ParticleEmitter(
								                     new Point((a.loc.X + f.loc.X) / 2, (a.loc.Y + f.loc.Y) / 2),
								                     a.owner.color, f.owner.color, a.num + f.armyStrength);
							
							this.renderer.AddParticleEmitter(pe);
                            
							f.Join(a);
						}
						
					}
				}
                
			}
            
			//remove dead
			foreach (Player p in players) {
				for (int i = 0; i < p.armies.Count; i++) {
					Army a = p.armies[i];
					if (!a.shouldRemove)
						continue;
					
					//Notify players and roads of dead armies
					a.owner.armies.Remove(a);
					a.road.armies.Remove(a);
					i--;
					
				}
			}
            
			return CheckForWinner();
		}
		public void Draw(Graphics g, Theme theme, bool showStats)
		{
			renderer.UpdateInfo(showStats, theme);
			renderer.Render(this, g);
		}
		Player CheckForWinner()
		{
			foreach (Player p in players) {
				bool won = true;
				foreach (Player enemy in players) {
					if (p != enemy) {
						if (enemy.numFactories > 0 || enemy.armies.Count > 0)
							won = false;
					}
				}
				if (won && p.armies.Count < 2)
					return p;
			}
			return null;
		}
		public Node target{ get; private set; }
		public void MouseUpdate(Point mouse)
		{
			target = UTILS.GetClosest(mouse, factorynodes);
		}
		public void MouseSlide(Point mouseStart, Point mouseEnd, MouseMode mousemode)
		{
			//gets closest endpoint factory end where mouse let go
			Node fac_end = UTILS.GetClosest(mouseEnd, factorynodes);

			//gets closest factory end where mouse started
			Node fac_start = UTILS.GetClosest(mouseStart, factorynodes);
            			
			if (fac_end.loc == fac_start.loc
			    || fac_start.owner == null
			    || fac_start.owner.is_AI) //AI can't use mouse!!!
				
				return;
            
			//make sure not degenerate
			int COST = 0;
			bool alreadyHasRoad = false; 
			if (fac_start.owner != null)
				COST = fac_start.owner.roadCost;
                
			//can't use foreach b/c modified by DestroyRoad() or UpgradeRoad()
			for (int i = 0; i < fac_start.roadsConnected.Count; i++) { 	
				//finds the road that connects the two things that is owned by player
				Road road = fac_start.roadsConnected[i];
				if (road.endpoints.Contains(fac_end)) {
					if (mousemode == MouseMode.SendArmy) {
						// can send all armies on base via holding shift key
						if (sendAll)
							fac_start.SendAll(fac_end, road);
						else
							fac_start.SplitHalf(fac_end, road);
                            
					} else if (mousemode == MouseMode.UpgradeRoads) {
                            
						fac_start.owner.TryUpgradeRoad(fac_start, fac_end, road);
					} else if (mousemode == MouseMode.DestroyRoads) {
						//must own both nodes end destroy roads for fairness reasons
						fac_start.owner.TryDestroyRoad(fac_start, fac_end, road);
					}

					alreadyHasRoad = true;
				}
			}
			if (!alreadyHasRoad) {
				fac_start.owner.TryBuildNewRoad(fac_start, fac_end);
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
                total += f.armyStrength;
                if (f.owner == me) mystuff += f.armyStrength;
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
