using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Interface for items with lives
    /// </summary>
    public interface ILived
    {
        public double Regeneration { get; set; }
        public double MaxLives { get; set; }
        public void ChangeCurLives(double amount, Item killer, Gvars gvars);
        public double GetCurLives();
        protected void Death(Gvars gvars);

    }
}
