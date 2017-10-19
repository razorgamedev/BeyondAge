using BeyondAge.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Interfaces
{
    interface IGameEventHandler
    {
        void Update(GameTime time);
        void NonPausableUpdate(GameTime time);
        void Draw(SpriteBatch batch, Primitives primitives);
        void DrawGui(SpriteBatch batch, Primitives primitives);
        void Destroy();
        void Load();
    }

    class GameEventHandler
    {
        public virtual void Update(GameTime time) { }
        public virtual void NonPausableUpdate(GameTime time) { }
        public virtual void Draw(SpriteBatch batch, Primitives primitives) { }
        public virtual void DrawGui(SpriteBatch batch, Primitives primitives) { }
        public virtual void Destroy() { }
        public virtual void Load() { }
    }
}
