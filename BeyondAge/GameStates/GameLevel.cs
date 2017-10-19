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

namespace BeyondAge.GameStates
{
    class GameLevel: GameState
    {
        private World world;
        private TileMap map;

        public GameLevel(World world)
        {
            this.world = world;
        }

        public override void Load()
        {
            map = new TileMap("test_map", world);

            var spriteSystem = (SpriteRenderer)world.GetFilter<SpriteRenderer>();
            if (spriteSystem != null)
            {
                spriteSystem.MapHeight = map.Height * map.TileHeight * Constants.SCALE;
            }

            var player = world.Create("player");
            player.Add<Body>(new Body { X = 128, Y = 128, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE });
            player.Add<Sprite>(new Sprite(BeyondAge.Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
            player.Add<PhysicsBody>(new PhysicsBody { });
            player.Add<Player>(new Player());

            var test = world.Create("npc");
            test.Add<Body>(new Body { X = 128 + 128, Y = 128 + 32, Width = 8 * Constants.SCALE, Height = 6 * Constants.SCALE });
            
            var sprite = test.Add<Sprite>(new Sprite(BeyondAge.Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
            sprite.Color = new Color(0f, 0f, 3f, 1f);

            test.Add<PhysicsBody>(new PhysicsBody { });
        }
        
        public override void Update(GameTime time)
        {
            if (GameInput.Self.KeyPressed(Keys.D1))
                if (gsm != null) gsm.Goto(new Menu(world));
        }

        public override void Draw(SpriteBatch batch, Primitives primitives)
        {
            map.Draw(batch, primitives);
        }

        public override void Destroy()
        {
            world.DestroyAll();
        }
    }
}
