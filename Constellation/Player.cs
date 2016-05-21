using System;
using System.Collections.Generic;

using System.Drawing;

namespace Constellation
{
	public class Player
	{
		public string name{ get; private set; }
		public Color color{ get; private set; }
		public void SetColor(Color c)
		{
			this.color = c;
		}
		public int dead;
        
		public bool is_AI = false;
        
		public List<Army> armies = new List<Army>();
		public List<Node> nodesOwned = new List<Node>();
		
		protected Rectangle gameWorld;
        
		public int roadCost {
			get{ return this.numFactories; }
		}
        
		public Player(string name, Color color, Rectangle gameWorld)
		{
			this.gameWorld = gameWorld;
			this.name = name;
			this.color = color;
			//my own random sound
		}
		public List<Player> getEnemyPlayers(List<Player> players)
		{
			players.Remove(this);
			return players;
		}
		public List<Army> getEnemyArmies(List<Player> players)
		{
			List<Army> temp = new List<Army>();
			foreach (Player p in getEnemyPlayers(players)) {
				temp.AddRange(p.armies);
			}
			return temp;       	
		}
		public List<Army> getEnemyArmies(List<Army> allArmies)
		{
			List<Army> temp = new List<Army>();
			foreach (Army a in allArmies) {
				if (a.owner != this)
					temp.Add(a);
			}
			return temp;
		}
		public void Update(List<ParticleEmitter> particleEmitters)
		{
			foreach (Army myArmy in this.armies) {
				myArmy.Move();	

				//optimized: checks only collisions for all armies on the same road				
				foreach (Army a in myArmy.road.armies) {
					//proximity check for fighting
					if (UTILS.DistSquared(myArmy.loc, a.loc) < Math.Pow(myArmy.radius + a.radius, 2)
					    && myArmy.road == a.road //and on same road
					   	&& myArmy.owner != a.owner) { //is enemy
						particleEmitters.Add(new ParticleEmitter(
							new Point((a.loc.X + a.loc.X) / 2,
								(a.loc.Y + a.loc.Y) / 2),
							myArmy.owner.color, a.owner.color, myArmy.num + a.num));
						myArmy.CollideEnemy(a);
					}
				}
			}
		}
        
		public void RemoveArmy(Army a)
		{
			armies.Remove(a);
		}
		public int numFactories {
			get { return nodesOwned.Count; }
		}
		
		
		public bool TryBuildNewRoad(Node start, Node end, List<Road> roads)
		{
			//checks if can legitimately build new road
			if (start == null 
			    || start == end 
			    || start.owner != this)
				return false;
			
			
			foreach (Road r in start.roadsConnected) {
				if (r.Connects(start, end))
					return false; //already has road
			}

			if (start.armyStrength >= start.owner.roadCost)//if enough money
			{
				start.armyStrength -= start.owner.roadCost;
				NewRoad(start, end, roads);
				return true;
			}
			return false;
            
		}
		public bool TryDestroyRoad(Node start, Node end, Road r, List<Road> roads)
		{
			//free
			if (r.Connects(start, end)
			    && start.owner ==this && end.owner==this
			    && r.armies.Count==0) { //and if noone is on the road

				DestroyRoad(r, roads);
				return true;
			}
			return false;
		}
		public bool TryUpgradeRoad(Node start, Node end, Road r)
		{
			if (r.Connects(start, end)
			    && start.owner !=null
			    && start.armyStrength >= start.owner.roadCost) {
				
				start.armyStrength -= start.owner.roadCost;
				UpgradeRoad(r);
				return true;
			}
			return false;
		}
		
        #region possible road commands
        public void UpgradeRoad(Road r)
        {
            r.Upgrade();
        }
        public void DestroyRoad(Road r, List<Road> roads)
        {
            foreach (Node f in r.endpoints)
	        {		 
            	//update factory's info of connections
                f.roadsConnected.Remove(r);
                f.nodesConnected.Remove(r.endpoints[1-r.endpoints.IndexOf(f)]);
            }
            roads.Remove(r);
        }
		public void NewRoad(Node start, Node end, List<Road> roads)
		{
			Road r = new Road(start, end, RoadTypes.Dirt);
			
			//update information
			start.roadsConnected.Add(r);
			end.roadsConnected.Add(r);
			start.nodesConnected.Add(end);
			end.nodesConnected.Add(start);
			
			roads.Add(r);
	
			if (end.owner == null) { //if neutral fac
				/*
				particleEmitters.Add(new ParticleEmitter(
					end.loc, start.owner.color, start.owner.color, 10));
					*/

				end.NewOwner(start.owner);
			}
			
		}
        #endregion

	}
}
