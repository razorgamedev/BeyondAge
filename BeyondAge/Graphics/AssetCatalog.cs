using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Graphics
{
    public class AssetCatalog
    {
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, SpriteFont> fonts;

        private ContentManager content;
        
        public AssetCatalog(ContentManager _content) {
            this.textures = new Dictionary<string, Texture2D>();
            this.content = _content;

            var texture_names = Directory.GetFiles("Content/images");
            foreach (var texture in texture_names)
            {
                var name = texture.Split('/', '\\').Last().Split('.').First();
                
                textures.Add(name, content.Load<Texture2D>("images/" + name));
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (textures.ContainsKey(name) == false)
            {
                Console.WriteLine($"ERROR:: Cannot find texture: {name}");
                return null;
            }

            return textures[name];
        }
    }
}
