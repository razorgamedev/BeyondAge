using Microsoft.Xna.Framework;
using NLua;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class EntityAssembler
    {
        private World world;

        public EntityAssembler(World world)
        {
            this.world = world;
        }

        private bool ContainsKey(LuaTable t, string key)
        {
            var myList = new List<string>();
            foreach(string aKey in t.Keys)
                myList.Add(aKey);
            return myList.Contains(key);
        }

        private List<string> ToStringList(LuaTable t)
        {
            var list = new List<string>();
            foreach(string obj in t.Values)
                list.Add(obj);
            return list;
        }

        private List<string> GetKeys(LuaTable t)
        {
            var list = new List<string>();
            foreach(var key in t.Keys)
                list.Add((string)key);
            return list;
        }

        private bool ValidateKeys(string name, LuaTable t, params string[] keys)
        {
            bool valid = true;
            var theKeys = GetKeys(t);
            foreach (var key in keys)
                if (!theKeys.Contains(key))
                {
                    Console.WriteLine($"[WARNING]:: Component {name} is missing key {key}");
                    return false;
                }
            return valid;
        }

        public Entity Assemble(string name)
        {
            var allEntData = BeyondAge.Assets.GetLuaData("entities");
            var entData = allEntData[name] as LuaTable;
            var entity = this.world.Create();

            if (entData == null)
            {
                Console.WriteLine($"[WARNING]:: Entity assembler cannot find {name}");
                return entity;
            }

            // Add tags to the entity
            if (ContainsKey(entData, "tags")) {
                var tags = ToStringList(entData["tags"] as LuaTable);
                entity.AddTag(tags.ToArray());
            }

            if (ContainsKey(entData, "components")) {
                var comData = entData["components"] as LuaTable;
                var keys = GetKeys(comData);
                foreach(var key in keys)
                {
                    var component = comData[key] as LuaTable;
                    var componentKeys = GetKeys(component);

                    switch (key)
                    {
                        case "Body":
                            {
                                if (!ValidateKeys(key, component, "X", "Y", "Width", "Height")) return entity;
                                var x = (float)(component["X"] as Double?);
                                var y = (float)(component["Y"] as Double?);
                                var width = (float)(component["Width"] as Double?);
                                var height = (float)(component["Height"] as Double?);
                                entity.Add<Body>(new Body { X = x, Y = y, Width = width, Height = height });
                            }
                            break;
                        case "Sprite":
                            {
                                if (!ValidateKeys(key, component, "Texture", "Region")) return entity;

                                var textureName = component["Texture"] as string;
                                var region = component["Region"] as LuaTable;
                                var color = Color.White;
                                var scaleX = 1f;
                                var scaleY = 1f;

                                if (componentKeys.Contains("Color"))
                                {
                                    var colorData = (component["Color"] as LuaTable);
                                    var r = (float)(colorData[1] as Double?);
                                    var g = (float)(colorData[2] as Double?);
                                    var b = (float)(colorData[3] as Double?);
                                    var a = (float)(colorData[4] as Double?);
                                    color = new Color(r, g, b, a);
                                }

                                if (componentKeys.Contains("Scale"))
                                {
                                    var scale = component["Scale"] as LuaTable;
                                    scaleX = (float)(scale[1] as Double?);
                                    scaleY = (float)(scale[2] as Double?);
                                }

                                var sprite = entity.Add<Sprite>(new Sprite(
                                    BeyondAge.Assets.GetTexture(textureName),
                                    new Rectangle(
                                        (int)(region[1 + 0] as Double?),
                                        (int)(region[1 + 1] as Double?),
                                        (int)(region[1 + 2] as Double?),
                                        (int)(region[1 + 3] as Double?)
                                        )));
                                sprite.Color = color;
                                sprite.ScaleX = scaleX;
                                sprite.ScaleY = scaleY;
                            }
                            break;
                        case "PhysicsBody":
                            entity.Add<PhysicsBody>(new PhysicsBody());
                            break;
                        case "Player":
                            entity.Add<Player>(new Player());
                            break;
                        case "Illuminate":
                            {
                                var color = Color.White;
                                var intensity = 1f;
                                var radius = 500f;
                                var scale = 500f;

                                if (componentKeys.Contains("Color"))
                                {
                                    var colorData = (component["Color"] as LuaTable);
                                    var r = (float)(colorData[1] as Double?);
                                    var g = (float)(colorData[2] as Double?);
                                    var b = (float)(colorData[3] as Double?);
                                    var a = (float)(colorData[4] as Double?);
                                    color = new Color(r, g, b, a);
                                }

                                if (componentKeys.Contains("Intensity"))
                                    intensity = (float)(component["Intensity"] as Double?);

                                if (componentKeys.Contains("Radius"))
                                    radius = (float)(component["Radius"] as Double?);

                                if (componentKeys.Contains("Scale"))
                                    scale = (float)(component["Scale"] as Double?);

                                entity.Add<Illuminate>(new Illuminate(new PointLight
                                {
                                    Color = color,
                                    Intensity = intensity,
                                    Radius = radius,
                                    Scale = new Vector2(scale)
                                }));
                            }
                            break;
                        case "Character":
                            {
                                if (!ValidateKeys(key, component, "Name")) return entity;

                                var charName = component["Name"] as string;
                                entity.Add<Character>(new Character { Name = charName });
                            }
                            break;
                        case "Health":
                            {
                                entity.Add<Status>(new Status());
                            }
                            break;
                        default:
                            Console.WriteLine($"[WARNING]:: Unknown component type: {key}");
                            break;
                    }

                }
            } else {
                Console.WriteLine("[WARNING]:: Entity was assembled without components.");
            }

            return entity;
        }
    }
}
