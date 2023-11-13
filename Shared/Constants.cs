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
        /// Coeficient of number of score needed to reach next level
        /// </summary>
        public const double NEXTLEVELINCRESE = 5;
    }
}
