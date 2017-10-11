using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class PhysicsBody: Component
    {
        public float VelX { get; set; } = 0f;
        public float VelY { get; set; } = 0f;
        public float Speed { get; set; } = 600f;

        public float Friction { get; set; } = 0.5f;
        public float Bounce { get; set; } = 0.2f;
        public float Deceleration { get; set; } = 0.92f;

        public float Direction { get; set; } = 0f;

        public Vector2 Velocity
        {
            get => new Vector2(VelX, VelY);
            set {
                VelX = value.X;
                VelY = value.Y;
            }
        }
    }
}
