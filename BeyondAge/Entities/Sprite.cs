using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class Sprite: Component
    {
        public Texture2D Texture    { get; set; }
        public Rectangle Region     { get; set; }
        public Color Color          { get; set; } = Color.White;

        public float OffsetY        { get; set; } = 0;
        public float OffsetX        { get; set; } = 0;

        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;

        public float Scale { set { ScaleX = value; ScaleY = value; } }
        public float Layer { get; set; } = 0f;

        public Vector2 Offset { 
            get => new Vector2(OffsetX, OffsetY); 
            set {
                OffsetX = value.X;
                OffsetY = value.Y;
            }
        }

        public Sprite(Texture2D texture, Rectangle region) {
            this.Texture = texture;
            this.Region = region;
        }
    }
}
