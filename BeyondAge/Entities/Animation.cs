using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeyondAge.Entities
{
    class Frame
    {
        private int height;
        private int x;
        private int y;
        private int width;
        private float time = 0.1f;

        public int X       { get => x;         set => x = value; }
        public int Y       { get => y;         set => y = value; }
        public int Width   { get => width;     set => width = value; }
        public int Height  { get => height;    set => height = value; }
        public float Time  { get => time;      set => time = value; }

        public Rectangle Rect { get => new Rectangle(X, Y, Width, Height); }
    }

    class Animation : Sprite
    {
        public Dictionary<string, List<Frame>> Data { get; set; }
        
        public enum PlaybackState
        {
            Playing,
            Paused,
            Reversed
        }

        public PlaybackState State { get; set; } = PlaybackState.Paused;

        public int FrameIndex { get; set; } = 0;
        public float Timer { get; set; } = 0f;
        public float TimerScale { get; set; } = 0f;
        public string CurrentAnimationID { get; set; } = "";
        
        public List<Frame> CurrentAnimation { get => Data[CurrentAnimationID]; }
        public Frame CurrentFrame { get => CurrentAnimation[FrameIndex]; }
        public int NumberOfFrames { get => CurrentAnimation.Count(); }

        public Animation(Texture2D texture, Dictionary<string, List<Frame>> Data) : base(texture, Rectangle.Empty)
        {
            this.Data = Data;
            this.Scale = 1;
            this.ScaleX = 1;
            this.ScaleY = 1;

            if (this.Data.Keys.Count > 0)
                this.CurrentAnimationID = this.Data.Keys.First();
        }
    }

    class AnimationSystem : Filter
    {
        public AnimationSystem() : base(typeof(Body), typeof(Animation))
        {
        }

        public override void Update(Entity ent, GameTime time)
        {
            var sprite = ent.Get<Animation>();

            sprite.Timer += (float)time.ElapsedGameTime.TotalSeconds * sprite.TimerScale;
            if (sprite.Timer > sprite.CurrentFrame.Time)
            {
                sprite.Timer = 0;
                sprite.FrameIndex++;
            }

            if (sprite.FrameIndex >= sprite.NumberOfFrames)
            {
                sprite.FrameIndex = 0;
            }
        }

        public override void Draw(Entity ent, SpriteBatch batch)
        {
            var sprite = ent.Get<Animation>();
            var body = ent.Get<Body>();

            float layer = 0.3f + ((body.Y + body.Size.Y) / (32 * Constants.MapSize)) * 0.1f;
            sprite.DrawLayer = layer;

            batch.Draw(
               sprite.Texture,
               new Rectangle(
                   (int)(body.X + sprite.OffsetX),
                   (int)(body.Y + sprite.OffsetY - (sprite.CurrentFrame.Rect.Height * Constants.SCALE * sprite.ScaleY) + body.Height),
                   (int)(sprite.CurrentFrame.Rect.Width * Constants.SCALE * sprite.ScaleX),
                   (int)(sprite.CurrentFrame.Rect.Height * Constants.SCALE * sprite.ScaleY)),
               sprite.CurrentFrame.Rect,
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
    }
}
