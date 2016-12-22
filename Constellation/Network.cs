
using System;
using System.Collections.Generic;

namespace Constellation
{
	/// <summary>
	/// Collection of general methods useful for AIs
	/// </summary>
	public static class Network
	{
		/// <summary>
		/// Returns the road between two facs, if any
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static Road RoadBetween(Node start, Node end)
		{
			foreach (Road r in start.roadsConnected) {
				if (r.endpoints.Contains(end))
					return r;
			}
			return null;
		}
		
		/// <summary>
		/// Returns the number of ticks it will take to travel between two nodes
		/// Assumes road is available
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static float NumTravelTicks(Node start, Node end)
		{
			//so if used in reinforcement senses, no reinforcements will come
			if (Network.RoadBetween(start, end) != null)
				return UTILS.Distance(start.loc, end.loc) / Network.RoadBetween(start, end).travelSpeed;
			else {
				//imagine road was built, calculate odds --> will build later if good choice?
				Road r = new Road(start, end, RoadTypes.Dirt);
				return UTILS.Distance(start.loc, end.loc) / r.travelSpeed;
			}
		}
		
		
		/// <summary>
		/// finds the first Node that is connected to both start & end, if any
		/// </summary>
		/// <returns></returns>
		public static Node GetIntermediate(Player p, Node start, Node end, NodeOwner f_owner)
		{
			Node intermediate = null;
			foreach (Node mid in start.nodesConnected) {
				//if NodeOwner.anyone, then always passes
				if (f_owner == NodeOwner.me && mid.owner != p ||
				    f_owner == NodeOwner.enemy && (mid.owner == p || mid.owner == null) ||
				    f_owner == NodeOwner.noone && mid.owner != p ||
				    f_owner == NodeOwner.notMe && mid.owner == p)
					continue;
				
				if (mid.nodesConnected.Contains(end)
					//and if not counterproductive
				    && .8 * UTILS.DistSquared(mid.loc, end.loc)
				    < UTILS.DistSquared(start.loc, end.loc))
					intermediate = mid;
			}
			return intermediate;
		}
		
		/// <summary>
		/// Calculates how many new units will be made during travel time of two nodes
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static int NewUnits(Node start, Node end)
		{
			return (int)Math.Floor(Network.NumTravelTicks(start, end) * Game.MAINTICK / Game.BUILDTICK);
		}
		
		/// <summary>
		/// finds the closest factory of a certain owner or group of owners from ANY one of
		/// my own Game.factorynodes
		/// </summary>
		/// <param name="f_owner"></param>
		/// <param name="mustHaveRoad"></param>
		/// <returns></returns>
		public static Node ClosestNodeOf(Player p, NodeOwner f_owner, RoadExistence mustHaveRoad)
		{
			float d = 2000 * 2000;
			Node closestFac = null;

			foreach (Node myFac in p.nodesOwned) {
				foreach (Node f in Game.factorynodes) {
					if (mustHaveRoad == RoadExistence.yes && Network.RoadBetween(myFac, f) != null
					    || mustHaveRoad == RoadExistence.no && Network.RoadBetween(myFac, f) == null
					    || mustHaveRoad == RoadExistence.any) {
						
						if (f_owner == NodeOwner.me && f.owner == p ||
						    f_owner == NodeOwner.enemy && f.owner != p && f.owner != null ||
						    f_owner == NodeOwner.noone && f.owner == null ||
						    f_owner == NodeOwner.anyone ||
						    f_owner == NodeOwner.notMe && f.owner != p) {
							
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
		
		
		public static Node ClosestNodeTo(Player p, Node fromThis, RoadExistence mustHaveRoad, NodeOwner f_owner)
		{
			//slight optimization
			List<Node> f_temp;
			if (mustHaveRoad == RoadExistence.yes) {
				f_temp = UTILS.GetClosestList(fromThis.loc, fromThis.nodesConnected);
			} else
				f_temp = UTILS.GetClosestList(fromThis.loc, Game.factorynodes);
			
			foreach (Node f in f_temp) {
				if (f == fromThis) //closest fac can't be itself
					continue;
				if (f_owner == NodeOwner.me && f.owner == p ||
				    f_owner == NodeOwner.enemy && f.owner != p && f.owner != null ||
				    f_owner == NodeOwner.noone && f.owner == p ||
				    f_owner == NodeOwner.anyone ||
				    f_owner == NodeOwner.notMe && f.owner != p) {
					
					if (mustHaveRoad == RoadExistence.no && Network.RoadBetween(fromThis, f) == null
					    || mustHaveRoad == RoadExistence.any) {
										
						return f;
					}
				}
			}
			return null;
		}
		
		
		/// <summary>
		/// Returns net incoming armies less than "maxdist" away
		/// Note: friendlies & enemies balance out
		/// Note: may be negative
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static int NetNearbyIncoming(Player p, Node f, int maxdist = 300)
		{
			int attacker_pop = 0;
			foreach (Road r in f.roadsConnected) {
				foreach (Army a in r.armies) {
					if (a.target != f.loc)
						continue;
					if (UTILS.DistSquared(a.loc, f.loc) > maxdist * maxdist) //only counts nearby things
						continue;
					if (a.owner == p)
						attacker_pop += a.num;
					else
						attacker_pop -= a.num;
					
				}
			}
			return attacker_pop;
		}
		
	}
	
	public enum NodeOwner
	{
		me,
		enemy,
		noone,
		anyone,
		notMe
	}
	public enum RoadExistence
	{
		yes,
		no,
		any
	}
}
