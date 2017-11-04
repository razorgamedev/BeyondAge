using BeyondAge.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

using TiledSharp;

namespace BeyondAge.Graphics
{
    class Billboard
    {
        public Rectangle Region;
        public Vector2 Position;
        public Vector2 Size;
    }

    class TileMap
    {
        private TiledSharp.TmxMap map;
        private List<Rectangle> regions;
        private List<Billboard> billboards;
        private World world;
        private Camera camera;
        private Penumbra.PenumbraComponent penumbra;
        
        private Dictionary<string, Type> bgGraphicEffects = new Dictionary<string, Type>
        {
            {"BgClouds", typeof(BgClouds)}
        };
        
        private List<BgGraphics> bgEffects = new List<BgGraphics>();

        private Dictionary<string, XmlElement> objectTypes;
        private DateTime loadTime;
        private string path;
        private string name;

        public int Width { get => map.Width; }
        public int Height { get => map.Height; }

        public int TileWidth { get => map.TileHeight; }
        public int TileHeight { get => map.TileHeight; }

        public TileMap(string map_name, World world, Penumbra.PenumbraComponent penumbra, Camera camera)
        {
            this.world = world;
            this.penumbra = penumbra;
            this.camera = camera;
            ParseDefaultObjectData();
            Load(map_name);
        }

        // Parses the default values for each entity type.
        public void ParseDefaultObjectData()
        {
            var path = "Content/data/objecttypes.xml";
            var doc = new XmlDocument();
            doc.Load(path);

            var listOfTypes = doc["objecttypes"];
            objectTypes = new Dictionary<string, XmlElement>();
            foreach (XmlElement obj in listOfTypes)
            {
                objectTypes.Add(obj.GetAttribute("name"), obj);
            }
        }

        public void Load(string map_name)
        {
            var physicsSystem = (PhysicsSystem)world.GetFilter<PhysicsSystem>();
            this.path = $"Content/maps/{map_name}.tmx";
            this.name = map_name;

            map = new TmxMap(path);
            loadTime = File.GetLastWriteTime(path);

            regions = new List<Rectangle>();
            billboards = new List<Billboard>();
            Console.WriteLine($"Loading map: {map_name}");

            penumbra.Lights.Clear();
            penumbra.Hulls.Clear();
            physicsSystem.ClearSolids();
            bgEffects.Clear();

            // Set the cameras bounds

            camera.Bounds = Rectangle.Empty;
            if (map.Properties.ContainsKey("ClampCamera"))
            {
                if ((map.Properties["ClampCamera"] as string) == "true")
                {
                    camera.Bounds = new Rectangle(
                        0, 0,
                        (int)((map.Width * map.TileWidth) * Constants.SCALE),
                        (int)((map.Height * map.TileHeight) * Constants.SCALE)
                        );
                }
            }

            if (map.Properties.ContainsKey("BgEffects"))
            {
                var effects = map.Properties["BgEffects"].Split(' ');
                foreach(var effect in effects)
                    if (bgGraphicEffects.ContainsKey(effect))
                        bgEffects.Add((BgGraphics)Activator.CreateInstance(bgGraphicEffects[effect]));
                    else
                    {
                        Console.WriteLine($"[WARNING]:: Cannot find BgGraphics effect {effect}");
                    }

                bgEffects.ForEach(e => e.Load());
            }

            if (physicsSystem != null)
            {
                foreach(var olayer in map.ObjectGroups)
                {
                    foreach(var obj in olayer.Objects)
                    {
                        XmlElement odata = null;
                        if (objectTypes.ContainsKey(obj.Type)) {
                            odata = (objectTypes[obj.Type]);
                        }

                        if (obj.Type == "light")
                        {
                            var color = Color.White;
                            var colorStr = "";
                            var intensity = 1f;
                            var scale = 200f;

                            if (odata != null)
                            {
                                if (odata.HasAttribute("color"))
                                    colorStr = odata.GetAttribute("color");

                                if (odata.HasAttribute("intensity"))
                                    float.TryParse(odata.GetAttribute("intensity"), out intensity);
                                if (odata.HasAttribute("scale"))
                                    float.TryParse(odata.GetAttribute("scale"), out scale);
                            }

                            if (obj.Properties.ContainsKey("color"))
                                colorStr = obj.Properties["color"];

                            if (obj.Properties.ContainsKey("intensity"))
                                float.TryParse(obj.Properties["intensity"], out intensity);
                            if (obj.Properties.ContainsKey("scale"))
                                float.TryParse(obj.Properties["scale"], out scale);

                            colorStr = colorStr.TrimStart('#');
                            var a = byte.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
                            var r = byte.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
                            var g = byte.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
                            var b = byte.Parse(colorStr.Substring(6, 2), NumberStyles.HexNumber);
                            color = new Color(r, g, b);

                            var light = new PointLight
                            {
                                Position = new Vector2((float)obj.X * Constants.SCALE, (float)obj.Y * Constants.SCALE),
                                Color = color,
                                Intensity = intensity,
                                Scale = new Vector2(scale),
                                Enabled = true
                            };
                            penumbra.Lights.Add(light);

                        } else if (obj.Type == "hull")
                        {
                            if (obj.Points != null)
                            {
                                var points = new List<Vector2>();

                                foreach (var p in obj.Points)
                                    points.Add(new Vector2((float)((obj.X + p.X) * Constants.SCALE), (float)((obj.Y + p.Y) * Constants.SCALE)));

                                var hull = new Hull(points.ToArray());
                                if (!hull.Valid)
                                    Console.WriteLine("Hull not valid");

                                penumbra.Hulls.Add(hull);
                            }

                        } else if (obj.Type == "bill") { 

                            var rx = 0;
                            var ry = 0;
                            var rw = 0;
                            var rh = 0;
                            
                            if (odata != null)
                            {
                                if (odata.HasAttribute("rx"))
                                    int.TryParse(odata.GetAttribute("rx"), out rx);
                                if (odata.HasAttribute("ry"))
                                    int.TryParse(odata.GetAttribute("ry"), out ry);
                                if (odata.HasAttribute("rw"))
                                    int.TryParse(odata.GetAttribute("intensity"), out rw);
                                if (odata.HasAttribute("rh"))
                                    int.TryParse(odata.GetAttribute("scale"), out rh);
                            }

                            if (obj.Properties.ContainsKey("rx"))
                                int.TryParse(obj.Properties["rx"], out rx);
                            if (obj.Properties.ContainsKey("ry"))
                                int.TryParse(obj.Properties["ry"], out ry);
                            if (obj.Properties.ContainsKey("rw"))
                                int.TryParse(obj.Properties["rw"], out rw);
                            if (obj.Properties.ContainsKey("rh"))
                                int.TryParse(obj.Properties["rh"], out rh);

                            billboards.Add(new Billboard
                            {
                                Position    = new Vector2((float)(obj.X * Constants.SCALE), (float)(obj.Y * Constants.SCALE)),
                                Size        = new Vector2((float)obj.Width * Constants.SCALE, (float)obj.Height * Constants.SCALE),
                                Region      = new Rectangle(rx,ry,rw,rh)
                            });

                        } else {
                            // Add the solid to the physics engine
                            if (obj.Points != null)
                            {
                                var points = new List<Vector2>();

                                foreach (var p in obj.Points)
                                    points.Add(new Vector2((float)(p.X * Constants.SCALE), (float)(p.Y * Constants.SCALE)));
                                points.Add(new Vector2((float)(obj.Points[0].X * Constants.SCALE), (float)(obj.Points[0].Y * Constants.SCALE)));

                                physicsSystem.AddPolygon(new Polygon
                                {
                                    Points = points,
                                    Position = new Vector2((float)obj.X * Constants.SCALE, (float)obj.Y * Constants.SCALE)
                                });
                            }
                            else
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

            for (int i = map.Layers.Count - 1; i >= 0; i--)
            {
                var layer = map.Layers[i];
                if (layer.Name == "ignore")
                    map.Layers.Remove(layer);
            }
            
        }

        public void Update(GameTime time, bool runTimeReload = false)
        {
            if (runTimeReload)
            {
                var now = File.GetLastWriteTime(path);
                if (now != loadTime)
                {
                    Load(name);
                    loadTime = now;
                }
            }

            bgEffects.ForEach(e => e.Update(time));
        }

        public void Draw(SpriteBatch batch, Primitives primitives)
        {
            BeyondAge.ClearColor = new Color(map.BackgroundColor.R, map.BackgroundColor.G, map.BackgroundColor.B, 1);

            var image_name = map.Tilesets[0].Name;
            var texture = BeyondAge.Assets.GetTexture(image_name);

            bgEffects.ForEach(e => e.Draw(batch));

            foreach (var layer in map.Layers)
            {
                var drawLayer = 0f;
                if (layer.Properties.ContainsKey("layer"))
                    drawLayer = float.Parse(layer.Properties["layer"]);
                
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
                                Color.White,
                                0,
                                Vector2.Zero,
                                SpriteEffects.None,
                                drawLayer
                                );
                        }
                    }
            }

            foreach(var body in billboards)
            {

                var body_y = body.Position.Y + body.Size.Y;
                var mapHeight = SpriteRenderer.MapHeight;
                float layer = 0.3f + (body_y / mapHeight) * 0.1f;
                
                batch.Draw(
                    texture,
                    new Rectangle(
                       (int)(body.Position.X),
                       (int)(body.Position.Y),
                       (int)(body.Region.Width * Constants.SCALE),
                       (int)(body.Region.Height * Constants.SCALE)),
                   body.Region,
                   Color.White,
                   0,
                   Vector2.Zero,
                   SpriteEffects.None,
                   layer
                );

            }
        }
    }
}
