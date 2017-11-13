using BeyondAge.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BeyondAge.Entities;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using NLua;

namespace BeyondAge.GameStates
{
    class GameLevel: GameState
    {
        private World world;
        private TileMap map;
        private Camera camera;
        private Penumbra.PenumbraComponent penumbra;

        public GameLevel(World world, Penumbra.PenumbraComponent penumbra, Camera camera)
        {
            this.camera = camera;
            this.world = world;
            this.penumbra = penumbra;
        }

        public override void Load()
        {
            map = new TileMap("Dungeon_Room_2", world, penumbra, camera);

            var spriteSystem = (SpriteRenderer)world.GetFilter<SpriteRenderer>();
            if (spriteSystem != null)
            {
                SpriteRenderer.MapHeight = map.Height * map.TileHeight * Constants.SCALE;
            }

            camera.Zoom = 0.6f;
            
            var entities = BeyondAge.Assets.GetLuaData("entities");

            var p = world.Assemble("Player", 1280, 1280);
            p.Add<Animation>(new Animation(
                BeyondAge.Assets.GetTexture("character_sheet"),
                new Dictionary<string, List<Frame>>
                {
                    {"front",   new List<Frame>{ 
                        new Frame { X = 0,  Y = 0,  Width = 16, Height = 32 },
                        new Frame { X = 0,  Y = 32, Width = 16, Height = 32 },
                        new Frame { X = 0,  Y = 64, Width = 16, Height = 32 },
                        new Frame { X = 0,  Y = 96, Width = 16, Height = 32 },
                    }},
                    {"left",    new List<Frame>{ 
                        new Frame { X = 16,  Y = 0,  Width = 16, Height = 32 },
                        new Frame { X = 16,  Y = 32, Width = 16, Height = 32 },
                        new Frame { X = 16,  Y = 64, Width = 16, Height = 32 },
                        new Frame { X = 16,  Y = 96, Width = 16, Height = 32 },
                    }},
                    {"back",    new List<Frame>{ 
                        new Frame { X = 32,  Y = 0,  Width = 16, Height = 32 },
                        new Frame { X = 32,  Y = 32, Width = 16, Height = 32 },
                        new Frame { X = 32,  Y = 64, Width = 16, Height = 32 },
                        new Frame { X = 32,  Y = 96, Width = 16, Height = 32 },
                    }},
                    {"right",   new List<Frame>{ 
                        new Frame { X = 48,  Y = 0,  Width = 16, Height = 32 },
                        new Frame { X = 48,  Y = 32, Width = 16, Height = 32 },
                        new Frame { X = 48,  Y = 64, Width = 16, Height = 32 },
                        new Frame { X = 48,  Y = 96, Width = 16, Height = 32 },
                    }}
                }));
            p.Add<Character>(new Character
            {
                CharacterType = Character.Type.Player,
                Clothing = {
                    Clothing.LoadHair("plain_brown"),
                    Clothing.LoadShirt("plain_tee")
                }
            });

            var n = world.Assemble("Npc1", 228 + 512, 128 + 512);
        }
        
        public override void Update(GameTime time)
        {
            if (GameInput.Self.KeyPressed(Keys.D1))
                if (gsm != null) gsm.Goto(new Menu(world, penumbra, camera));
            map.Update(time);
        }

        public override void Draw(SpriteBatch batch, Primitives primitives)
        {
            map.Draw(batch, primitives);
        }

        public override void Destroy()
        {
            world.DestroyAll();
            penumbra.Lights.Clear();
            penumbra.Hulls.Clear();

            var physicsSystem = (PhysicsSystem)world.GetFilter<PhysicsSystem>();
            if (physicsSystem != null)
                physicsSystem.ClearSolids();
        }
    }
}
