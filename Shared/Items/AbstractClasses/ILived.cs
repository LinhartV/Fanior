using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public interface ILived
    {
        public double MaxLives { get; set; }
        public double CurLives { get; set; }
        protected void Death(Gvars gvars);

    }
}
