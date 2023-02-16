

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Fanior.Shared.Gvars;

namespace Fanior.Shared
{
    static class ToolsItem
    {
        /*public static bool[,] SetMask(int rad)
         {
             bool[,] Mask = new bool[2 * rad, 2 * rad];
             int y;
             for (double j = 0; j < 2 * Math.PI; j += 0.001)
             {
                 y = (int)Math.Floor(Math.Cos(j) * (rad));
                 for (int i = -y; i < y; i++)
                 {
                     Mask[i + rad, (int)Math.Floor(Math.Sin(j) * (rad)) + rad] = true;
                 }
             }
             return Mask;
         }
         public static bool[,] SetMask(int width, int height)
         {
             bool[,] Mask = new bool[height, width];
             for (int i = 0; i < height; i++)
             {
                 for (int j = 0; j < width; j++)
                 {
                     Mask[i, j] = true;
                 }
             }
             return Mask;
         }
         public static bool[,] SetMask(string path)
         {
             Task.Delay(100);
             bool[,] Mask = new bool[5, 5];

             try
             {
                  Image<Rgba32> image = new Image<Rgba32>(64, 64);
                  image[0, 0] = new Rgba32(100, 200, 150, 50);
                  image.SaveAsBmp("justbitmap.bmp");
              for (int i = 0; i < width; i++)
              {
                  for (int j = 0; j < height; j++)
                  {
                      Mask[i, j] = true;
                  }
              }
             }
             catch (Exception e)
             {
                 //Index.JSR.InvokeVoidAsync("Alert", e.ToString());
             }
             return Mask;
         }*/



        /*public static double GetCollideAngle(double x1collide, double y1collide, int rad)
        {
            if (x1collide - Mask.GetLength(1) / 2 == 0)
            {
                if (Mask.GetLength(0) / 2 - y1collide > 0)
                    return 3 * Math.PI / 2;
                else
                    return Math.PI / 2;
            }
            else if (y1collide - Mask.GetLength(0) / 2 == 0)
            {
                if (Mask.GetLength(1) / 2 - x1collide > 0)
                    return 0;
                else
                    return Math.PI;
            }
            double collideAngle = Math.Atan((double)Math.Abs(x1collide - Mask.GetLength(1) / 2) / (double)Math.Abs(Mask.GetLength(0) / 2 - y1collide));
            if (x1collide > Mask.GetLength(1) / 2 && y1collide < Mask.GetLength(0) / 2)
            {
                collideAngle = Math.Atan((double)(x1collide - Mask.GetLength(1) / 2) / (double)(Mask.GetLength(0) / 2 - y1collide));
            }
            else if (x1collide < Mask.GetLength(1) / 2 && y1collide < Mask.GetLength(0) / 2)
            {
                collideAngle += Math.PI / 2;
            }
            else if (x1collide < Mask.GetLength(1) / 2 && y1collide > Mask.GetLength(0) / 2)
            {
                collideAngle += Math.PI;
            }
            else if (x1collide > Mask.GetLength(1) / 2 && y1collide > Mask.GetLength(0) / 2)
            {
                collideAngle += 3 * Math.PI / 2;
            }
            return collideAngle;
        }*/
        public static double GetCollideAngle(double x1collide, double y1collide, bool[,] Mask)
        {
            if (x1collide - Mask.GetLength(1) / 2 == 0)
            {
                if (Mask.GetLength(0) / 2 - y1collide > 0)
                    return 3 * Math.PI / 2;
                else
                    return Math.PI / 2;
            }
            else if (y1collide - Mask.GetLength(0) / 2 == 0)
            {
                if (Mask.GetLength(1) / 2 - x1collide > 0)
                    return 0;
                else
                    return Math.PI;
            }
            double collideAngle = Math.Atan((double)Math.Abs(x1collide - Mask.GetLength(1) / 2) / (double)Math.Abs(Mask.GetLength(0) / 2 - y1collide));
            if (x1collide > Mask.GetLength(1) / 2 && y1collide < Mask.GetLength(0) / 2)
            {
                collideAngle = Math.Atan((double)(x1collide - Mask.GetLength(1) / 2) / (double)(Mask.GetLength(0) / 2 - y1collide));
            }
            else if (x1collide < Mask.GetLength(1) / 2 && y1collide < Mask.GetLength(0) / 2)
            {
                collideAngle += Math.PI / 2;
            }
            else if (x1collide < Mask.GetLength(1) / 2 && y1collide > Mask.GetLength(0) / 2)
            {
                collideAngle += Math.PI;
            }
            else if (x1collide > Mask.GetLength(1) / 2 && y1collide > Mask.GetLength(0) / 2)
            {
                collideAngle += 3 * Math.PI / 2;
            }
            return collideAngle;
        }
        //--------------------------------
        /*static public void MoveToContact(Item thisItem, Item collider, double angle)
        {
            if (angle < Math.PI / 2)
            {
                thisItem.z = collider.z - thisItem.coords.size.depth;
            }
            else if (angle < Math.PI)
            {
                thisItem.x = collider.x - thisItem.coords.size.width;
            }
            else if (angle < 3 * Math.PI / 2)
            {
                thisItem.z = collider.z + collider.coords.size.depth;
            }
            else
            {
                thisItem.x = collider.x + collider.coords.size.width;
            }
        }*/
        /*
        static public (List<double>, bool) CheckCollision(Item item, float xspeed, float zspeed)
        {
            bool fromRight;
            bool fromLeft;
            bool fromBottom;
            bool fromTop;
            double angle;
            List<double> angles = new();
            List<(Item, double)> possibleCollisions = new();
            foreach (Item myItem in Globals.itemsGameObjects)
            {
                /*
                fromLeft = x && ((item.x + xspeed < myItem.x && item.x + xspeed + item.coords.size.width > myItem.x) || ((item.x + xspeed > myItem.x) && (item.x + xspeed < myItem.x + myItem.coords.size.width)) || ((item.x > myItem.x + myItem.coords.size.width) && (item.x + xspeed + item.coords.size.width < myItem.x)));
                fromRight = x && ((item.x + xspeed < myItem.x && item.x + xspeed + item.coords.size.width > myItem.x + myItem.coords.size.width) || ((item.x + item.coords.size.width + xspeed > myItem.x) && (myItem.x + myItem.coords.size.width > item.x + xspeed + item.coords.size.width)) || ((item.x + item.coords.size.width < myItem.x) && (item.x + item.coords.size.width + xspeed > myItem.x + myItem.coords.size.width)));
                fromBottom = !x && ((item.z + zspeed < myItem.z && item.z + zspeed + item.coords.size.depth > myItem.z + myItem.coords.size.depth) || ((item.z + item.coords.size.depth + zspeed < myItem.z + myItem.coords.size.depth) && (item.z + zspeed + item.coords.size.depth > myItem.z)) || ((item.z + item.coords.size.depth < myItem.z) && (item.z + zspeed > myItem.z + myItem.coords.size.depth)));
                fromTop = !x && ((item.z + zspeed < myItem.z && item.z + zspeed + item.coords.size.depth > myItem.z + myItem.coords.size.depth) || ((item.z + zspeed > myItem.z) && (item.z + zspeed < myItem.z + myItem.coords.size.depth)) || ((item.z > myItem.z) && (item.z + zspeed < myItem.z - item.coords.size.depth)));
                *//*

                fromLeft = xspeed > 0 && (((item.x + xspeed + item.coords.size.width > myItem.x) && (item.x + xspeed + item.coords.size.width < myItem.x + myItem.coords.size.width)) || ((item.x + item.coords.size.width <= myItem.x) && (item.x + xspeed >= myItem.x + myItem.coords.size.width)));
                fromRight = xspeed < 0 && (((item.x + xspeed > myItem.x) && (myItem.x + myItem.coords.size.width > item.x + xspeed)) || ((item.x >= myItem.x + myItem.coords.size.width) && (item.x + item.coords.size.width + xspeed <= myItem.x)));
                fromBottom = zspeed > 0 && (((item.z + item.coords.size.depth + zspeed > myItem.z) && (item.z + zspeed + item.coords.size.depth < myItem.z + myItem.coords.size.depth)) || ((item.z + item.coords.size.depth <= myItem.z) && (item.z + item.coords.size.depth + zspeed >= myItem.z + myItem.coords.size.depth)));
                fromTop = zspeed<0 && (((item.z + zspeed < myItem.z + myItem.coords.size.depth) && (item.z + zspeed > myItem.z)) || ((item.z >= myItem.z + myItem.coords.size.depth) && (item.z + zspeed <= myItem.z - item.coords.size.depth)));

                angle = -1;
                if (myItem.id != item.id && (fromLeft || fromRight || fromBottom || fromTop))
                {
                    if (fromLeft && item.z < myItem.z + myItem.coords.size.depth && item.z > myItem.z - item.coords.size.depth)
                    {
                        angle = (float)Math.PI / 2;
                    }
                    if (fromBottom && item.x < myItem.x + myItem.coords.size.width && item.x > myItem.x - item.coords.size.width)
                    {
                        angle = 0;
                    }
                    if (fromRight && item.z < myItem.z + myItem.coords.size.depth && item.z > myItem.z - item.coords.size.depth)
                    {
                        angle = (float)Math.PI * 3 / 2;
                    }
                    if (fromTop && item.x < myItem.x + myItem.coords.size.width && item.x > myItem.x - item.coords.size.width)
                    {
                        angle = (float)Math.PI;
                    }
                    if (angle == -1)
                        continue;

                    /*if (xspeed > 0 && zspeed >= 0)
                        angle = (Math.Atan(zspeed / xspeed) % (2 * Math.PI));
                    else if (xspeed <= 0 && zspeed > 0)
                        angle = Math.PI / 2 + (Math.Atan(-xspeed/ zspeed) % (2 * Math.PI));
                    else if (xspeed < 0 && zspeed <= 0)
                        angle = Math.PI + (Math.Atan(zspeed / xspeed) % (2 * Math.PI));
                    else if (xspeed <= 0 && zspeed < 0)
                        angle = Math.PI*3/2 + (Math.Atan(xspeed / zspeed) % (2 * Math.PI));*//*
                    possibleCollisions.Add((myItem, angle));
                    if (!angles.Contains(angle) && angle != -1 && myItem.solid)
                        angles.Add(angle);
                }
            }
            if(possibleCollisions.Count>1)
            {
                if(xspeed>0)
                {
                    float min = possibleCollisions[0].Item1.x;
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if(possibleCollisions[i].Item1.x < min)
                        {
                            min = possibleCollisions[i].Item1.x;
                        }
                    }
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.x > min)
                        {
                            possibleCollisions.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else if(xspeed<0)
                {
                    float min = possibleCollisions[0].Item1.x;
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.x > min)
                        {
                            min = possibleCollisions[i].Item1.x;
                        }
                    }
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.x < min)
                        {
                            possibleCollisions.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else if (zspeed < 0)
                {
                    float min = possibleCollisions[0].Item1.z;
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.z > min)
                        {
                            min = possibleCollisions[i].Item1.z;
                        }
                    }
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.z < min)
                        {
                            possibleCollisions.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else if (zspeed > 0)
                {
                    float min = possibleCollisions[0].Item1.z;
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.z < min)
                        {
                            min = possibleCollisions[i].Item1.z;
                        }
                    }
                    for (int i = 0; i < possibleCollisions.Count; i++)
                    {
                        if (possibleCollisions[i].Item1.z > min)
                        {
                            possibleCollisions.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            foreach ((Item, double) myItem in possibleCollisions)
            {
                
                //item.Collide(myItem.Item1, myItem.Item2);
                //myItem.Item1.Collide(item, myItem.Item2, ActionsAtCollision.MoveToContact);
            }
            if (angles.Count > 0)
                return (angles, true);
            return (angles, false);
        }
    */
    }
}
