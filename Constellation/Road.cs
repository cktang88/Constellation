using System;
using System.Collections.Generic;
using System.Linq;

namespace Constellation
{
    public enum RoadTypes { Dirt, Gravel, Highway, Rail}
    public class Road
    {
        public List<Node> endpoints; 
        public RoadTypes rdtype;
		public List<Army> armies = new List<Army>();
		public bool Contains(Army a)
		{
			return armies.Contains(a);
		}
        
        public Road(Node a, Node b, RoadTypes rdtype)
        {
            endpoints = new List<Node>(2);
            endpoints.Add(a);
            endpoints.Add(b);
            this.rdtype = rdtype;
            travelSpeed = (int)rdtype * .4f + .4f;
        }
        public float travelSpeed;
        public bool Connects(Node start, Node end)
        {
			return endpoints.Contains(start) && endpoints.Contains(end) 
				&& start.loc != end.loc; //and not the same fac
        }
        /// <summary>
        /// use TryUpgradeRoad() first!!!!
        /// </summary>
        public void Upgrade()
        {
            //returns is upgrade successful (is the road upgradable)
            if (rdtype == RoadTypes.Highway) rdtype = RoadTypes.Rail;
            else if (rdtype == RoadTypes.Gravel) rdtype = RoadTypes.Highway;
            else if (rdtype == RoadTypes.Dirt) rdtype = RoadTypes.Gravel;

            travelSpeed = (int)rdtype * .4f+ .4f;
            
        }
        
    }
}
