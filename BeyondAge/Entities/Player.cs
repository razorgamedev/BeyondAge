using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BeyondAge.Entities
{
    class Player: Component
    {
    }

    class PlayerController : Filter
    {
        public PlayerController() : base(typeof(Player), typeof(Body), typeof(PhysicsBody))
        {
        }

        public override void Update(Entity ent, GameTime time)
        {
            var physics = ent.Get<PhysicsBody>();
            var dt = (float)time.ElapsedGameTime.TotalSeconds;

            if (GameInput.Self.KeyDown(Keys.Left))
                physics.VelX -= physics.Speed * dt;

            if (GameInput.Self.KeyDown(Keys.Right))
                physics.VelX += physics.Speed * dt;

            if (GameInput.Self.KeyDown(Keys.Up))
                physics.VelY -= physics.Speed * dt;

            if (GameInput.Self.KeyDown(Keys.Down))
                physics.VelY += physics.Speed * dt;
        }
    }
}
