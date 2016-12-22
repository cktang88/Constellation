
using System;

namespace Constellation
{
	/// <summary>
	/// Static helper methods for any AI
	/// </summary>
	public static class Network
	{
		/// <summary>
		/// returns the road between the two allFacs, if any
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
		/// calculates how many new units will be produced by the time help arrives
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static float NewUnits(Node start, Node end)
		{
			return Network.NumTravelTicks(start, end) * Game.MAINTICK / Game.BUILDTICK;
		}
		
	}
}
