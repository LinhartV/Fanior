using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Actions shared on server and client and executed by name.
    /// </summary>
    public static class PlayerAction
    {
        public static void MoveUp(int playerId, Gvars gvars)
        {
            gvars.ItemsPlayers[playerId].Y -= 5;

        }
        public static void MoveDown(int playerId, Gvars gvars)
        {
            gvars.ItemsPlayers[playerId].Y += 5;
        }
        public static void MoveRight(int playerId, Gvars gvars)
        {
            gvars.ItemsPlayers[playerId].X += 5;
        }
        public static void MoveLeft(int playerId, Gvars gvars)
        {
            gvars.ItemsPlayers[playerId].X -= 5;
        }
    }

}
