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

        private Dictionary<string, (long, ItemAction)> actions = new();

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
                    if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.EveryTime)
                    {
                        try
                        {
                            actions[actionName].Item2.Action();
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                    else
                    {
                        if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.NotFirstTime)
                            actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                    }
                    actions.Remove(actionName);
                    if (tempActions[actionName].Item2.Repeat > 0 && tempActions[actionName].Item2.executionType != ItemAction.ExecutionType.OnlyFirstTime)
                    {
                        actions.Add(actionName, (now + tempActions[actionName].Item2.Repeat, tempActions[actionName].Item2));
                    }
                }
            }
        }
        public bool ChangeRepeatTime(int repeat, string actionName)
        {
            if (actions.ContainsKey(actionName))
            {
                actions[actionName].Item2.Repeat = repeat;
                return true;
            }
            else { return false; }
        }
        public void AddAction(ItemAction action, string name)
        {
            if (!actions.ContainsKey(name))
                actions.Add(name, (0, action));
        }
        public void DeleteAction(string name)
        {
            if (actions.ContainsKey(name))
                actions[name].Item2.Repeat = 0;
        }
        public void Dispose(Gvars gvars)
        {
            gvars.Items.Remove(this.Id);
            if (gvars.ItemsPlayers.ContainsKey(Id))
            {
                gvars.ItemsPlayers.Remove(this.Id);
            }
            if (gvars.ItemsStep.ContainsKey(Id))
            {
                gvars.ItemsStep.Remove(this.Id);
            }
        }
        public virtual void SetItemFromClient(Gvars gvars)
        {

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
            this.Id = gvars.Id++;
            gvars.Items.Add(Id, this);
        }
    }
}
