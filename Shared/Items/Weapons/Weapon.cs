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
        /// <param name="reloadTimeCoef">number of frames it takes to reload</param>
        /// <param name="shotSpeedCoef">What it says...</param>
        /// <param name="damageCoef">Damage of the created shot</param>
        public Weapon(bool autoFire, double reloadTimeCoef, int shotSpeedCoef, double damageCoef, double lifeSpan, string name, string imageName)
        {
            this.ReloadTimeCoef = reloadTimeCoef;
            this.WeaponSpeedCoef = shotSpeedCoef;
            this.DamageCoef = damageCoef;
            this.AutoFire = autoFire;
            Name = name;
            LifeSpan = lifeSpan;
            ImageName = imageName;
        }
        public bool Reloaded { get; set; } = true;
        //number of frames it takes to reload
        public double ReloadTimeCoef { get; set; }
        [JsonProperty]
        public double WeaponSpeedCoef { get; set; }
        [JsonProperty]
        public double DamageCoef { get; set; }
        public double LifeSpan { get; set; }
        public int CharacterId { get; set; }
        public bool AutoFire { get; set; }
        public string Name { get; private set; }
        public string ImageName { get; private set; }

        public virtual void Fire(Gvars gvars)
        {
            CreateShot(gvars);
        }
        protected virtual void CreateShot(Gvars gvars) { }

    }
}
