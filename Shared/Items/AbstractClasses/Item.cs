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
        //just for graphics
        public double VirtualX { get; set; }
        public double VirtualY { get; set; }
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

        public abstract void Collide(Item collider, double angle, Gvars gvars);
        /// <summary>
        /// Actions to be executed in the current frame. Is action is supposed to repeat, it will be added again to the list.
        /// Due to possible differences in duration of particular frames, actions will be executed be number of frames, not real time
        /// </summary>
        public void ExecuteActions(long now, Gvars gvars, bool server)
        {
            foreach (var action in actionsEveryFrame.Values)
            {
                if (!server && action.ActionName == "move")
                {
                    continue;
                }
                LambdaActions.executeAction(action.ActionName, gvars, this.Id);
            }

            Dictionary<string, (long, ItemAction)> tempActions = new Dictionary<string, (long, ItemAction)>(actions);
            foreach (var actionName in tempActions.Keys)
            {
                if (tempActions[actionName].Item1 < now)
                {
                    if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.EveryTime || (actions[actionName].Item2.Repeat == 0 && actions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime))
                    {
                        LambdaActions.executeAction(actions[actionName].Item2.ActionName, gvars, this.Id);
                    }
                    else if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.NotFirstTime)
                    {
                        actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                    }
                    actions.Remove(actionName);
                    if (tempActions[actionName].Item2.Repeat > 0 && tempActions[actionName].Item2.executionType != ItemAction.ExecutionType.StopExecuting)
                    {
                        actions.Add(actionName, (now + (long)(tempActions[actionName].Item2.Repeat * Constants.FRAME_TIME), tempActions[actionName].Item2));
                        if (tempActions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime)
                        {
                            tempActions[actionName].Item2.Repeat = 0;
                            actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                        }
                    }

                }
            }
        }

        //Idea that actions will be reversed, then I would add pending actions and then I would execute them several times to reach current state
        //This way I would execute pending actions in the past, when the actually happened... didn't work...
        /// <summary>
        /// Invokes playerActions
        /// </summary>
        public void SetActions(long now, Gvars gvars, int delay, List<(PlayerActions.PlayerActionsEnum, bool)> actionMethodNames)
        {
            /*for (int i = 0; i < frames; i++)
            {
                foreach (var action in actionsEveryFrame.Values)
                {
                    LambdaActions.executeAntiAction(action.ActionName, gvars, this.Id);
                }
            }*/
            foreach (var action in actionMethodNames)
            {
                PlayerActions.InvokeAction(action.Item1, action.Item2, Id, gvars, delay);
            }
            /*for (int i = 0; i < frames; i++)
            {
                foreach (var action in actionsEveryFrame.Values)
                {
                    LambdaActions.executeAction(action.ActionName, gvars, this.Id);
                }
            }*/
        }

        public bool ChangeRepeatTime(double repeat, string actionName)
        {
            if (actions.ContainsKey(actionName))
            {
                actions[actionName].Item2.Repeat = repeat;

                if (repeat <= 1 && repeat > 0)
                {
                    actionsEveryFrame.Add(actionName, actions[actionName].Item2);
                    actions.Remove(actionName);
                }
                return true;
            }
            else if (actionsEveryFrame.ContainsKey(actionName))
            {
                if (repeat > 1)
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
        /// <param name="delay">"When to execute the action. Gotta be (now + delay) "</param>
        public void AddAction(ItemAction action, long delay = 0, bool rewrite = true)
        {
            this.AddAction(action, action.ActionName, delay, rewrite);
        }
        /// <summary>
        /// Adds a new action to be executed
        /// </summary>
        /// <param name="action">ItemAction to add</param>
        /// <param name="storeName">The name the action will be stored in the dictionary under</param>
        /// <param name="rewrite">Whether to rewrite running action</param>
        public void AddAction(ItemAction action, string storeName, long delay = 0, bool rewrite = true)
        {
            if (action.Repeat > 1 || action.Repeat == 0)
            {
                if (!actions.ContainsKey(storeName))
                    actions.Add(storeName, (delay, action));
                else if (rewrite)
                {
                    actions.Remove(storeName);
                    actions.Add(storeName, (delay, action));
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
        public virtual void Dispose(Gvars gvars)
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
            if (mask == null)
            {
                Mask = new Mask(shape.ImageWidth, shape.ImageHeight, shape.Geometry);
            }
            this.X = x;
            this.Y = y;
            VirtualX = x;
            VirtualY = y;
            this.IsVisible = isVisible;
            Solid = !justGraphics;
            this.Id = gvars.Id++;
            gvars.Items.Add(Id, this);
        }
    }
}
