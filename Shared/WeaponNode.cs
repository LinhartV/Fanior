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
