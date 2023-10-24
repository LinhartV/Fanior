using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class StraightGoingAI : IEnemyAI
    {
        public void Control(Gvars gvars, Enemy enemy)
        {
            enemy.RotateControlledMovement("default", 0, false);
            if(enemy.X > gvars.ArenaWidth)
            {
                //Console.WriteLine(gvars.GetNow());
            }
            enemy.UpdateControlledMovement("default", gvars.PercentageOfFrame);
        }
    }
}
