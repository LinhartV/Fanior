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

        public enum ExecutionType { EveryTime, NotFirstTime, OnlyFirstTime, StopExecuting }
        //Action to be executed
        public string ActionName { get; private set; }
        //whether the action will be repeated and if so, how long it take between each repetition (number of frames)
        [JsonProperty]
        private double repeat;
        public ExecutionType executionType;

        public ItemAction(string actionName, double repeat, ExecutionType executionType = ExecutionType.EveryTime)
        {
            ActionName = actionName;
            Repeat = repeat;
            this.executionType = executionType;
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