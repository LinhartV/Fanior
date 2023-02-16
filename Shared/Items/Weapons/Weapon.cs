using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public abstract class Weapon
    {
        public Weapon(bool autoFire, int reloadTime, int shotSpeed, float damage, int characterId)
        {
            this.reloadTime = reloadTime;
            this.shotSpeed = shotSpeed;
            this.damage = damage;
            this.characterId = characterId;
            this.autoFire = autoFire;
        }
        public bool reloaded = true;
        protected int reloadTime;
        protected int shotSpeed;
        protected float damage;
        protected int characterId;
        internal bool autoFire;

        public virtual void Fire()
        {            
            if(reloaded)
            {
                reloaded = false;
                //character.AddAction(null, "reload", reloadTime, null, (Item item) => { (item as Character).weapon.reloaded = true; });
                CreateShot();
            }
        }
        protected abstract void CreateShot();

    }
}
