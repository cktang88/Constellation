using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace Constellation
{
	public class ParticleEmitter
	{
		public List<Particle> particles{ get; private set; }
		public ParticleEmitter(PointF loc, Color colorA, Color colorB, int num)
		{
			particles = new List<Particle>();
        	
			if (num > 0) {
				Random r = new Random();
				int n = Math.Max(10, (int)Math.Floor(Math.Sqrt(num)));
				int scatter = (int)Math.Floor(Math.Sqrt(num) / 4) + 5;
				for (int i = 0; i < n; i++) {
					//alternate colors
					Color c = colorA;
					//if even
					if (i % 2 == 0)
						c = colorB;

					//slightly random centering and angle and size
					particles.Add(new Particle(loc + new Size(r.Next(-scatter, scatter), r.Next(-scatter, scatter)),
						r.Next(0, 360), c, (float)Math.Max(1, (float)Math.Sqrt(num) / 10) * 1f, r.Next(2, 6)));


				}
			}
		}
        
		public void MoveParticles()
		{
			foreach (Particle p in particles) {
				p.Move();
			}
			for (int i = 0; i < particles.Count; i++) {
				if (particles[i].shouldRemove) {
					particles.Remove(particles[i]);
					i--;
				}
			}
		}
	}
	public class Particle
	{
		public int lifetime = 30;
		public Color color;
		public float age = 0;
		public int radius = 1;
		Random r = new Random();
		public Particle(PointF loc, float angle_deg, Color color, float speed, int radius)
		{

			this.loc = loc;
			this.radius = radius;
			this.color = color;
			
			movement.X = speed * (float)Math.Cos(Math.PI * angle_deg / 180);
			movement.Y = speed * (float)Math.Sin(Math.PI * angle_deg / 180);
		}
		
		public PointF loc;

		PointF movement;
		public bool shouldRemove = false;
		
		public void Move()
		{
			loc.X += movement.X;
			loc.Y += movement.Y;
			age++;
			if (age >= lifetime) {
				shouldRemove = true;
			}
		}
	}
}
