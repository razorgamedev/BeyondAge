using BeyondAge.Entities;
using BeyondAge.GameStates;
using BeyondAge.Graphics;
using BeyondAge.Managers;
using BeyondAge.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;
using Penumbra;
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
        Lua lua;
        PenumbraComponent penumbra;

#if DEVELOPMENT_BUILD
        DevelopmentConsole console;
#endif

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

            // Add the penumbra lighting component
            penumbra = new PenumbraComponent(this);
            Components.Add(penumbra);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera();
            gsm = new GameStateManager();
            lua = new Lua();

            var c = new Constants();
            lua["Constants"] = c;


            // Initialize the lighting system
            penumbra.Initialize();
            penumbra.AmbientColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);

            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch       = new SpriteBatch(GraphicsDevice);
            Assets      = new AssetCatalog(Content, lua);
            TheGame     = new GameManager();

            primitives = new Primitives(GraphicsDevice, batch);


#if DEVELOPMENT_BUILD
            console = new DevelopmentConsole(this, batch, primitives, lua);
            Components.Add(console);
#endif

            world = new World(32, 32);
            world.Register(new SpriteRenderer());
            world.Register(new PlayerController(camera));
            world.Register(new CharacterController(primitives));
            world.Register(new IlluminationSystem(penumbra));
            var physics = (PhysicsSystem)world.Register(new PhysicsSystem(primitives));
            
            // Goto the menu level
            gsm.Goto(new GameLevel(world, penumbra, camera));
            //gsm.Goto(new Menu(world));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime time)
        {
            if (TheGame.GameStatus == GameManager.Status.RUNNING)
                if (GameInput.Self.KeyPressed(Keys.Escape))
                    Exit();

            camera.ViewportWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            camera.ViewportHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (TheGame.Debugging)
            {
                if (GameInput.Self.KeyDown(Keys.OemPlus))
                    camera.AdjustZoom(0.02f);
                if (GameInput.Self.KeyDown(Keys.OemMinus))
                    camera.AdjustZoom(-0.02f);
            }

            // TODO: Add your update logic here
            world.Update(time);

            TheGame.Update(time);
            GameInput.Self.Update();
            gsm.Update(time);
            TimerManager.Self.Update(time);

#if DEVELOPMENT_BUILD
            console.Update(time);
#endif

            base.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            penumbra.BeginDraw();
            penumbra.Transform = camera.TranslationMatrix;

            GraphicsDevice.Clear(ClearColor);

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, transformMatrix: camera.TranslationMatrix);
            
            gsm.Draw(batch, primitives);
            world.Draw(batch);
            
            batch.End();

            try
            {
                penumbra.Draw(time);
            } catch (Exception e) { }

            //batch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, transformMatrix: camera.TranslationMatrix);
            //batch.End();

            // GUI
            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp);

            gsm.DrawGui(batch, primitives);
            TheGame.UiDraw(batch, primitives);

            // Draw Fps
            if (TheGame.Debugging)
            {
                var font = Assets.GetFont("Font");
                primitives.DrawRect(new Rectangle(10, 10, 64, 64), Color.DarkSlateGray);
                batch.DrawString(font, (Math.Floor(1 / time.ElapsedGameTime.TotalSeconds)).ToString(), new Vector2(20, 20), Color.White);
            }

            batch.End();

#if DEVELOPMENT_BUILD
            console.Draw(time);
#endif

            //base.Draw(time);
        }
    }
}
