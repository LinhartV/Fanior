using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeCollections;

namespace Fanior.Shared
{
    public static class WeaponTree
    {
        public static WeaponNode GetRoot()
        {
            //lvl2
            var burningBoulder = new WeaponNode(new BurningBoulderWeapon(true, 70, 13, 15, 100, "Fireball", "fireball.svg"));
            var bomber = new WeaponNode(new BombWeapon(true, 70, 10, 1, 50, "Bomber", "bombWeapon.svg"));
            //lvl1
            var flameBow = new WeaponNode(new FireBowWeapon(true, 35, 14, 7, 220, "Flame bow", "fireArrow.png"), bomber, burningBoulder);
            var crossbow = new WeaponNode(new CrossbowWeapon(true, 55, 25, 17, 70, "Crossbow", "crossbow.svg"));
            var slingshot = new WeaponNode(new SlingshotWeapon(true, 60, 17, 7, 120, "Slingshot", "slingshot.svg"));
            //root
            var root = new WeaponNode(new BasicWeapon(true, 30, 13, 5, 40, "Stone thrower", "none"), flameBow, crossbow, slingshot);
            return root;
        }

        public class WeaponNode : SerialTreeNode<WeaponNode>
        {
            // empty constructor is a type constraint imposed by the base class
            public WeaponNode() { }

            public WeaponNode(Weapon weapon, params WeaponNode[] children) : base(children)
            {
                this.Weapon = weapon;
            }

            public Weapon Weapon { get; set; }
        }
    }
}
