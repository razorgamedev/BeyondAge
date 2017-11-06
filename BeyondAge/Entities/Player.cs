using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondAge.Entities
{
    class Player: Component
    {
        public int Sector { get; set; } = 1;
    }

    class PlayerController : Filter
    {
        Camera camera;
        public PlayerController(Camera camera) : base(typeof(Player), typeof(Body), typeof(PhysicsBody), typeof(Animation))
        {
            this.camera = camera;
        }

        public override void Update(Entity ent, GameTime time)
        {
            var physics = ent.Get<PhysicsBody>();
            var body = ent.Get<Body>();
            var player = ent.Get<Player>();

            physics.Speed = 1280;

            var dt = (float)time.ElapsedGameTime.TotalSeconds;
            
            if (GameInput.Self.KeyDown(Constants.PlayerMoveLeft))
            {
                physics.VelX -= physics.Speed * dt;
                player.Sector = 4;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveRight))
            {
                physics.VelX += physics.Speed * dt;
                player.Sector = 2;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveUp))
            {
                physics.VelY -= physics.Speed * dt;
                player.Sector = 3;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveDown))
            {
                physics.VelY += physics.Speed * dt;
                player.Sector = 1;
            }
            
            var target = new Vector2(body.X + physics.VelX * Constants.CameraPredictionScale, body.Y + physics.VelY * Constants.CameraPredictionScale);

            camera.CenterOn(body.Center);


            var facing = physics.Direction;
            
            var which = new string[] { "front", "left", "back", "right" }[player.Sector - 1];
            var sprite = ent.Get<Animation>();

            // NOTE(Dustin): This is a poor solution, try to find a way to 
            // get the maximum speed of the player.
            if (physics.Velocity == Vector2.Zero)
                sprite.TimerScale = 0;
            else
                sprite.TimerScale = physics.Velocity.Length() * 0.005f;
            
            sprite.CurrentAnimationID = which;
        }

        public override void UiDraw(Entity ent, SpriteBatch batch)
        {
            var health = ent.Get<Status>().Health;
            var icons = BeyondAge.Assets.GetTexture("icons");
            
            // Draw the UI
            for (int i = 0; i < health; i++)
            {
                int s = 12 * (int)Constants.SCALE / 2;
                batch.Draw(icons, new Rectangle((i * (s + 8) + 16), 16, s, s), new Rectangle(0, 0, 12, 12), Color.White);
            }
        }
    }
}
