using BeyondAge.Entities;
using BeyondAge.GameStates;
using BeyondAge.Graphics;
using BeyondAge.Utilities;
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
        Camera camera;
        GameStateManager gsm;

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
            gsm = new GameStateManager();

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch       = new SpriteBatch(GraphicsDevice);
            Assets      = new AssetCatalog(Content);
            TheGame     = new GameManager();

            primitives = new Primitives(GraphicsDevice, batch);

            world = new World();
            world.Register(new SpriteRenderer());
            world.Register(new PlayerController(camera));
            var physics = (PhysicsSystem)world.Register(new PhysicsSystem(primitives));
            
            // Goto the menu level
            gsm.Goto(new GameLevel(world));
            //gsm.Goto(new Menu(world));
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
            gsm.Update(time);

            base.Update(time);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearColor);

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, transformMatrix: camera.TranslationMatrix);
            
            gsm.Draw(batch, primitives);
            world.Draw(batch);
            
            batch.End();

            // GUI
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp);
            gsm.DrawGui(batch, primitives);

            // Draw Fps
            if (TheGame.Debugging)
            {
                var font = Assets.GetFont("Font");
                primitives.DrawRect(new Rectangle(10, 10, 64, 64), Color.DarkSlateGray);
                batch.DrawString(font, (Math.Floor(1 / gameTime.ElapsedGameTime.TotalSeconds)).ToString(), new Vector2(20, 20), Color.White);
            }

            batch.End();

            base.Draw(gameTime);
        }
    }
}
