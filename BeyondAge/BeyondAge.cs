using BeyondAge.Entities;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BeyondAge
{
    public class BeyondAge : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch batch;
        World world;
        Primitives primitives;
        TileMap map;
        Camera camera;

        public static readonly int Width = 1280;
        public static readonly int Height = 720;
        public static AssetCatalog Assets { get; private set; }
        public static GameManager TheGame { get; private set; }
        public static Color ClearColor { get;set; } = Color.CornflowerBlue;

        public BeyondAge()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };

            graphics.ApplyChanges();
            
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera();

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch       = new SpriteBatch(GraphicsDevice);
            Assets      = new AssetCatalog(Content);
            TheGame     = new GameManager();

            primitives = new Primitives(GraphicsDevice, batch);

            map = new TileMap("test_map");

            world = new World();
            world.Register(new SpriteRenderer());
            world.Register(new PlayerController(camera));
            world.Register(new PhysicsSystem(primitives));

            var test = world.Create("player");
            test.Add<Body>(new Body { X = 128, Y = 128, Width = 8 * Constants.SCALE, Height = 16 * Constants.SCALE });
            test.Add<Sprite>(new Sprite(Assets.GetTexture("character_sheet"), new Rectangle(0, 0, 8, 16)));
            test.Add<PhysicsBody>(new PhysicsBody{});
            test.Add<Player>(new Player());

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime time)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            world.Update(time);

            TheGame.Update();
            GameInput.Self.Update();
            base.Update(time);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearColor);

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, transformMatrix: camera.TranslationMatrix);
            
            map.Draw(batch, primitives);
            
            world.Draw(batch);
            batch.End();

            // GUI
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp);
            var font = Assets.GetFont("Font");

            primitives.DrawRect(new Rectangle(10, 10, 64, 64), Color.DarkSlateGray);
            batch.DrawString(font, (Math.Floor(1 / gameTime.ElapsedGameTime.TotalSeconds)).ToString(), new Vector2(20, 20), Color.White);

            batch.End();

            base.Draw(gameTime);
        }
    }
}
