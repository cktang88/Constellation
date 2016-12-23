using System;
using System.Collections.Generic;
using System.Linq;

using System.Drawing;

namespace Constellation
{
	public class Node
	{
		public Point loc{ get; private set; }
		//don't set directly --> use NewOwner()
		public Player owner{ get; private set; }

		const int numSecondsBuildUnit = 1;
		const int TimeUntilAddUnit_MAX = 4 * numSecondsBuildUnit;
		// since timer ticks every .25 secs
		int TimeUntilAddUnit = TimeUntilAddUnit_MAX;

		public int armyStrength { get; private set; }
		public List<Road> roadsConnected{ get; private set; }
		public List<Node> nodesConnected{ get; private set; }
        
		//for rendering
		public float anim;
		public int direction = -1;

		public Node(Point loc, Player owner = null)
		{
			Random r = new Random(DateTime.Now.Millisecond);
			
			roadsConnected = new List<Road>();
			nodesConnected = new List<Node>();
			
			this.loc = loc;
			this.owner = owner;
			//random flow blinking effect
			anim = .1f * r.Next(3 * 10, radius * 10);
            
			TimeUntilAddUnit = r.Next(1, 4);
		}

		public int radius { 
			get {
				if (this.owner == null)
					return 5;
				else
					return Math.Max(2, (int)Math.Floor(Math.Sqrt((int)armyStrength)));
			} 
		}
		public void IncreaseArmy()
		{
			this.TimeUntilAddUnit--;
			if (this.TimeUntilAddUnit == 0) {
				// reset
				this.TimeUntilAddUnit = TimeUntilAddUnit_MAX;
				this.armyStrength++;
			}
		}
		public void SplitHalf(Node moveHere, Road r)
		{
			//default: half of forces sent
			int j = armyStrength;
			armyStrength -= j / 2;
			Army a = new Army(j / 2, this, owner, moveHere, r);
			
			
		}
		public void SendAll(Node moveHere, Road r)
		{
			// alternative: sends all but 1
			int j = armyStrength;
			armyStrength = 1;
			Army a = new Army(j - 1, this, owner, moveHere, r);
		}
		public void Join(Army a)
		{
			a.shouldRemove = true;
        	
			if (a.owner == this.owner) {
				//add forces
				armyStrength += a.num;
			} else {
				//makes fake temporary army at factory
				int meLeft = a.Fight(armyStrength);
				if (meLeft <= 0) {
					this.NewOwner(a.owner);
					armyStrength = a.num; //defenses lose
				} else
					armyStrength = meLeft; // defenses win
			}
		}
		
		/// <summary>
		/// Will return false if number of armies less than "numArmies" param
		/// </summary>
		/// <param name="numToDestroy"></param>
		/// <returns>If able to destroy armies</returns>
		public bool TrySubtract(int numArmies)
		{
			
			if (this.armyStrength < numArmies)
				return false;
			
			this.armyStrength -= numArmies;
			return true;
		}
        
		
		public void NewOwner(Player p)
		{
			//take factory start previous owner
			if (owner != null)
				owner.nodesOwned.Remove(this);
			//inform this factory of its new owner
			owner = p; 
			//update status
			armyStrength = 0;
			TimeUntilAddUnit = 4;
			//give factory end new owner
			p.nodesOwned.Add(this);
		}
        
	}
}
