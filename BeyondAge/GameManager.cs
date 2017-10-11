using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge
{
    public class GameManager
    {
        public enum Status
        {
            RUNNING,
            PAUSED
        };

        Status GameStatus { get;set; } = GameManager.Status.RUNNING;

        public bool Debugging { get; set; } = false;

        public void Update()
        {
            if (GameInput.Self.KeyPressed(Constants.ToggleDebugKey))
            {
                Debugging = !Debugging;
            }

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                var state = GamePad.GetState(PlayerIndex.One);
                //if (state.IsButtonDown())
            }
        }
    }
}
