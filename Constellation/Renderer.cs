using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Constellation
{
    public class Renderer
    {
        Theme theme; Game game; bool showStats;

        String fontStyle = "Microsoft Sans Serif";

        public Renderer(Game game, Theme theme, bool showStats = false)
        {
            this.theme = theme; this.game = game; this.showStats = showStats;
        }
        public void DrawParticles(ParticleEmitter partEmit, Graphics g)
        {
            //draws all particles start particleEmitters
            foreach (Particle p in partEmit.particles)
            {
                var col = Color.FromArgb(Math.Max(255 - Convert.ToInt32(p.age / p.lifetime * 255), 0), p.color);

                //random size particles
                Brush b = new SolidBrush(col);
                g.FillEllipse(b, Convert.ToSingle(p.loc.X - 1),
                    Convert.ToSingle(p.loc.Y - 1), p.radius, p.radius);
            }

        }
        public void UpdateInfo(bool showStats, Theme theme)
        {
            this.showStats = showStats; this.theme = theme;
        }
        public void Render(Graphics g)
        {
            //draws EVERYTHING

            //===================== temp vars
            List<Player> players = game.players;
			int gameXright = game.gameWorld.Width + game.gameWorld.X;
            //=====================


            foreach (ParticleEmitter pe in game.particleEmitters)
            {
                DrawParticles(pe, g);
            }

            //if (mouseOverThisFactory != null)
            //{
            //    var font = new Font(fontStyle, 28F);
            //    g.DrawString(mouseOverThisFactory.armyNumHere.ToString(), font,
            //        new SolidBrush(mouseOverThisFactory.owner.color),
            //        new Point(mouseOverThisFactory.loc.X - 10,
            //            mouseOverThisFactory.loc.Y - 34 - mouseOverThisFactory.radius));
            //}

            if (showStats)
            {

                Color c = Color.Black;
                if (theme == Theme.dark) c = Color.White;

                foreach (Player p in game.players)
                {
                    Brush b = new SolidBrush(Color.FromArgb(128, p.color));

                    var font = new Font(fontStyle, 16F);
                    g.DrawString(p.numFactories + " *", font, new SolidBrush(p.color),
                        new Point(gameXright + 20, 120 * players.IndexOf(p) + 30));
                    //total strength------------
                    int strength = 0;
                    foreach (FactoryNode f in p.factoriesOwned) strength += f.armyNumHere;
                    foreach (Army a in p.armies) strength += a.num;

                    g.DrawString("+ " + strength, new Font(fontStyle, 20F), new SolidBrush(p.color),
                        new Point(gameXright + 20, 120 * players.IndexOf(p) + 60));
                    g.DrawString("- " + p.dead, new Font(fontStyle, 12F), new SolidBrush(p.color),
                        new Point(gameXright + 20, 120 * players.IndexOf(p) + 90));
                }

                //total time elapsed
                int seconds = (int)Math.Floor((decimal)game.totalTime / 1000);
                int minutes = (int)Math.Floor((decimal)seconds / 60);
                seconds -= 60 * minutes; string space1 = ""; string space2 = "";
                if (minutes < 10) space1 = "0";
                if (seconds < 10) space2 = "0";
                g.DrawString(space1 + minutes + " : " + space2 + seconds, new Font(fontStyle, 22F), new SolidBrush(c),
                        new Point(gameXright + 20, players.Count * 120 + 60));

            }
            //---------------------------------------------this has a layering effect...
            foreach (Player p in players)
            {
                foreach (Army a in p.armies)
                {
                    Draw(a, g);
                }
            }
            foreach (FactoryNode f in game.factorynodes)
            {
                Draw(f, g);
            }
            if(game.target!=null)
                DrawTarget(game.target, g); // draw where the fleet will be sent
            foreach (Road r in game.roads)
            {
                Draw(r, g);
            }

        }
        public void DrawTarget(FactoryNode facNode, Graphics g)
        {
            int radius = facNode.radius + 10;
            Point loc = facNode.loc;
            Color c;
            if (facNode.owner==null || facNode.owner.color !=Color.Lime) c = Color.Lime;
            else c = Color.Cyan;
            g.DrawEllipse(new Pen(new SolidBrush(c),3), loc.X - facNode.anim, loc.Y - facNode.anim,
                    2 * facNode.anim, 2 * facNode.anim);
        }
        public void Draw(FactoryNode facNode, Graphics g)
        {
            //draw factory nodes

            //===================== temp vars
            int radius = facNode.radius;
            Point loc = facNode.loc;
            Player owner = facNode.owner;
            //=====================

            if (owner == null)
            {
                //can't use temp var for anim or direction, because anim must UPDATE!!!
                if (facNode.anim <= 2) facNode.direction = 1;
                else if (facNode.anim >= radius) facNode.direction = -1;
                facNode.anim += facNode.direction * .2f;
                Color c = Color.Black;

                if (theme == Theme.dark) c = Color.White;
                g.DrawEllipse(new Pen(new SolidBrush(c), 3), loc.X - facNode.anim, loc.Y - facNode.anim,
                    2 * facNode.anim, 2 * facNode.anim);
            }

            else
            {
                g.DrawEllipse(new Pen(new SolidBrush(owner.color),3), loc.X - radius/2, loc.Y - radius/2,
                    radius, radius);

                g.DrawEllipse(new Pen(new SolidBrush(Color.FromArgb(32, owner.color)), 11),
                    loc.X - radius/2, loc.Y - radius/2,
                    radius, radius);
                 

                //eventually, remove numbers all together!!
                var font = new Font(fontStyle, 15F);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                RectangleF rect = new RectangleF(loc.X - 100, loc.Y - radius - 25, 200, 25);
                g.DrawString(facNode.armyNumHere.ToString(), font, new SolidBrush(owner.color),
                    rect, stringFormat);
            }
        }
        public void Draw(Road road, Graphics g)
        {
            //draw road
            Color c = Color.Black;
            if (theme == Theme.dark) c = Color.White;

            float[] dashValues = { 2 * (int)road.rdtype + 1, 2 };
            Pen p = new Pen(c, (int)road.rdtype);
            p.DashPattern = dashValues;

            g.DrawLine(p, road.endpoints[0].loc, road.endpoints[1].loc);
        }
        public void Draw(Army army, Graphics g)
        {
            //draw army

            //===================== temporary vars
            PointF loc_true = army.loc_true;
            int radius = army.radius;
            int num = army.num;
            //=====================


            var font = new Font(fontStyle, 10f, FontStyle.Bold);
            //largest power end two still smaller than the number
            int largestPowerOfTwo = Math.Max(0,(int)Math.Floor(Math.Log(num) / Math.Log(2)));
            Color c= Color.FromArgb(Math.Min(25*largestPowerOfTwo,250), army.owner.color);

            Color highlight = Color.Black;
            if (theme == Theme.dark) highlight = Color.White;
            highlight = Color.FromArgb(Math.Min(25 * largestPowerOfTwo, 250), highlight);



            RectangleF rect = new RectangleF(loc_true.X - radius, loc_true.Y - radius, 2 * radius, 2 * radius);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //=== shows how many people in the army
            //g.DrawString(num.ToString(), font, new SolidBrush(Color.White),rect, stringFormat);


            //experimental arrow

            //clockwise
            PointF[] pts= new PointF[4];
            pts[0]=new PointF(loc_true.X - radius, loc_true.Y - radius);
            pts[1] = new PointF(loc_true.X, loc_true.Y + radius);
            pts[2]=new PointF(loc_true.X + radius, loc_true.Y - radius);           
            pts[3]=new PointF(loc_true.X, loc_true.Y - radius/2);
            //...
            Rotate(pts, loc_true, (float)army.angle);
            g.DrawPolygon(new Pen(new SolidBrush(army.owner.color),3), pts);

        }


        //
        //=========== helper methods ===============
        //

        //end rotate sets of points
        void Rotate(PointF[] points, PointF center, float degreesCounterClockwise)
        {
            float ang = degreesCounterClockwise;
            
            for (int i = 0; i < points.Count(); i++)
            {
                float ang_orig = (float)Math.Atan2(points[i].Y - center.Y, points[i].X - center.X)-(float)Math.PI/2;
                points[i] = new PointF(
                    UTILS.Distance(points[i], center)*(float)Math.Cos(ang+ang_orig)+center.X,
                    UTILS.Distance(points[i], center) * (float)Math.Sin(ang+ang_orig) + center.Y);
            }
        }
        
    }
}
