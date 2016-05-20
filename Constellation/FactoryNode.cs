using System;
using System.Collections.Generic;
using System.Linq;

using System.Drawing;

namespace Constellation
{
    public class FactoryNode
    {
        public Point loc; 
        public Player owner; //don't set this directly --> use NewOwner() instead

        public const int numSecondsBuildUnit = 1;
        public const int TimeUntilAddUnit_MAX = 4 * numSecondsBuildUnit; // since timer ticks every .25 secs
        public int TimeUntilAddUnit = TimeUntilAddUnit_MAX;

        public int armyNumHere = 0; Game game;
        public List<Road> roadsConnected = new List<Road>();
        public List<FactoryNode> factoriesConnected = new List<FactoryNode>();
        Random r = new Random();
        
        
        public float anim; public int direction = -1;

        public FactoryNode(Point loc, Player owner, Game game)
        {
            this.loc = loc; this.owner = owner; this.game = game;
            //random flow blinking effect
            anim = r.Next(3 * 10, radius * 10) / 10;
        }

        public int radius 
        { 
            get 
            {
                if (this.owner == null) 
                    return 5;
                else
                    return Math.Max(2, (int)Math.Floor(Math.Sqrt((int)armyNumHere)));
            } 
        }
        public void IncreaseArmy()
        {
            this.TimeUntilAddUnit--;
            if (this.TimeUntilAddUnit == 0)
            {
                // reset
                this.TimeUntilAddUnit = TimeUntilAddUnit_MAX;
                this.armyNumHere++;
            }
        }
		public void SplitHalf(FactoryNode moveHere, Road r)
		{
			//default: half of forces sent
			int j = armyNumHere;
			armyNumHere -= j / 2;
			Army a = new Army(j / 2, this, owner, moveHere, r);
			
			
		}
		public void SendAll(FactoryNode moveHere, Road r)
		{
			// alternative: sends all but 1
			int j = armyNumHere;
			armyNumHere = 1;
			Army a = new Army(j - 1, this, owner, moveHere, r);
		}
        public void Join(Army a)
        {
        	a.shouldRemove = true;
        	
            if (a.owner == this.owner)
            {
                //add forces
                armyNumHere += a.num;
            }
            else
            {
                //fight by making new scram fake standing army at factory
                int initialForce= armyNumHere;
                int meLeft=a.Fight(armyNumHere);
                this.owner.dead += initialForce - Math.Max(0,meLeft);
                if (meLeft <= 0)
                {
                    this.NewOwner(a.owner); armyNumHere += a.num; //defenses lose
                }
                else
                    armyNumHere = meLeft; // defenses win
            }
        }
        //TODO: refactor this method!!!
        public void NewOwner(Player p)
        {
            //take factory start previous owner
            if (owner != null) owner.factoriesOwned.Remove(this);
            //inform this factory of its new owner
            owner = p; 
            //update status
            armyNumHere = 0; TimeUntilAddUnit = 4;
            //give factory end new owner
            p.factoriesOwned.Add(this);
        }
        
    }
}
