﻿
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class IMovement
    {
        public IMovement(double movementSpeed, double angle)
        {
            this.Angle = angle;
            this.MovementSpeed = movementSpeed;
        }
        public double MovementSpeed { get; set; }
        [JsonProperty]
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public abstract (double, double) Move();
        //what will happen every frame
        public abstract void Frame(double friction);
        //change properties of this movement
        public abstract void ResetMovement(double angle, double speed);
        //proceed action of this movement on call (eg. player keeps on pressing arrow up)
        public abstract void UpdateMovement();
        public virtual void SuddenStop()
        {
            this.MovementSpeed = 0;
        }
    }
}
