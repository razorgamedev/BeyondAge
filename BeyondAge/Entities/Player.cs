using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BeyondAge.Graphics;

namespace BeyondAge.Entities
{
    class Player: Component
    {
    }

    class PlayerController : Filter
    {
        Camera camera;
        public PlayerController(Camera camera) : base(typeof(Player), typeof(Body), typeof(PhysicsBody))
        {
            this.camera = camera;
        }

        public override void Update(Entity ent, GameTime time)
        {
            var physics = ent.Get<PhysicsBody>();
            var body = ent.Get<Body>();

            var dt = (float)time.ElapsedGameTime.TotalSeconds;

            if (GameInput.Self.KeyDown(Constants.PlayerMoveLeft))
                physics.VelX -= physics.Speed * dt;

            if (GameInput.Self.KeyDown(Constants.PlayerMoveRight))
                physics.VelX += physics.Speed * dt;

            if (GameInput.Self.KeyDown(Constants.PlayerMoveUp))
                physics.VelY -= physics.Speed * dt;

            if (GameInput.Self.KeyDown(Constants.PlayerMoveDown))
                physics.VelY += physics.Speed * dt;
            
            //camera.Position = body.Position - new Vector2(BeyondAge.Width / 2 - body.Width / 2, BeyondAge.Height / 2 - body.Height / 2);

            var target = new Vector2(body.X + physics.VelX * Constants.CameraPredictionScale, body.Y + physics.VelY * 1.5f);

            var dx = -BeyondAge.Width / 2  + body.Width  / 2 + target.X - camera.Position.X;
            var dy = -BeyondAge.Height / 2 + body.Height / 2 + target.Y - camera.Position.Y;
            camera.MoveCamera(new Vector2(dx * Constants.CameraSmoothValue, dy * Constants.CameraSmoothValue));

        }
    }
}
