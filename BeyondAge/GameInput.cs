using NLua;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge
{
    class KeyState
    {
        public bool Now;
        public bool Last;
    }

    class GameInput
    {
        private static GameInput self;
        private Dictionary<Keys, KeyState> keyStates;
        private LuaTable KeyMap;

        private GameInput() { 
            keyStates = new Dictionary<Keys, KeyState>();
            
            Lua lua = new Lua();
            var fn = lua.LoadFile("Content/lua/keymap.lua");

            KeyMap = fn.Call()[0] as LuaTable;

            
        }
        
        public bool KeyPressed(string keyName)
        {
            var theKey = (KeyMap["Keyboard"] as LuaTable)[keyName];
            bool succ = Enum.TryParse<Keys>(theKey as string, out Keys key);
            if (!succ)
            {
                Console.WriteLine($"WARNING::GameInput:: Cant find key: {theKey}");
                return false;
            }
            return KeyPressed(key);
        }

        public bool KeyReleased(string keyName)
        {
            var theKey = (KeyMap["Keyboard"] as LuaTable)[keyName];
            bool succ = Enum.TryParse<Keys>(theKey as string, out Keys key);
            if (!succ)
            {
                Console.WriteLine($"WARNING::GameInput:: Cant find key: {theKey}");
                return false;
            }
            return KeyReleased(key);
        }

        public bool KeyDown(string keyName)
        {
            var theKey = (KeyMap["Keyboard"] as LuaTable)[keyName];
            bool succ = Enum.TryParse<Keys>(theKey as string, out Keys key);
            if (!succ)
            {
                Console.WriteLine($"WARNING::GameInput:: Cant find key: {theKey}");
                return false;
            }
            return KeyDown(key);
        }

        public bool KeyUp(string keyName)
        {
            return !KeyDown(keyName);
        }

        public bool KeyDown(Keys key)
        {
            if (!keyStates.ContainsKey(key))
            {
                keyStates.Add(key, new KeyState { Now = false, Last = false });
                return false;
            }

            var state = keyStates[key];
            state.Now = Keyboard.GetState().IsKeyDown(key);
            return state.Now;
        }

        public bool KeyUp(Keys key) => !KeyDown(key);

        public bool KeyPressed(Keys key)
        {
            if (!keyStates.ContainsKey(key))
            {
                keyStates.Add(key, new KeyState { Now = false, Last = false });
                return false;
            }

            var k = keyStates[key];
            k.Now = KeyDown(key);

            return (k.Now && !k.Last);
        }

        public bool KeyReleased(Keys key)
        {
            if (!keyStates.ContainsKey(key))
            {
                keyStates.Add(key, new KeyState { Now = false, Last = false });
                return false;
            }

            var k = keyStates[key];
            k.Now = KeyDown(key);

            return (!k.Now && k.Last);
        }

        public void Update()
        {
            foreach(var key in keyStates.Values)
            {
                key.Last = key.Now;
            }
        }

        public static GameInput Self
        {
            get {
                if (self == null) self = new GameInput();
                return self;
            }
        }
    }
}
