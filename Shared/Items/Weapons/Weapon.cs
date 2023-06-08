using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Weapon
    {
        public Weapon(bool autoFire, int reloadTime, int shotSpeed, double damage)
        {
            this.reloadTime = reloadTime;
            this.shotSpeed = shotSpeed;
            this.damage = damage;
            this.autoFire = autoFire;
        }
        public bool reloaded = true;
        public int reloadTime;
        [JsonProperty]
        protected int shotSpeed;
        [JsonProperty]
        protected double damage;
        public int characterId;
        public bool autoFire;

        public virtual void Fire(Gvars gvars)
        {
            CreateShot(gvars);
        }
        protected abstract void CreateShot(Gvars gvars);

    }
}
