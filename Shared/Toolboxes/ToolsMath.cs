using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public static class ToolsMath
    {
        /// <summary>
        /// Calculate angle from x and y length (0=right)
        /// </summary>
        /// <param name="xlength"></param>
        /// <param name="ylength"></param>
        /// <returns></returns>
        static public double GetAngleFromLengts(double xlength, double ylength)
        {
            double newAngle;
            if (xlength == 0)
            {
                newAngle = Math.PI / 2;
            }
            else
            {
                newAngle = Math.Atan(Math.Abs(ylength / xlength));
            }
            if (xlength >= 0 && ylength <= 0)
                newAngle = 2 * Math.PI - newAngle;
            else if (xlength <= 0 && ylength <= 0)
                newAngle = Math.PI + newAngle;
            else if (xlength <= 0 && ylength >= 0)
                newAngle = Math.PI - newAngle;
            return newAngle;
        }
        /// <summary>
        /// Calculate angle between two items
        /// </summary>
        /// <returns></returns>
        static public double GetAngleFromLengts(Item it1, Item it2)
        {

            return GetAngleFromLengts(it2.X - it1.X, it1.Y - it2.Y);
        }
        /// <summary>
        /// Changes speed from polar coordinates to cartesian
        /// </summary>
        /// <returns>width and height</returns>
        public static (double, double) PolarToCartesian(double angle, double size)
        {
            return (size * Math.Cos(angle), size * Math.Sin(angle));
        }
        /// <summary>
        /// Converts time in seconds to number of frames
        /// </summary>
        /// <param name="time">Number of seconds</param>
        /// <returns>Number of frames</returns>
        public static double TimeToFrames(double time)
        {
            return time * 1000 / Constants.GAMEPLAY_FRAME_TIME;
        }


    }
}
