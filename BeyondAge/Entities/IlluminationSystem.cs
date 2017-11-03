using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Penumbra;

namespace BeyondAge.Entities
{
    class IlluminationSystem : Filter
    {
        PenumbraComponent penumbra;
        
        public IlluminationSystem(PenumbraComponent penumbra) : base(typeof(Illuminate), typeof(Body))
        {
            this.penumbra = penumbra;
        }

        public override void Load(Entity ent)
        {
            this.penumbra.Lights.Add(ent.Get<Illuminate>().Light);
        }

        public override void Destroy(Entity ent)
        {
            this.penumbra.Lights.Remove(ent.Get<Illuminate>().Light);
        }

        public override void Update(Entity ent, GameTime time)
        {
            var body = ent.Get<Body>();
            var ill = ent.Get<Illuminate>();

            ill.Light.Position = body.Position;
        }
    }
}
