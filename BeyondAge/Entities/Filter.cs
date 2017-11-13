using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    abstract class Filter
    {
        protected List<Type> filter;
        protected List<Type> optional;

        public  World WorldRef;
        public Filter(params Type[] filter) {
            this.filter = filter.ToList();
            this.optional = new List<Type>();
        }

        public bool Matches(Entity ent)
        {
            var keys = ent.ComponentTypes;
            var matches = true;
            foreach (var f in filter) {
                if (!keys.Contains(f))
                {
                    matches = false;
                    break;
                }
            }
            return matches;
        }

        public virtual void Load(Entity ent) { }
        public virtual void PreUpdate(GameTime time) { }
        public virtual void PreDraw(SpriteBatch batch) { }
        public virtual void UiDraw(Entity ent, SpriteBatch batch) { }
        public virtual void Update(Entity ent, GameTime time) { }
        public virtual void ConstantUpdate(Entity ent, GameTime time) { }
        public virtual void Draw(Entity ent, SpriteBatch batch) { }
        public virtual void DebugDraw(Entity ent, SpriteBatch batch) { }
        public virtual void DebugUiDraw(Entity ent, SpriteBatch batch) { }
        public virtual void Destroy(Entity ent) { }
    }
}
