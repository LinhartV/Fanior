using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Fanior.Shared
{
    /// <summary>
    /// Object that actions can be assigned to
    /// </summary>
    public class ActionHandler
    {
        /// <summary>
        /// Enum for determining what to do with action with the same name
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum RewriteEnum { Ignore = 0, Rewrite = 1, AddNew = 2 }
        private int storeIndex = 0;
        //actions of this item with information when to be execuded, accessed by name.
        [JsonProperty]
        protected Dictionary<string, (double, ItemAction)> actions = new();
        //actions of this item to be executed everyFrame
        [JsonProperty]
        protected Dictionary<string, ItemAction> actionsEveryFrame = new();


        /// <summary>
        /// Actions to be executed in the current frame. Is action is supposed to repeat, it will be added again to the list.
        /// Due to possible differences in duration of particular frames, actions will be executed be number of frames, not real time
        /// </summary>
        /// <param name="id">Set -1 for gvars</param>
        public void ExecuteActions(double now, Gvars gvars, int id = -1)
        {
            foreach (var action in actionsEveryFrame.Values)
            {
                LambdaActions.ExecuteActions(action.ActionName, gvars, id, action.Parameters);
            }

            Dictionary<string, (double, ItemAction)> tempActions = new Dictionary<string, (double, ItemAction)>(actions);
            foreach (var actionName in tempActions.Keys)
            {
                if (tempActions[actionName].Item1 < now)
                {
                    if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.EveryTime || (actions[actionName].Item2.Repeat == 0 && actions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime))
                    {
                        LambdaActions.ExecuteActions(actions[actionName].Item2.ActionName, gvars, id, actions[actionName].Item2.Parameters);
                    }
                    else if (actions[actionName].Item2.executionType == ItemAction.ExecutionType.NotFirstTime)
                    {
                        actions[actionName].Item2.executionType = ItemAction.ExecutionType.EveryTime;
                    }
                    actions.Remove(actionName);
                    if (tempActions[actionName].Item2.Repeat > 0 && tempActions[actionName].Item2.executionType != ItemAction.ExecutionType.StopExecuting)
                    {
                        actions.Add(actionName, (now + (double)(tempActions[actionName].Item2.Repeat * Constants.GAMEPLAY_FRAME_TIME), tempActions[actionName].Item2));
                        if (tempActions[actionName].Item2.executionType == ItemAction.ExecutionType.OnlyFirstTime)
                        {
                            tempActions[actionName].Item2.Repeat = 0;
                        }
                    }

                }
            }
        }
        public void DeleteNonClientActions()
        {
            var tempList = new Dictionary<string, ItemAction>(actionsEveryFrame);

            foreach (var action in tempList.Keys)
            {
                if (!actionsEveryFrame[action].ClientAction)
                {
                    DeleteAction(actionsEveryFrame[action].ActionName);
                }
            }

            Dictionary<string, (double, ItemAction)> tempActions = new Dictionary<string, (double, ItemAction)>(actions);
            foreach (var actionName in tempActions.Keys)
            {
                if (!actions[actionName].Item2.ClientAction)
                {
                    DeleteAction(actions[actionName].Item2.ActionName);
                }
            }
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
        public void AddAction(Gvars gvars, ItemAction action, long delay = 0, RewriteEnum rewrite = RewriteEnum.Rewrite)
        {
            this.AddAction(gvars, action, action.ActionName, delay, rewrite);
        }
        /// <summary>
        /// Adds a new action to be executed
        /// </summary>
        /// <param name="action">ItemAction to add</param>
        /// <param name="storeName">The name the action will be stored in the dictionary under</param>
        /// <param name="rewrite">Whether to rewrite running action</param>
        public void AddAction(Gvars gvars, ItemAction action, string storeName, long delay = 0, RewriteEnum rewrite = RewriteEnum.Rewrite)
        {
            if (gvars.server || action.ClientAction == true)
            {
                if (action.Repeat > 1 || action.Repeat == 0)
                {
                    if (!actions.ContainsKey(storeName))
                        actions.Add(storeName, (delay, action));
                    else if (rewrite == RewriteEnum.Rewrite)
                    {
                        actions.Remove(storeName);
                        actions.Add(storeName, (delay, action));
                    }
                    else if (rewrite == RewriteEnum.AddNew)
                    {
                        actions.Add(storeName + storeIndex.ToString(), (delay, action));
                        storeIndex++;
                    }
                }
                else
                {
                    if (!actionsEveryFrame.ContainsKey(storeName))
                        actionsEveryFrame.Add(storeName, action);
                    else if (rewrite == RewriteEnum.Rewrite)
                    {
                        actionsEveryFrame.Remove(storeName);
                        actionsEveryFrame.Add(storeName, action);
                    }
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
    }
}
