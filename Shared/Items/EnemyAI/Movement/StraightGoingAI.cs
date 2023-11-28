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
    /// just go forwards (does not make sence, just for testing purposes)
    /// </summary>
    public class StraightGoingAI : IEnemyMovementAI
    {
        public void Control(Gvars gvars, Enemy enemy)
        {
            enemy.RotateControlledMovement("default", 0, false);
            enemy.UpdateControlledMovement("default", gvars.PercentageOfFrame);
        }
    }
}
