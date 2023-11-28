using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    //ještě předělám na interface. Něco bude kolo, něco čtverec, něco zvláštní maska, ale vždy budu prvně checkovat šířku a výšku.
    public class Mask
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Shape.GeometryEnum Geometry { get; }

        public Mask(double width, double height, Shape.GeometryEnum geometry)
        {
            this.Width = width;
            this.Height = height;
            Geometry = geometry;
        }
    }
}
