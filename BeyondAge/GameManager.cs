using BeyondAge.Graphics;
using BeyondAge.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLua;

namespace BeyondAge
{
    public class GameManager
    {
        DialogViewer dialogViewer;

        public GameManager()
        {
            dialogViewer = new DialogViewer();
        }

        public enum Status
        {
            RUNNING,
            PAUSED
        };

        public Status GameStatus { get; set; } = GameManager.Status.RUNNING;

        public bool Debugging { get; set; } = false;

        public void DoDialog(LuaTable dialog, int startingIndex = 1)
        {
            dialogViewer.ShowDialog(dialog, startingIndex);
        }
        
        public void Update(GameTime time)
        {
#if DEVELOPMENT_BUILD
            if (GameInput.Self.KeyPressed(Constants.ToggleDebugKey) && !GameInput.Self.KeyDown(Keys.LeftShift))
            {
                Debugging = !Debugging;
            }
#endif

            if (dialogViewer.Showing)
            {
                GameStatus = Status.PAUSED;
                dialogViewer.Update(time);
            }

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                var state = GamePad.GetState(PlayerIndex.One);
            }
        }

        public void UiDraw(SpriteBatch batch, Primitives primitives)
        {
            //if (dialogViewer.Showing)
            dialogViewer.UiDraw(batch, primitives);
        }
    }
}
