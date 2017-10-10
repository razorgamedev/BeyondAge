using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TiledSharp;

namespace BeyondAge.Graphics
{
    class TileMap
    {
        private TiledSharp.TmxMap map;

        public TileMap(string map_name)
        {
            map = new TmxMap($"Content/{map_name}.tmx");
        }
    }
}
