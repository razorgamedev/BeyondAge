using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BeyondAge.Entities
{
    class SpriteRenderer : Filter
    {
        public SpriteRenderer() : base(typeof(Body), typeof(Sprite))
        {
        }

        public override void Load(Entity ent)
        {
        }

        public override void Draw(Entity ent, SpriteBatch batch)
        {
            var sprite = ent.Get<Sprite>();
            var body = ent.Get<Body>();

            batch.Draw(
               sprite.Texture,
               new Rectangle((int)(body.X + sprite.OffsetX), (int)(body.Y + sprite.OffsetY), (int)(body.Width), (int)(body.Height)),
               sprite.Region,
               sprite.Color,
               0,
               Vector2.Zero,
               SpriteEffects.None,
               0
            );
        }
    }
}
