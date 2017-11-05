using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class Entity
    {
        private Dictionary<Type, object> components;
        private List<string> tags;

        public bool Remove { get; private set; } = false;
        public bool Loaded { get; set; } = false;

        public Entity(params string[] tags)
        {
            this.components = new Dictionary<Type, object>();
            this.tags = tags.ToList();
        }

        public List<Type> ComponentTypes { get => components.Keys.ToList(); }
        
        public bool HasTag(string tag) => tags.Contains(tag);
        public void AddTag(params string[] tag)
        {
            foreach(var id in tag) tags.Add(id);
        }

        public void Kill() => Remove = true;
        public void Revive() => Remove = false;

        public bool Has(Type t) => components.ContainsKey(t);

        public T Get<T>()
        {
            // Whew this is gross and slow
            if (Has(typeof(T)) == false) return default(T);
            return (T)(object)(components[typeof(T)]);
        }

        public T Add<T>(T component){
            // I dont even know..
            components.Add(typeof(T), component);
            return (T)(object)(component);
        }

        internal void Add<T>()
        {
            throw new NotImplementedException();
        }
    }
}
