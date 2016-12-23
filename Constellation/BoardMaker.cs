
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Constellation
{
	/// <summary>
	/// Description of BoardMaker.
	/// </summary>
	public static class BoardMaker
	{
		public static List<Point> MakeBoard(BoardType boardtype, Rectangle gameWorld)
		{
			Random r = new Random(DateTime.Now.Millisecond);
			List<Point> fac_LocList = new List<Point>();
			
			Point midpoint = new Point((gameWorld.X + gameWorld.Width) / 2, 
				                 (gameWorld.Y + gameWorld.Height) / 2);
			
			switch (boardtype) {
				case BoardType.Random:
					for (int i = 0; i < 40; i++) {
						bool tooClose = false;

						Point a = new Point(r.Next(gameWorld.X, gameWorld.Width),
							          r.Next(gameWorld.Y, gameWorld.Height));

					
						foreach (Point f in fac_LocList) {
							if (UTILS.DistSquared(f, a) < Math.Pow(50, 2))
								tooClose = true;
						}
						if (!tooClose) {
							fac_LocList.Add(a);
						} else {
							//if you can't place here b/c too crowded, give another chance
							//end ensure exact num of dots
							i--;
						}
					}
					break;
			
				case BoardType.HexGrid:
					const int spacing = 150;
					for (int i = gameWorld.X; i < gameWorld.Width - spacing / 2; i += spacing) {
						for (int j = gameWorld.Y; j < gameWorld.Height - spacing / 2; j += spacing) {
							fac_LocList.Add(new Point(i + r.Next(-10, 10), j + r.Next(-10, 10)));
						}
					}
				
					break;
				case BoardType.Spiral:
				//fermat spiral, a subset of archimedean spiral
                
					float radius;
					for (float i = 0; i < 3 * Math.PI; i += (float)(2 * Math.PI / 17)) {
						radius = (float)(100 * Math.Sqrt(i));
						fac_LocList.Add(
							midpoint + new Size((int)Math.Floor(radius * Math.Cos(i)),
								(int)Math.Floor(radius * Math.Sin(i))));

						fac_LocList.Add(
							midpoint + new Size((int)Math.Floor(-radius * Math.Cos(i)),
								(int)Math.Floor(-radius * Math.Sin(i))));
					}
					break;
				case BoardType.Simple:
					for (int i = gameWorld.X; i < midpoint.X; i += 150) {
						for (int j = gameWorld.Y; j < gameWorld.Y + 3 * 150 + 1; j += 150) {
							fac_LocList.Add(
								new Point(i + r.Next(-10, 10), j + r.Next(-10, 10)));
						}
					}

					break;
				case BoardType.Comb:

					for (int i = gameWorld.X; i < gameWorld.Width; i += 80) {
						int c = r.Next(1, 6);
						if (c == 4) {
							for (int j = r.Next(-(int)Math.Floor((decimal)midpoint.Y / 40), -2);
                            j < r.Next(2, (int)Math.Floor((decimal)midpoint.Y / 40)); j++) {
								fac_LocList.Add(new Point(i + r.Next(-10, 10), j * 80 + midpoint.Y));
							}
						} else
							fac_LocList.Add(new Point(i, midpoint.Y + r.Next(-10, 10)));
					}
					break;
				case BoardType.Hourglass:
					for (int i = gameWorld.X; i < gameWorld.Width; i += 80) {
						for (int j = 0; j < (int)Math.Floor((decimal)gameWorld.Height / 2); j += 80) {
                        
						}
					}
					break;
				case BoardType.Challenge8:
					for (int i = 1; i < 3; i++) {
						for (int j = 1; j < 4; j++) {
							fac_LocList.Add(new Point(i * 400 + r.Next(-10, 10), j * 200 + r.Next(-10, 10)));
						}
						fac_LocList.Add(new Point(600 + r.Next(-10, 10), i * 200 + 100 + r.Next(-10, 10)));
					}
					break;
				case BoardType.Star:
				//x-shape, 9 dots
					for (int i = 0; i < 4; i++) {
						for (int j = 1; j < 3; j++) {
							fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Cos(Math.PI / 2 * i + Math.PI / 4)),
								midpoint.Y + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Sin(Math.PI / 2 * i + Math.PI / 4))));
						}
					}
					fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10), midpoint.Y + r.Next(-10, 10)));
					break;
				case BoardType.Temple:
				//x-shape, 9 dots
					for (int i = 0; i < 4; i++) {
						for (int j = 1; j < 3; j++) {
							fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Cos(Math.PI / 2 * i + Math.PI / 4)),
								midpoint.Y + r.Next(-10, 10) + (int)Math.Floor(150 * j * Math.Sin(Math.PI / 2 * i + Math.PI / 4))));
						}
					}
					fac_LocList.Add(new Point(midpoint.X + r.Next(-10, 10), midpoint.Y + r.Next(-10, 10)));
					break;
				default:
				//default is no board
					break;
				
			}
			
			
			return fac_LocList;
		}
	}
	public enum BoardType
	{
		Random,
		HexGrid,
		Spiral,
		Simple,
		Comb,
		Hourglass,
		Star,
		Temple,
		Challenge8
	}
}
