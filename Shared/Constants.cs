using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Constants
    {
        public const int PLAYERS_LIMIT = 50;
        /// <summary>
        /// Duration of actual frame
        /// </summary>
        public const int CONTROL_FRAME_TIME = 20;
        /// <summary>
        /// Duration of actions that happens continuously (movement, healing etc.)
        /// </summary>
        public const int GAMEPLAY_FRAME_TIME = 30;
        public const int DELAY = 0;
        /// <summary>
        /// Coeficient determining score needed to reach next level
        /// </summary>
        public const double NEXT_LEVEL_INCRESE = -9;
        /// <summary>
        /// How much score is needed to reach first level
        /// </summary>
        public const int FIRST_LEVEL = 10;
        /// <summary>
        /// Initial speed of players
        /// </summary>
        public const double INICIAL_MOVEMENT_SPEED = 6;
        public const int POINTS_NEEDED_TO_UPGRADE_WEAPON = 7;

    }
}
