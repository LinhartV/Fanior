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
        public enum ExecutionType { EveryTime, NotFirstTime, OnlyFirstTime }
        //Action to be executed
        public Action Action { get; private set; }
        //whether the action will be repeated and if so, how often
        [JsonProperty]
        private int repeat;
        public ExecutionType executionType;

        public ItemAction(Action action, int repeat, ExecutionType executionType = ExecutionType.EveryTime)
        {
            Action = action;
            Repeat = repeat;
            this.executionType = executionType;
        }

        public int Repeat
        {
            get { return repeat; }
            set
            {
                if (value > 0)
                {
                    if (value <= Constants.FRAME_TIME)
                    {
                        repeat = Constants.FRAME_TIME;
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