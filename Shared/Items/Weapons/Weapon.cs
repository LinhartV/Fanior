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
            this.ReloadTime = reloadTime;
            this.WeaponSpeed = shotSpeed;
            this.Damage = damage;
            this.AutoFire = autoFire;
        }
        public bool Reloaded { get; set; } = true;
        //number of frames it takes to reload
        public double ReloadTime { get; set; }
        public double WeaponSpeed { get; set; }
        public double Damage { get; set; }
        public int CharacterId { get; set; }
        public bool AutoFire { get; set; }

        public virtual void Fire(Gvars gvars)
        {
            CreateShot(gvars);
        }
        protected virtual void CreateShot(Gvars gvars) { }

    }
}
