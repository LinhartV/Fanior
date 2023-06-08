using Fanior.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


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
        [JsonProperty]
        private Dictionary<string, (long, ItemAction)> actions = new();
        //actions of this item to be executed everyFrame
        [JsonProperty]
        private Dictionary<string, ItemAction> actionsEveryFrame = new();

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
        public void ExecuteActions(long now, Gvars gvars, bool procedeEveryFrameActions)
        {
            foreach (var action in actionsEveryFrame.Values)
            {
                LambdaActions.executeAction(action.ActionName, gvars, this.Id);
            }
            Dictionary<string, (long, ItemAction)> tempActions = new Dictionary<string, (long, ItemAction)>(actions);
            foreach (var actionName in tempActions.Keys)
            {
                if (tempActions[actionName].Item1 < now)
                {
                    if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.EveryTime)
                    {
                        try
                        {
                            LambdaActions.executeAction(actions[actionName].Item2.ActionName, gvars, this.Id);
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                    }
                    else if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.NotFirstTime)
                    {
                        actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                    }
                    actions.Remove(actionName);
                    if (tempActions[actionName].Item2.Repeat > 0 && tempActions[actionName].Item2.executionType != ItemAction.ExecutionType.StopExecuting)
                    {
                        actions.Add(actionName, (now + tempActions[actionName].Item2.Repeat, tempActions[actionName].Item2));
                    }
                    if (tempActions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime)
                    {
                        tempActions[actionName].Item2.Repeat = 0;
                        actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                    }
                }
            }
        }
        public bool ChangeRepeatTime(int repeat, string actionName)
        {
            if (actions.ContainsKey(actionName))
            {
                actions[actionName].Item2.Repeat = repeat;

                if (repeat <= Constants.FRAME_TIME && repeat > 0)
                {
                    actionsEveryFrame.Add(actionName, actions[actionName].Item2);
                    actions.Remove(actionName);
                }
                return true;
            }
            else if (actionsEveryFrame.ContainsKey(actionName))
            {
                if (repeat > Constants.FRAME_TIME)
                {
                    actions.Add(actionName, (0, actionsEveryFrame[actionName]));
                    actionsEveryFrame.Remove(actionName);
                }
                return true;
            }
            else { return false; }
        }
        /// <summary>
        /// Adds a new action to be executed
        /// </summary>
        /// <param name="action">ItemAction to add</param>
        /// <param name="rewrite">Whether to rewrite running action</param>
        public void AddAction(ItemAction action, bool rewrite = true)
        {
            this.AddAction(action, action.ActionName, rewrite);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">ItemAction to add</param>
        /// <param name="storeName">The name the action will be stored in the dictionary under</param>
        /// <param name="rewrite">Whether to rewrite running action</param>
        public void AddAction(ItemAction action, string storeName, bool rewrite = true)
        {
            if (action.Repeat > Constants.FRAME_TIME)
            {
                if (!actions.ContainsKey(storeName))
                    actions.Add(storeName, (0, action));
                else if (rewrite)
                {
                    actions.Remove(storeName);
                    actions.Add(storeName, (0, action));
                }
            }
            else
            {
                if (!actionsEveryFrame.ContainsKey(storeName))
                    actionsEveryFrame.Add(storeName, action);
                else if (rewrite)
                {
                    actionsEveryFrame.Remove(storeName);
                    actionsEveryFrame.Add(storeName, action);
                }
            }
        }

        public void DeleteAction(string name)
        {
            if (actions.ContainsKey(name))
            {
                actions[name].Item2.executionType = ItemAction.ExecutionType.StopExecuting;
                actions.Remove(name);
            }
            if (actionsEveryFrame.ContainsKey(name))
                actionsEveryFrame.Remove(name);
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
            if (!gvars.Items.ContainsKey(Id))
            {
                gvars.Items.Add(Id, this);
                gvars.Id++;
            }
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
