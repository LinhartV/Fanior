﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class ConstantMovement : IMovement
    {
        public ConstantMovement(string movementName, double movementSpeed, double angle) : base(movementName, movementSpeed, angle)
        {
        }

        public override (double, double) Move()
        {
            return ((double)(MovementSpeed * Math.Sin(Angle)), (double)(MovementSpeed * Math.Cos(Angle)));

        }

        public override void RenewMovement(double angle, double speed)
        {
            this.Angle = angle;
            MovementSpeed = speed;
        }

        public override void SmoothStop(double friction)
        {
            this.SuddenStop();
        }
    }
}
