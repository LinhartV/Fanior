
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fanior.Shared
{
    public class ConstantMovement : IMovement
    {
        //speed the item can be moving by
        [JsonProperty]
        private double baseSpeed = 0;
        [JsonProperty]
        private int stopMovement = 0;
        public ConstantMovement(double movementSpeed, double angle) : base(movementSpeed, angle)
        {
            baseSpeed = movementSpeed;
        }
        public ConstantMovement() : base(){}
        public override void AntiFrame(double friction)
        {
            
        }

        public override void AntiUpdateMovement()
        {
            stopMovement = 0;
        }

        public override bool Frame(double friction, double percentage)
        {
            stopMovement++;
            if (stopMovement > 1)
            {
                MovementSpeed = 0;
                return true;
            }
            else
            {
                MovementSpeed = baseSpeed;
                return false;
            }
        }

        public override (double, double) Move(double percentage)
        {
            return ToolsMath.PolarToCartesian(Angle, MovementSpeed * percentage); 

        }

        public override void ResetMovementAngle(double angle)
        {
            this.Angle = angle;
        }
        public override void ResetMovementSpeed(double speed)
        {
            this.baseSpeed = speed;
        }
        public override void UpdateMovement(double percentage)
        {
            stopMovement = 0;
        }
    }
}
