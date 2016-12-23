using System;
using System.Collections.Generic;
using System.Linq;

namespace Constellation
{
	public enum RoadTypes
	{
		Dirt,
		Gravel,
		Highway,
		Rail

	}
	public class Road
	{
		public List<Node> endpoints{ get; private set; }
		public RoadTypes rdtype{ get; private set; }
		public List<Army> armies{ get; private set; }
		public float travelSpeed{ get; private set; }
		public bool Contains(Army a)
		{
			return armies.Contains(a);
		}
        
		public Road(Node a, Node b, RoadTypes rdtype)
		{
			armies = new List<Army>();
			
			
			endpoints = new List<Node>(2);
			endpoints.Add(a);
			endpoints.Add(b);
			this.rdtype = rdtype;
			travelSpeed = (int)rdtype * .4f + .4f;
		}
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
			if (rdtype == RoadTypes.Highway)
				rdtype = RoadTypes.Rail;
			else if (rdtype == RoadTypes.Gravel)
				rdtype = RoadTypes.Highway;
			else if (rdtype == RoadTypes.Dirt)
				rdtype = RoadTypes.Gravel;

			
			/* Optimization:
			 * calculate this only when upgrading
			 * instead of calculating on each retrieval of travel speed
			 */
			travelSpeed = (int)rdtype * .4f + .4f;
            
		}
        
	}
}
