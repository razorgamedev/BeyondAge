using BeyondAge.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondAge.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BeyondAge.Entities;

namespace BeyondAge.GameStates
{
    class Menu: GameState
    {
        World world;
        Camera camera;
        Penumbra.PenumbraComponent penumbra;

        public Menu(World world, Penumbra.PenumbraComponent penumbra, Camera camera)
        {
            this.world = world;
            this.penumbra = penumbra;
            this.camera = camera;
        }

        public override void DrawGui(SpriteBatch batch, Primitives primitives)
        {
            BeyondAge.ClearColor = Color.CornflowerBlue;

            var font = BeyondAge.Assets.GetFont("Font");
            
            var scale = 2f;
            var origin = new Vector2(
                -font.MeasureString("MENU").X,
                -font.MeasureString("MENU").Y
                ) / 2;
            batch.DrawString(font, "MENU", new Vector2(BeyondAge.Width / 2, BeyondAge.Height / 2), Color.White, 0, -origin, scale, SpriteEffects.None, 1);
        }

        public override void Update(GameTime time)
        {
            if (GameInput.Self.KeyPressed(Keys.Enter))
                if (this.gsm != null)
                    gsm.Goto(new GameLevel(world, penumbra, camera));
        }
    }
}
