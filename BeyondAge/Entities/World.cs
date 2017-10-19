using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class World
    {
        private List<Filter> filters;
        private List<Entity> entities;

        private Entity[] entityGrid;

        public World(int width, int height)
        {
            entityGrid = new Entity[width * height];

            filters = new List<Filter>();
            entities = new List<Entity>();
        }

        public Entity Create(params string[] tags)
        {
            var ent = new Entity(tags);
            entities.Add(ent);
            return ent;
        }

        public Filter Register(Filter filter)
        {
            filters.Add(filter);
            filter.WorldRef = this;
            return filter;
        }

        public List<Entity> GetAllWithComponent(Type t)
        {
            return entities.Where(e => e.Has(t)).ToList();
        }

        public Entity GetFirstWithComponent(Type t)
        {
            foreach(var ent in entities) {
                if (ent.Has(t)) return ent;
            }
            return null;
        }

        public Filter GetFilter<T>()
        {
            foreach(var filter in filters)
            {
                if (filter is T)
                {
                    return (filter);
                }
            }
            return null;
        }

        public void DestroyAll()
        {
            entities.ForEach(e =>
            {
                foreach (var filter in filters)
                {
                    if (filter.Matches(e))
                    {
                        filter.Destroy(e);
                    }
                }
            });
            entities.Clear();
        }

        public void Update(GameTime time)
        {
            filters.ForEach(f => f.PreUpdate(time));
            for (int i = entities.Count() - 1; i >= 0; i--)
            {
                var ent = entities[i];
                if (ent.Remove)
                {
                    entities.RemoveAt(i);
                    foreach (var filter in filters) {
                        if (filter.Matches(ent)) {
                            filter.Destroy(ent);
                        }
                    }
                } else
                {
                    foreach(var filter in filters)
                    {
                        if (filter.Matches(ent))
                        {
                            if (ent.Loaded == false)
                                filter.Load(ent);
                            if (BeyondAge.TheGame.GameStatus == GameManager.Status.RUNNING)
                                filter.Update(ent, time);
                            filter.ConstantUpdate(ent, time);
                        }
                    }
                    ent.Loaded = true;
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            filters.ForEach(f => f.PreDraw(batch));
            entities.ForEach(e =>
            {
                foreach (var filter in filters)
                {
                    if (filter.Matches(e))
                    {
                        filter.Draw(e, batch);
                    }
                }
            });
        }
        
        public void UiDraw(SpriteBatch batch)
        {
            
        }
    }
}
