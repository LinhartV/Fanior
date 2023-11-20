using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Ability of a player
    /// </summary>
    public class Ability
    {
        /// <summary>
        /// Create new ability
        /// </summary>
        /// <param name="reloadTime">Number of seconds to reload the ability</param>
        /// <param name="duration">Number of seconds of duration of the ability</param>
        /// <param name="name">Lambda action name and display name</param>
        /// <param name="cost">Number of upgrade points needed to obtain this ability</param>
        /// <param name="imageName">Image name of this ability</param>
        /// <param name="description">Displayed description of the ability</param>
        public Ability(double reloadTime, double duration, string name, int cost, string imageName, string description)
        {
            ReloadTime = reloadTime;
            Duration = duration;
            Name = name;
            this.lambdaAction = name;
            this.Cost = cost;
            this.ImageName = imageName;
            Description = description;
        }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public bool Reloaded { get; set; } = true;
        public bool BeingUsed { get; set; } = false;
        //number of frames it takes to reload
        public double ReloadTime { get; set; }
        //number of frames it durates
        public double Duration { get; set; }
        public string Name { get; set; }
        private string lambdaAction;

        public void UseAbility(Gvars gvars, int id)
        {
            if (Reloaded)
            {
                BeingUsed = true;
                Reloaded = false;
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("abilityRunOut", ToolsMath.TimeToFrames(Duration), ItemAction.ExecutionType.OnlyFirstTime, false, this));
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction(lambdaAction, 0, ItemAction.ExecutionType.OnlyFirstTime, false, ToolsMath.TimeToFrames(Duration)));
                gvars.ItemsPlayers[id].AddAction(gvars, new ItemAction("abilityReload", ToolsMath.TimeToFrames(ReloadTime+Duration), ItemAction.ExecutionType.OnlyFirstTime, false, this));
            }
        }

    }
}
