using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    static class ToolsMath
    {
        static public double GetAngleFromLengts(double xlength, double ylength)
        {
            double newAngle = Math.Atan(Math.Abs(xlength / ylength));
            if (xlength > 0 && ylength < 0)
                newAngle = Math.PI - newAngle;
            else if (xlength < 0 && ylength < 0)
                newAngle = Math.PI + newAngle;
            else if (xlength < 0 && ylength > 0)
                newAngle = Math.PI * 2 - newAngle;
            return newAngle;
        }
    }
}
