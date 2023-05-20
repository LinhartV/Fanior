using Fanior.Shared;
using Newtonsoft.Json;
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
        //actions of this item with information when to be execuded, accessed by name.
        private Dictionary<string,(long, ItemAction)> actions = new();



        /*public virtual void Collide(Item collider, double angle, params Globals.ActionsAtCollision[] actionsNotToPerform)
        {
            if (actionsNotToPerform.Contains(Globals.ActionsAtCollision.All))
                return;
            if (!actionsNotToPerform.Contains(Globals.ActionsAtCollision.MoveToContact) && collider.solid)
                ToolsItem.MoveToContact(this, collider, angle);
        }*/
        /// <summary>
        /// Actions to be executed in the current frame. Is action is supposed to repeat, it will be added again to the list.
        /// </summary>
        /// <returns>List of actions to be executed</returns>
        public void ExecuteActions(long now)
        {
            Dictionary<string, (long, ItemAction)> tempActions = new Dictionary<string, (long, ItemAction)>(actions);
            foreach (var actionName in tempActions.Keys)
            {
                if (tempActions[actionName].Item1 < now)
                {
                    if (actions[actionName].Item2.executeOnFirstTime)
                    {
                        try
                        {
                            actions[actionName].Item2.Action(this, actions[actionName].Item2);
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                    else 
                    {
                        actions[actionName].Item2.executeOnFirstTime = true;
                    }
                    actions.Remove(actionName);
                    if (tempActions[actionName].Item2.Repeat > 0)
                    {
                        actions.Add(actionName,(now + tempActions[actionName].Item2.Repeat, tempActions[actionName].Item2));
                    }
                }
            }
        }
        public void AddAction(ItemAction action, string name)
        {
            actions.Add(name, (0, action));
        }
        public void DeleteAction(string name)
        {
            actions[name].Item2.Repeat = 0;
        }
        public void Dispose()
        {

        }
        public virtual void SetItem(Gvars gvars)
        {
            this.Id = gvars.Id++;
            gvars.Items.Add(Id, this);
        }
        public Item() { }
        public Item(Gvars gvars, double x, double y, Shape shape, Mask mask, bool isVisible = true, bool justGraphics = false)
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
