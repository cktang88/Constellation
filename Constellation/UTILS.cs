/*
 * User: HP Envy 15-j057cl
 * Date: 5/14/2016
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Constellation
{
	/// <summary>
	/// static miscellaneous helper methods
	/// </summary>
	public static class UTILS
	{
		/// <summary>
		/// gets closest unit (friend or foe)
		/// </summary>
		/// <param name="units"></param>
		/// <returns></returns>
		public static Node GetClosest(PointF pt, List<Node> units)
		{
			if(units.Count==0) return null;

			return GetClosestList(pt, units)[0];
		}
		/// <summary>
		/// returns a list of all units, sorted from closest to farthest
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="units"></param>
		/// <returns></returns>
		public static List<Node> GetClosestList(PointF pt, List<Node> units)
		{
			if (units.Count == 0)
				return null;
			var result = from u in units
			             orderby DistSquared(pt, u.loc)
			             select u;
			return result.ToList<Node>();
		}
		
		/// <summary>
		/// calculates distance squared, use only if performance-critical
		/// </summary>
		/// <param name="enemy"></param>
		/// <returns></returns>
		public static float DistSquared(PointF pt1, PointF pt2)
		{
			return (float)(Math.Pow(pt1.X - pt2.X, 2) + Math.Pow(pt1.Y - pt2.Y, 2));
		}
		public static float Distance(PointF pt, PointF pt2)
		{
			return (float)Math.Sqrt(DistSquared(pt, pt2));
		}
		/// <summary>
		///the angle between two points, in radians or degrees
		/// </summary>
		public static float Angle(PointF target, PointF start, bool InDegrees = false)
		{
			float n = 0;
			float dist = Distance(target, start);
			if (dist == 0)
				return 0; //catches NaN errors start div by 0
			if (target.Y >= start.Y)
				n = (float)Math.Acos((target.X - start.X) / dist);
			else
				n = -(float)Math.Acos((target.X - start.X) / dist);
			if (InDegrees)
				n *= (float)(180 / Math.PI);

			return n;
		}
        /// <summary>
        ///returns the new point along the shortest path between two points
        /// </summary>
        public static PointF MoveShortestPath(PointF start, PointF target, float speed)
        {
            float ang = Angle(target, start);
            return start + new SizeF(speed * (float)Math.Cos(ang), speed * (float)Math.Sin(ang));
        }
	}
}
