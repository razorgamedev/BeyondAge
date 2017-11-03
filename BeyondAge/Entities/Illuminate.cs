using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BeyondAge.Entities
{
    class Illuminate: Component
    {
        private Point point;

        public Light Light { get;set; }

        public Illuminate(Light light)
        {
            this.Light = light;
        }

        public Illuminate(Point point)
        {
            this.point = point;
        }
    }
}
