using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Graphics
{
    class Primitives
    {
        private Texture2D pixel;
        private SpriteBatch batch;

        public int LinePixelWidth { get; set; } = 3;

        public Primitives(GraphicsDevice device, SpriteBatch batch)
        {
            pixel = new Texture2D(device, 1, 1);

            Color[] data = new Color[1];
            data[0] = Color.White;
            pixel.SetData(data);

            this.batch = batch;
        }

        public void DrawRect(Rectangle rect, Color color)
        {
            batch.Draw(pixel, rect, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        public void DrawLineRect(Rectangle rect, Color color)
        {
            var left = new Rectangle(rect.X, rect.Y, LinePixelWidth, rect.Height);
            var right = new Rectangle(rect.X + rect.Width - LinePixelWidth, rect.Y, LinePixelWidth, rect.Height);
            var top = new Rectangle(rect.X, rect.Y, rect.Width, LinePixelWidth);
            var bottom = new Rectangle(rect.X, rect.Y + rect.Height - LinePixelWidth, rect.Width, LinePixelWidth);

            batch.Draw(pixel, left, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            batch.Draw(pixel, right, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            batch.Draw(pixel, top, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
            batch.Draw(pixel, bottom, null, color, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            var dist = Vector2.Distance(start,end);

            var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            var rectangle = new Rectangle
            {
                X = (int)start.X,
                Y = (int)start.Y,
                Width = (int)dist,
                Height = LinePixelWidth
            };

            batch.Draw(
                pixel, 
                rectangle, 
                new Rectangle(0, 0, 1, 1), 
                color, 
                angle, 
                new Vector2(0, 0), 
                SpriteEffects.None, 
                1f);
        }
    }
}
