using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class Body: Component
    {
        public float X          { get; set; } = 0f;
        public float Y          { get; set; } = 0f;
        public float Width      { get; set; } = 0f;
        public float Height     { get; set; } = 0f;

        public float Left { get => X; }
        public float Right { get => X + Width; }
        public float Top { get => Y; }
        public float Bottom { get => Y + Height; }

        public Vector2 Position {
            get => new Vector2(X, Y);
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        public Vector2 Size{
            get => new Vector2(Width, Height);
            set {
                Width = value.X;
                Height = value.Y;
            }
        }

        public Rectangle Region
        {
            get => new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public bool Contains(Body other) => 
            ( 
            X + Width > other.X && X < other.X + other.Width && 
            Y + Height > other.Y && Y < other.Y + other.Height
            );
    }
}
