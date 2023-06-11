
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Character : Movable, ILived
    {
        //initial shield
        public double MaxShield { get; set; }
        [JsonProperty]
        private double shield;
        public double Shield
        {
            get => shield; 
            private set
            {
                if (value < 0)
                    shield = 0;
                else
                    shield = value;
            }
        }
        public double MaxLives { get; set; }
        [JsonProperty]
        private double curLives;
        // Angle where the character is "looking" (for picture, shooting and stuff)
        private double angle;
        public double Angle
        {
            get => angle;
            set
            {
                angle = value % (Math.PI * 2);
            }
        }
        public Weapon Weapon { get; set; }
        public double Regeneration { get; set; }

        public Character() { }
        public Character(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double lives, double regeneration, Weapon weapon, double shield = 0, IMovement defaultMovement = null, bool isVisible = true) :
            base(gvars, x, y, shape, mask, movementSpeed, defaultMovement, acceleration, friction, isVisible)
        {
            this.MaxShield = shield;
            this.Shield = shield;
            this.Regeneration = regeneration;
            MaxLives = lives;
            this.curLives = lives;
            this.Weapon = weapon;
            if (Weapon != null)
            {
                weapon.characterId = this.Id;
            }
            this.AddAction(new ItemAction("regenerate", 1));
        }
        public virtual void Death(Gvars gvars)
        {
            this.Dispose(gvars);
        }

        public abstract int Bounty();

        public void ChangeCurLives(double amount, Item killer, Gvars g)
        {
            curLives += amount;
            if (curLives < 0)
            {
                curLives = 0;
                Death(g);
                if (killer is Player p)
                {
                    p.Score += this.Bounty();
                }
            }
            if (curLives > MaxLives)
            {
                curLives = MaxLives;
            }
        }

        public double GetCurLives()
        {
            return curLives;
        }

        public override void Collide(Item collider, double angle, Gvars gvars)
        {
            base.Collide(collider, angle, gvars);
            if (collider is Shot s && s.CharacterId != this.Id)
            {
                this.ReceiveDamage(s.Damage, gvars.ItemsPlayers.ContainsKey(s.CharacterId) ? gvars.ItemsPlayers[s.CharacterId] : null, gvars);
            }
        }

        protected virtual void ReceiveDamage(double damage, Item killer, Gvars gvars)
        {
            if (Shield > 0)
            {
                if (Shield - damage < 0)
                {
                    this.ChangeCurLives(-(damage - Shield), killer ,gvars);
                }
                Shield -= damage;
            }
            else
            {
                this.ChangeCurLives(-damage, killer, gvars);
            }
            

        }
    }
}
