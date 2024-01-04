using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public interface IHitReaction
    {

        public void React(Gvars gvars, int characterId, int shooterId);
    }
}
