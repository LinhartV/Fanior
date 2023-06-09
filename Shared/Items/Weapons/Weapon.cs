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
        /// <summary>
        /// Weapon of a character
        /// </summary>
        /// <param name="autoFire">Whether player must press key each time or it's automatic</param>
        /// <param name="reloadTime">number of frames it takes to reload</param>
        /// <param name="shotSpeed">What it says...</param>
        /// <param name="damage">Damage of the created shot</param>
        public Weapon(bool autoFire, double reloadTime, int shotSpeed, double damage)
        {
            this.reloadTime = reloadTime;
            this.shotSpeed = shotSpeed;
            this.damage = damage;
            this.autoFire = autoFire;
        }
        public bool reloaded = true;
        //number of frames it takes to reload
        public double reloadTime;
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
