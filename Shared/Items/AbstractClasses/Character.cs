

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
            set
            {
                if (value < 0)
                    shield = 0;
                else
                    shield = value;
                gvars?.AddProperty(Id, ItemProperties.Shield, shield);
            }
        }
        public double MaxLives { get; set; }

        [JsonProperty]
        private WeaponTree.WeaponNode weaponNode;
        public WeaponTree.WeaponNode WeaponNode
        {
            get => weaponNode;
            set
            {
                weaponNode = value;
                if (value != null)
                    weaponNode.Weapon.CharacterId = Id;
            }
        }
        public double Regeneration { get; set; }
        [JsonProperty]
        private double curLives;
        public double CurLives
        {
            get => curLives;
            set
            {
                curLives = value;
            }
        }
        [JsonProperty]
        private bool empowered;
        public bool Empowered
        {
            get => empowered;
            set
            {
                empowered = value;
                gvars?.AddProperty(Id, ItemProperties.Empowerment, Convert.ToDouble(empowered));
            }
        }
        [JsonProperty]
        private bool immortal;
        public bool Immortal
        {
            get => immortal;
            set
            {
                immortal = value;
                gvars?.AddProperty(Id, ItemProperties.Immortality, Convert.ToDouble(immortal));
            }
        }
        public double Damage { get; set; } = 1;
        public double BodyDamage { get; set; } = 0.5;
        [JsonProperty]
        private double reloadTime = 1;
        public double ReloadTime
        {
            get => reloadTime;
            set
            {
                reloadTime = value;
                if (gvars != null && gvars.server)
                    ChangeRepeatTime(value * weaponNode.Weapon.ReloadTimeCoef, "fire");
            }
        }
        public double BulletSpeed { get; set; } = 1;

        public Character() { }
        public Character(Gvars gvars, double x, double y, Shape shape, Mask mask, double movementSpeed, double acceleration, double friction, double lives, double regeneration, WeaponTree.WeaponNode weaponNode, bool setAngle, double shield = 0, IMovement defaultMovement = null, bool isVisible = true) :
            base(gvars, x, y, shape, mask, movementSpeed, defaultMovement, acceleration, friction, setAngle, isVisible)
        {
            this.MaxShield = shield;
            this.Shield = shield;
            this.Regeneration = regeneration;
            MaxLives = lives;
            this.curLives = lives;
            this.WeaponNode = weaponNode;
            if (weaponNode != null)
            {
                weaponNode.Weapon.CharacterId = this.Id;
            }
            this.AddAction(gvars, new ItemAction("regenerate", 1));
        }
        public virtual void Death()
        {
            this.Dispose();
        }

        public abstract int Bounty();

        public void ChangeCurLives(double amount, Item killer = null)
        {
            curLives += amount;
            if (curLives < 0)
            {
                curLives = 0;
                Death();
                if (killer is Player p)
                {
                    p.IncreaseScore(this.Bounty());
                }
            }
            if (curLives > MaxLives)
            {
                curLives = MaxLives;
            }
            gvars?.AddProperty(Id, ItemProperties.Lives, curLives);
        }

        public override void CollideServer(Item collider, double angle)
        {
            if (collider is Shot s && s.CharacterId != this.Id)
            {
                this.ReceiveDamage(s.Damage, gvars.ItemsPlayers.ContainsKey(s.CharacterId) ? gvars.ItemsPlayers[s.CharacterId] : null);
            }
            if (collider is Character c)
            {
                this.ReceiveDamage(c.BodyDamage, c);
            }
        }

        public virtual void ReceiveDamage(double damage, Item killer)
        {
            if (!Immortal)
            {
                if (Shield > 0)
                {
                    if (Shield - damage < 0)
                    {
                        this.ChangeCurLives(-(damage - Shield), killer);
                    }
                    Shield -= damage;
                    gvars?.AddProperty(Id, ItemProperties.Shield, Shield);
                }
                else
                {
                    this.ChangeCurLives(-damage, killer);
                }
            }



        }

    }
}
