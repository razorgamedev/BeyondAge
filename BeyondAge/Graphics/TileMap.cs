using BeyondAge.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private List<Rectangle> regions;
        private World world;

        public int Width { get => map.Width; }
        public int Height { get => map.Height; }

        public int TileWidth { get => map.TileHeight; }
        public int TileHeight { get => map.TileHeight; }

        public TileMap(string map_name, World world)
        {
            this.world = world;
            Load(map_name);
        }

        public void Load(string map_name)
        {
            var physicsSystem = (PhysicsSystem)world.GetFilter<PhysicsSystem>();

            map = new TmxMap($"Content/maps/{map_name}.tmx");

            regions = new List<Rectangle>();

            if (physicsSystem != null)
            {
                foreach(var olayer in map.ObjectGroups)
                {
                    foreach(var obj in olayer.Objects)
                    {
                        if (obj.Points != null)
                        {
                            var points = new List<Vector2>();
                            
                            foreach(var p in obj.Points)
                                points.Add(new Vector2((float)(p.X * Constants.SCALE), (float)(p.Y * Constants.SCALE)));
                            points.Add(new Vector2((float)(obj.Points[0].X * Constants.SCALE), (float)(obj.Points[0].Y * Constants.SCALE)));
                            
                            physicsSystem.AddPolygon(new Polygon
                            {
                                Points = points,
                                Position = new Vector2((float)obj.X * Constants.SCALE, (float)obj.Y * Constants.SCALE)
                            });
                        } else
                        {
                            physicsSystem.AddSolid(
                                new Solid
                                {
                                    X = (float)obj.X * Constants.SCALE,
                                    Y = (float)obj.Y * Constants.SCALE,
                                    Width = (float)obj.Width * Constants.SCALE,
                                    Height = (float)obj.Height * Constants.SCALE
                                }
                                );
                        }
                    }
                }
            }
            var ts = map.Tilesets[0];
            int ntw = (int)(ts.Image.Width / ts.TileWidth);
            int nth = (int)(ts.Image.Height / ts.TileHeight);

            for (int y = 0; y < nth; y++)
                for (int x = 0; x < ntw; x++)
                {
                    var xx = x * ts.TileWidth;
                    var yy = y * ts.TileHeight;
                    var ww = ts.TileWidth;
                    var hh = ts.TileHeight;

                    regions.Add(new Rectangle(x: xx, y: yy, width: ww, height: hh));
                }
        }

        public void Draw(SpriteBatch batch, Primitives primitives)
        {
            BeyondAge.ClearColor = new Color(map.BackgroundColor.R, map.BackgroundColor.G, map.BackgroundColor.B, 1);

            var image_name = map.Tilesets[0].Name;
            var texture = BeyondAge.Assets.GetTexture(image_name);

            foreach (var layer in map.Layers)
            {
                for (int y = 0; y < map.Height; y++)
                    for (int x = 0; x < map.Width; x++)
                    {
                        var index = x + y * map.Width;
                        var tile = layer.Tiles[index];
                        if (tile.Gid != 0)
                        {
                            var region = regions[tile.Gid - 1];
                            batch.Draw(
                                texture,
                                new Rectangle(
                                    x * map.TileWidth * (int)(Constants.SCALE), 
                                    y * map.TileHeight * (int)(Constants.SCALE), 
                                    map.TileWidth * (int)(Constants.SCALE), 
                                    map.TileHeight * (int)(Constants.SCALE)),
                                region,
                                Color.White
                                );
                        }
                    }
            }

        }
    }
}
