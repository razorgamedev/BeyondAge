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
        public static float MapHeight { get; set; } = 32 * Constants.MapSize;

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

            float layer = 0.3f + ((body.Y + body.Size.Y) / MapHeight) * 0.1f;
            sprite.DrawLayer = layer;

            batch.Draw(
               sprite.Texture,
               new Rectangle(
                   (int)(body.X + sprite.OffsetX), 
                   (int)(body.Y + sprite.OffsetY - (sprite.Region.Height * Constants.SCALE * sprite.ScaleY) + body.Height), 
                   (int)(sprite.Region.Width * Constants.SCALE * sprite.ScaleX), 
                   (int)(sprite.Region.Height * Constants.SCALE * sprite.ScaleY)),
               sprite.Region,
               sprite.Color,
               0,
               Vector2.Zero,
               SpriteEffects.None,
               layer
            );

            if (BeyondAge.TheGame.Debugging)
            {
                var font = BeyondAge.Assets.GetFont("Font");
                batch.DrawString(
                    font,
                    layer.ToString(),
                    body.Position + new Vector2(body.Width + 4, body.Height / 2),
                    Color.White,
                    0,
                    Vector2.Zero,
                    0.5f,
                    SpriteEffects.None,
                    1
                    );
            }
        }

        public override void DebugDraw(Entity ent, SpriteBatch batch)
        {

        }
    }
}
