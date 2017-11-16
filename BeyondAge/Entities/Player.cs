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
        public float swingTimer { get; set; } = 0;
        public bool Swinging { get; set; } = false;
        public Vector2 SwingTarget { get; set; } = Vector2.Zero;
        public Vector2 SwingStart { get; set; } = Vector2.Zero;
    }

    class PlayerController : Filter
    {
        Camera camera;
        Primitives primitives;
        
        public PlayerController(Camera camera, Primitives primitives) : base(typeof(Player), typeof(Body), typeof(PhysicsBody), typeof(Animation))
        {
            this.primitives = primitives;
            this.camera = camera;
        }

        public override void Update(Entity ent, GameTime time)
        {
            var physics = ent.Get<PhysicsBody>();
            var body = ent.Get<Body>();
            var player = ent.Get<Player>();

            physics.Speed = 1280 * 2;

            if (BeyondAge.TheGame.Debugging)
                physics.Speed *= 8;

            var dt = (float)time.ElapsedGameTime.TotalSeconds;
            
            if (GameInput.Self.KeyDown(Constants.PlayerMoveLeft))
            {
                physics.VelX -= physics.Speed * dt;
                physics.Sector = 4;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveRight))
            {
                physics.VelX += physics.Speed * dt;
                physics.Sector = 2;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveUp))
            {
                physics.VelY -= physics.Speed * dt;
                physics.Sector = 3;
            }

            if (GameInput.Self.KeyDown(Constants.PlayerMoveDown))
            {
                physics.VelY += physics.Speed * dt;
                physics.Sector = 1;
            }
            
            var target = new Vector2(body.X + physics.VelX * Constants.CameraPredictionScale, body.Y + physics.VelY * Constants.CameraPredictionScale);

            camera.CenterOn(body.Center);
            
            var facing = physics.Direction;
            
            var which = new string[] { "front", "left", "back", "right" }[physics.Sector - 1];
            var sprite = ent.Get<Animation>();

            // NOTE(Dustin): This is a poor solution, try to find a way to 
            // get the maximum speed of the player.
            if (physics.Velocity == Vector2.Zero)
                sprite.TimerScale = 0;
            else
                sprite.TimerScale = physics.Velocity.Length() * 0.0025f;
            
            sprite.CurrentAnimationID = which;


            // Sword swing
            //player.swingTimer += (float)(time.ElapsedGameTime.TotalSeconds) * 2;
            //if (player.swingTimer > )
            var dir = physics.Sector;
            if (physics.Sector == 1) dir = 3;
            if (physics.Sector == 3) dir = 1;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var swingSpeed = 8;
                var timer = time.TotalGameTime.TotalSeconds * swingSpeed;
                player.swingTimer = (float)((-Math.PI / 4) + Math.Cos(timer) + (Math.PI / 4)) + (float)(Math.PI / 2) * (((dir - 1) % 4) - 1);
            }
        }

        public override void Draw(Entity ent, SpriteBatch batch)
        {
            var physics = ent.Get<PhysicsBody>();
            var body = ent.Get<Body>();
            var player = ent.Get<Player>();
            var sprite = ent.Get<Animation>();

            var swordLen = 48;

            var items = BeyondAge.Assets.GetTexture("items");
            
            var end = new Vector2(
                    body.Center.X + (float)Math.Cos(player.swingTimer) * swordLen,
                    body.Y + (sprite.CurrentFrame.Rect.Height / 2) + (float)Math.Sin(player.swingTimer) * swordLen);
            var start = new Vector2(
                    body.Center.X,
                    body.Y + sprite.CurrentFrame.Rect.Height / 2);

            var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                batch.Draw(
                items,
                start,
                new Rectangle(0, 0, swordLen, 6),
                Color.White,
                angle,
                new Vector2(0, 6 / 2f),
                Constants.SCALE / 2,
                SpriteEffects.None,
                1
                );
            }
            //primitives.DrawLine(start, end, Color.Aquamarine);
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
