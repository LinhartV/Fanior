using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class PressureWave : Item
    {
        [JsonProperty]
        private int characterId;
        public PressureWave() : base() { }
        public PressureWave(Gvars gvars, double x, double y, Shape shape, Mask mask, int characterId)
            : base(gvars, x, y, shape, mask)
        {
            AddAction(gvars, new ItemAction("pressureWaveSpreading", 1, ItemAction.ExecutionType.EveryTime));
            AddAction(gvars, new ItemAction("dispose", 20, ItemAction.ExecutionType.OnlyFirstTime));
            this.characterId = characterId;
            Solid = false;
        }


        public override void CollideServer(Item collider, double angle)
        {
            if (collider is Movable m && collider.Id != characterId)
            {
                m.AddControlledMovement(new AcceleratedMovement(0, angle, 3, 100), "repellsion" + this.Id);
                m.UpdateControlledMovement("repellsion" + this.Id, gvars.PercentageOfFrame);
                m.RotateControlledMovement("repellsion" + this.Id, angle, false);
            }
        }
    }
}
