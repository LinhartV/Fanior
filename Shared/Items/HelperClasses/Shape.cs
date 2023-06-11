
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    public class Shape
    {
        public enum GeometryEnum { rectange = 0, circle = 1, image = 2 }

        public string FillColor { get; set; }

        public string StrokeColor { get; set; }

        public string FillColorEnemy { get; set; }

        public string StrokeColorEnemy { get; set; }

        public int StrokeWidth { get; set; }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }

        public GeometryEnum Geometry { get; set; } = GeometryEnum.circle;

        /*public Shape(string fillColor, string strokeColor, string fillColorEnemy, string strokeColorEnemy, int strokeWidth, int imageWidth, int imageHeight)
        {
           
            this.ImageHeight = imageHeight;
            this.ImageWidth = imageWidth;
            
            //this.Geometry = ShapeEnum.rectange;
            this.FillColor = fillColor;
            this.StrokeColor = strokeColor;
            this.StrokeWidth = strokeWidth;
            this.FillColorEnemy = fillColorEnemy;
            this.StrokeColorEnemy = strokeColorEnemy;
        }*/
        public Shape(string fillColor, string strokeColor, int strokeWidth, int imageWidth, int imageHeight, GeometryEnum shape, string fillColorEnemy = "", string strokeColorEnemy = "")
        {
            
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            
            this.Geometry = shape;
            this.FillColor = fillColor;
            this.StrokeColor = strokeColor;
            this.StrokeWidth = strokeWidth;
            this.FillColorEnemy = fillColorEnemy;
            this.StrokeColorEnemy = strokeColorEnemy;
            if (fillColorEnemy == "")
            {
                FillColorEnemy = fillColor;
            }
            if (StrokeColorEnemy == "")
            {
                StrokeColorEnemy = strokeColor;
            }
        }

        
        /*public Shape(ShapeEnum shape, Bitmap bmp, int width, int height)
        {
            this.shape = shape;
            this.fillColor = fillColor;
            this.strokeColor = strokeColor;
            this.strokeWidth = strokeWidth;
            this.width = width;
            this.height = height;
            this.fillColorEnemy = fillColorEnemy;
            this.strokeColorEnemy = strokeColorEnemy;
        }*/
    }
}
