using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Chooses a point and goes towards it
    /// </summary>
    public class FollowClosestAI : IEnemyMovementAI
    {
        public void Control(Gvars gvars, Enemy enemy)
        {
            if (gvars.ItemsPlayers.Count > 0)
            {
                Player closestPlayer = GetClosestPlayer(gvars, enemy);
                enemy.RotateControlledMovement("default", ToolsMath.GetAngleFromLengts(enemy, closestPlayer), false);
                enemy.UpdateControlledMovement("default", gvars.PercentageOfFrame);
            }
        }
        private Player GetClosestPlayer(Gvars gvars, Enemy enemy)
        {
            Player closestPlayer = null;
            double minDist = double.PositiveInfinity;
            foreach (var player in gvars.ItemsPlayers.Values)
            {
                var dist = ToolsMath.GetDistance(player, enemy);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPlayer = player;
                }
            }
            return closestPlayer;
        }
    }
}
