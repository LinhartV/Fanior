using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public static class ToolsMath
    {
        static public double GetAngleFromLengts(double xlength, double ylength)
        {
            double newAngle = Math.Atan(Math.Abs(ylength / xlength));
            if (xlength >= 0 && ylength <= 0)
                newAngle = 2 * Math.PI - newAngle;
            else if (xlength <= 0 && ylength <= 0)
                newAngle = Math.PI + newAngle;
            else if (xlength <= 0 && ylength >= 0)
                newAngle = Math.PI - newAngle;
            return newAngle;
        }
        /// <summary>
        /// Changes speed from polar coordinates to cartesian
        /// </summary>
        /// <returns>width and height</returns>
        public static (double, double) PolarToCartesian(double angle, double size)
        {
            return (size * Math.Cos(angle), size * Math.Sin(angle));
        }
    }
}
