﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BeyondAge.Graphics;

namespace BeyondAge.Entities
{
    class Polygon: Body
    {
        public List<Vector2> Points { get; set; } = new List<Vector2>();
    }

    // TODO(Dustin): We need to have the sensor be a child of Solid
    class Solid : Body {
        public bool Is_Sensor { get; set; } = false;
        public string SensorString { get; set; } = "";
        public Vector2 SensorLocation { get; set; } = Vector2.Zero;
        public Action<Solid> Callback { get; set; }
    }

    class Line
    {
        public Vector2 Start, End;
    }

    class Polygon_Collision_Result
    {
        public bool Will_Intersect { get; set; } = false;
        public bool Intersect { get; set; } = false;
        public Vector2 Minimum_Translation_Vector { get; set; } = Vector2.Zero;
    }

    class PhysicsSystem : Filter
    {
        private List<Solid> solids;
        private List<Polygon> polygons;

        private List<Entity> physicsBodies = null;
        private Primitives primitives;

        public PhysicsSystem(Primitives primitives) : base(typeof(Body), typeof(PhysicsBody))
        {
            this.primitives = primitives;
            this.solids     = new List<Solid>();
            this.polygons   = new List<Polygon>();
        }

        public void ClearSolids()
        {
            solids.Clear();
            polygons.Clear();
        }

        public void Project_Polygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            float dot = Vector2.Dot(axis, polygon.Points[0]);
            min = dot;
            max = dot;
            foreach (var v in polygon.Points)
            {
                dot = Vector2.Dot(v, axis);
                if (dot < min) min = dot;
                else if (dot > max) max = dot;
            }
        }

        public float Interval_Distance(float min_a, float max_a, float min_b, float max_b)
        {
            if (max_a < min_b) return min_a - max_a;
            else return min_a - max_b;
        }

        public bool Line_Intersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            Vector2 intersection = Vector2.Zero;
            Vector2 b = Vector2.Subtract(a2, a1);
            Vector2 d = Vector2.Subtract(b2, b1);
            float b_dot_perp = b.X * d.Y - b.Y * d.X;
            if (b_dot_perp == 0) return false;

            Vector2 c = Vector2.Subtract(b1, a1);
            float t = (c.X * d.Y - c.Y * d.X) / b_dot_perp;
            if (t < 0 || t > 1) return false;

            float u = (c.X * b.Y - c.Y * b.X) / b_dot_perp;
            if (u < 0 || u > 1) return false;

            intersection = a1 + Vector2.Multiply(b, t);
            return true;
        }

        public bool Body_In_Polygon(Polygon poly, Body body)
        {
            // Get the lines from the body
            Line left_wall = new Line
            {
                Start = body.Position,
                End = new Vector2(body.X, body.Y + body.Height)
            };

            Line bottom_wall = new Line
            {
                Start = new Vector2(body.X, body.Y + body.Height),
                End = new Vector2(body.X + body.Width, body.Y + body.Height)
            };

            Line right_wall = new Line
            {
                Start = new Vector2(body.X + body.Width, body.Y),
                End = new Vector2(body.X + body.Width, body.Y + body.Height)
            };

            Line top_wall = new Line
            {
                Start = body.Position,
                End = new Vector2(body.X + body.Width, body.Y)
            };

            // Calculate the intersections
            var end = poly.Points[0] + poly.Position;
            for (int i = 1; i < poly.Points.Count; i++)
            {
                var start = poly.Points[i] + poly.Position;

                if (Line_Intersection(left_wall.Start, left_wall.End, start, end) ||
                    Line_Intersection(bottom_wall.Start, bottom_wall.End, start, end) ||
                    Line_Intersection(right_wall.Start, right_wall.End, start, end) ||
                    Line_Intersection(top_wall.Start, top_wall.End, start, end))
                {
                    return true;
                }

                end = start;
            }
            
            return false;
        }

        public Polygon AddPolygon(Polygon p) {
            polygons.Add(p);
            return p;
        }

        public Solid AddSolid(Solid solid) {
            solids.Add(solid);
            return solid;
        }

        public override void PreUpdate(GameTime time)
        {
            if (WorldRef == null) return;
            physicsBodies = WorldRef.GetAllWithComponent(typeof(PhysicsBody));
        }

        public override void Update(Entity ent, GameTime time)
        {
            if (physicsBodies == null) return;

            var physics = ent.Get<PhysicsBody>();
            var body = ent.Get<Body>();
            var dt = (float)time.ElapsedGameTime.TotalSeconds;

            physics.Direction = (float)Math.Atan2(body.Y - (body.Y + physics.VelY), body.X - (body.X + physics.VelX)) + (180 * (float)(Math.PI / 180));
            //Console.WriteLine(physics.Direction * (180 / Math.PI));

            var body_x = new Body { X = body.X + physics.VelX * dt, Y = body.Y, Width = body.Width, Height = body.Height };
            var body_y = new Body { Y = body.Y + physics.VelY * dt, X = body.X, Width = body.Width, Height = body.Height };

            foreach(var other in physicsBodies)
            {
                if (other == ent) continue;

                var other_body = other.Get<Body>();
                if (body_x.Contains(other_body)) body_x = body;
                if (body_y.Contains(other_body)) body_y = body;

            }

            bool collides = false;
            bool sensorHandlesPosition = false;
            for (int i = 0; i < solids.Count; i++)
            {
                var solid = solids[i];
                
                if (solid.Contains(body_x)) {
                    if (!solid.Is_Sensor)
                        body_x = body;
                    else sensorHandlesPosition = true;
                    
                    collides = true;
                }
                    
                if (solid.Contains(body_y)) {
                    if (!solid.Is_Sensor)
                        body_y = body;
                    else sensorHandlesPosition = true;

                    collides = true;
                }

                if (ent.Has(typeof(Player)) && collides && solid.Is_Sensor)
                {
                    solid.Callback?.Invoke(solid);
                    if (solids.Count == 0)
                        break;
                }
                
            }

            foreach (var poly in polygons)
            {
                if (Body_In_Polygon(poly, body_x)) body_x = body;
                if (Body_In_Polygon(poly, body_y)) body_y = body;
            }

            // Decelerate
            // TODO: Use delta time 
            physics.Velocity *= physics.Deceleration;

            if (!sensorHandlesPosition)
            {
                body.X = body_x.X;
                body.Y = body_y.Y;
            }
        }

        public override void Draw(Entity ent, SpriteBatch batch)
        {

            // Debug Drawing
            if (BeyondAge.TheGame.Debugging)
            {
                var rect = ent.Get<Body>();
                var physics = ent.Get<PhysicsBody>();

                primitives.DrawLineRect(rect.Region, Color.Red);
                
                var direction = (new Vector2((float)Math.Cos(physics.Direction), (float)Math.Sin(physics.Direction)));
                primitives.DrawLine(
                    rect.Center, 
                    rect.Center + direction * 100,
                    Color.LightPink
                    );

                var font = BeyondAge.Assets.GetFont("Font");
                batch.DrawString(
                    font,
                    $"X: {rect.X} Y: {rect.Y}",
                    rect.Position + new Vector2(0, rect.Height + 4),
                    Color.White,
                    0,
                    Vector2.Zero,
                    0.5f,
                    SpriteEffects.None,
                    1);

                foreach (var poly in polygons)
                {
                    var last = poly.Points[0];
                    for (int i = 1; i < poly.Points.Count; i++)
                    {
                        var now = poly.Points[i];
                        primitives.DrawLine(last + poly.Position, now + poly.Position, Color.Red);
                        last = now;
                    }
                }

                foreach (var body in solids)
                {
                    primitives.DrawLineRect(body.Region, Color.Red);
                }
            }
        }
    }
}
