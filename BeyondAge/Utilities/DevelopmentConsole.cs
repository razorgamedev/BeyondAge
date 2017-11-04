using BeyondAge.Graphics;
using NLua;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;

namespace BeyondAge.Utilities
{
    class DevelopmentConsole : DrawableGameComponent
    {
        SpriteBatch batch;
        Primitives primitives;
        Lua lua;

        public DevelopmentConsole(Game game, SpriteBatch batch, Primitives primitives, Lua lua) : base(game)
        {
            this.lua = lua;
            this.batch = batch;
            this.primitives = primitives;
            this.Enabled = true;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (GameInput.Self.KeyPressed(Constants.ToggleDebugKey) && GameInput.Self.KeyDown(Keys.LeftShift)){

            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
        }

    }
}
