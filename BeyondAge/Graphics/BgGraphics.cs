using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Graphics
{
    abstract class BgGraphics
    {
        public virtual void Load() { }
        public virtual void Update(GameTime time) { }
        public virtual void Draw(SpriteBatch batch) { }
        public virtual void Destroy() { }
    }
}
