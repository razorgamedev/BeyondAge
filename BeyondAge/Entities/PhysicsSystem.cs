using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BeyondAge.Graphics;

namespace BeyondAge.Entities
{
    class PhysicsSystem : Filter
    {
        private List<Entity> physicsBodies = null;
        private Primitives primitives;

        public PhysicsSystem(Primitives primitives) : base(typeof(Body), typeof(PhysicsBody))
        {
            this.primitives = primitives;
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

            // Decelerate
            // TODO: Use delta time 
            physics.Velocity *= physics.Deceleration;

            body.X = body_x.X;
            body.Y = body_y.Y;
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
            }
        }
    }
}
