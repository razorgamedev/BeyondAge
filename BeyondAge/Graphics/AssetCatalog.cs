using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NLua;
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

        private Dictionary<string, LuaTable> dialog;
        private Dictionary<string, LuaTable> luaData;

        private ContentManager content;
        
        public AssetCatalog(ContentManager _content, Lua lua) {
            this.textures = new Dictionary<string, Texture2D>();
            this.fonts = new Dictionary<string, SpriteFont>();
            this.dialog = new Dictionary<string, LuaTable>();
            this.luaData = new Dictionary<string, LuaTable>();

            this.content = _content;

            var texture_names = Directory.GetFiles("Content/images");
            var font_names = Directory.GetFiles("Content/fonts");
            var dialog_names = Directory.GetFiles("Content/dialog");
            var lua_data_names = Directory.GetFiles("Content/lua");
            
            foreach (var texture in texture_names)
            {
                var name = texture.Split('/', '\\').Last().Split('.').First();
                textures.Add(name, content.Load<Texture2D>("images/" + name));
            }

            foreach (var font in font_names)
            {
                var name = font.Split('/', '\\').Last().Split('.').First();
                fonts.Add(name, content.Load<SpriteFont>("fonts/" + name));
            }

            foreach(var dialogName in dialog_names)
            {
                var name = dialogName.Split('/', '\\').Last().Split('.').First();
                var tab = lua.DoFile(dialogName).Last() as LuaTable;
                dialog.Add(name, tab);
            }

            foreach(var luaName in lua_data_names)
            {
                var name = luaName.Split('/', '\\').Last().Split('.').First();
                var tab = lua.DoFile(luaName).Last() as LuaTable;
                luaData.Add(name, tab);
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

        public LuaTable GetDialogTable(string name)
        {
            return dialog[name];
        }

        public SpriteFont GetFont(string name)
        {
            if (fonts.ContainsKey(name) == false)
            {
                Console.WriteLine($"ERROR:: Cannot find font: {name}");
                return null;
            }

            return fonts[name];
        }

        public LuaTable GetLuaData(string name)
        {
            if (luaData.ContainsKey(name) == false)
            {
                Console.WriteLine($"[ERROR]:: Cannot find lua data: {name}");
                return null;
            }
            return luaData[name];
        }
    }
}
