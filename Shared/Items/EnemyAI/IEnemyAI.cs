using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public interface IEnemyAI
    {
        public void Control(Gvars gvars, Enemy enemy);
    }
}
