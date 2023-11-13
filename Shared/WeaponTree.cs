using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCollections;

namespace Fanior.Shared
{
    public static class WeaponTree
    {
        public static List<List<Weapon>> weaponTree = new List<List<Weapon>>();
        public static void SetWeaponTree()
        {
            var root = new WeaponNode();




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
