using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Shared
{
    /// <summary>
    /// Actions that can be assigned to Items - controlled by game itself.
    /// </summary>
    public class ItemAction
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ExecutionType {
            ///<summary>Action executes immidiatelly and repeats infinitely</summary>
            EveryTime,
            ///<summary>Action repeats infinitely, but doesn't execute first time</summary>
            NotFirstTime,
            ///<summary>Action executes only once after duration time runs out (set duration to 0 to happen at the start of the very next frame)</summary>
            OnlyFirstTime,
            ///<summary>Stops execution of selected action</summary>
            StopExecuting
        }
        //Action to be executed
        public string ActionName { get; private set; }
        //Parameters
        public object[] Parameters { get; private set; }
        //whether the action will be repeated and if so, how long it take between each repetition (number of frames)
        [JsonProperty]
        private double repeat;
        [JsonProperty]
        public bool ClientAction { get; set; }
        public ExecutionType executionType;
        /// <summary>
        /// Creates ItemAction - any possible action can be assigned to item
        /// </summary>
        /// <param name="actionName">Name of an action from LambdaActions</param>
        /// <param name="repeat">How many frames to wait between executions (0 = only first time, 1 = each frame)</param>
        /// <param name="clientAction">Whether this action should be executed on client side</param>
        /// <param name="executionType"></param>
        public ItemAction(string actionName, double repeat, ExecutionType executionType = ExecutionType.EveryTime, bool clientAction = false, params object[] parameters)
        {
            this.ClientAction = clientAction;
            ActionName = actionName;
            Repeat = repeat;
            this.executionType = executionType;
            this.Parameters = parameters;
        }

        public double Repeat
        {
            get { return repeat; }
            set
            {
                if (value > 0)
                {
                    if (value <= 1)
                    {
                        repeat = 1;
                    }
                    else
                        repeat = value;
                }
                else
                {
                    repeat = 0;
                }
            }
        }
    }
}