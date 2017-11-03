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
            map = new TileMap("test_map", world, penumbra, camera);

            var spriteSystem = (SpriteRenderer)world.GetFilter<SpriteRenderer>();
            if (spriteSystem != null)
            {
                SpriteRenderer.MapHeight = map.Height * map.TileHeight * Constants.SCALE;
            }

            var player = world.Create("player");
            player.Add<Body>(new Body { X = 512, Y = 512, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE });
            player.Add<Sprite>(new Sprite(BeyondAge.Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
            player.Add<PhysicsBody>(new PhysicsBody { });
            player.Add<Player>(new Player());
            player.Add<Illuminate>(new Illuminate(new PointLight
            {
                Color = Color.White,
                Intensity = 1,
                Radius = 1500,
                Scale = new Vector2(1500)
            }));

            var test = world.Create("npc");
            test.Add<Body>(new Body { X = 228 + 512, Y = 128 + 512, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE });
            test.Add<Character>(new Character { Name = "Bilmith", Age = 188 });
            var sprite = test.Add<Sprite>(new Sprite(BeyondAge.Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
            sprite.Color = new Color(0f, 0f, 3f, 1f);

            test.Add<PhysicsBody>(new PhysicsBody { });
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
