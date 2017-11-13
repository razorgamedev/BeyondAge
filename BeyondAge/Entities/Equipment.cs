using Microsoft.Xna.Framework;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Entities
{
    class Clothing
    {
        public static readonly Vector2 HairSize = new Vector2(16, 16);
        //public static readonly Vector2 HairOffset = new Vector2(16, 16);

        public enum Type
        {
            Hair,
            Shirt,
            Pants,
            Chestplate,
            Healmet,
            Leggings
        }

        public Type ClothingType { get; set; } = Type.Hair;
        public Point StartPos { get; set; } = Point.Zero;       // Starting point of the sprite
        public Vector2 Offset { get; set; } = Vector2.Zero;     // Offseting location on the body

        public Clothing(Type type)
        {
            this.ClothingType = type;
        }

        public static Clothing LoadHair(string name) {
            var clothing_data = BeyondAge.Assets.GetLuaData("clothing");
            var clothing = new Clothing(Type.Hair);

            var data = (clothing_data["hair"] as LuaTable)[name] as LuaTable;
            clothing.StartPos = new Point(
                (int)(data[1] as Double?),
                (int)(data[2] as Double?)
                );

            return clothing;
        }

        public static Clothing LoadShirt(string name)
        {
            var clothing_data = BeyondAge.Assets.GetLuaData("clothing");
            var clothing = new Clothing(Type.Shirt);

            var data = (clothing_data["shirts"] as LuaTable)[name] as LuaTable;
            clothing.StartPos = new Point(
                (int)(data[1] as Double?),
                (int)(data[2] as Double?)
                );

            return clothing;
        }
    }

    class Equipment
    {

        public List<Rectangle> Frames { get; set; }
        
    }
}
