using BeyondAge.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge.Managers
{
    class DialogViewer
    {
        public bool Showing { get; private set; } = false;
        private Dialog currentDialog = null;

        public void ShowDialog(Dialog dialog)
        {
            currentDialog = dialog;
            TimerManager.Self.AddTimer(new Timer{
                Time = 1,
                Callback = () =>
                {
                    currentDialog = null;
                    BeyondAge.TheGame.GameStatus = GameManager.Status.RUNNING;
                }
            });
        }

        public void UiDraw(SpriteBatch batch, Primitives primitives)
        {
            Showing = currentDialog != null;
            if (currentDialog != null)
            {
                primitives.DrawRect(new Rectangle(0, BeyondAge.Height - 256, BeyondAge.Width, 256), Color.SlateGray);
            }
        }
    }
}
