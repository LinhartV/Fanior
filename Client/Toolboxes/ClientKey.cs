using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Client
{
    class ClientKey
    {
        public string Key { get; set; }
        public bool Pressed { get; set; }
        public bool Active { get; set; }


        public ClientKey(string key)
        {
            this.Key = key;
        }
    }
}
