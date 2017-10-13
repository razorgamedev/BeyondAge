using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondAge
{
    class Constants
    {
        public static readonly float SCALE = 8;

        public static readonly string ToggleDebugKey = "ToggleDebug";

        public static readonly string PlayerMoveLeft    = "PlayerMoveLeft";
        public static readonly string PlayerMoveRight   = "PlayerMoveRight";
        public static readonly string PlayerMoveUp      = "PlayerMoveUp";
        public static readonly string PlayerMoveDown    = "PlayerMoveDown";

        public static readonly int GridSize                 = 8;
        public static readonly float CameraSmoothValue      = 0.02f;
        public static readonly float CameraPredictionScale  = 2.0f; // Used to point towards the players position + velocity * scale

    }
}
