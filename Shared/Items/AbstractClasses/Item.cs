using Fanior.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fanior.Shared
{
    public abstract class Item
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Id { get; set; }
        public bool Solid { get; set; }
        public bool IsVisible { get; set; }
        public Mask Mask { get; set; }
        public Shape Shape { get; set; }




        /*public virtual void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            if (actionsNotToPerform.Contains(Globals.ActionsAtCollision.All))
                return;
            if (!actionsNotToPerform.Contains(Globals.ActionsAtCollision.MoveToContact) && collider.solid)
                ToolsItem.MoveToContact(this, collider, angle);
        }*/
        public void Dispose()
        {
            
        }
        public virtual void SetItem(Gvars gvars)
        {
            this.Id = gvars.Id++;
            gvars.Items.Add(Id, this);
        }
        public Item() {}
        public Item(Gvars gvars, double x, double y, Shape shape ,Mask mask, bool isVisible = true, bool justGraphics = false)
        {
            this.Shape = shape;
            this.Mask = mask;
            this.X = x;
            this.Y = y;
            this.IsVisible = isVisible;
            Solid = !justGraphics;
          

        }
    }
}
