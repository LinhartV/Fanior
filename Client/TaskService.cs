
using Fanior.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanior.Client
{
    public class Return
    {
        public Gvars ReturnGvars { get; set; }
    }
    public class TaskService
    {
        public event EventHandler<Return> ReturnGvars;
        public async Task TryMethod(int parameter)
        {
            while (true)
            {
                parameter *= 2;
                Console.WriteLine(parameter);
                await Task.Delay(1000);
                //Num.Invoke(this, new Return() { Progress = parameter });
            }
        }
        public async Task<long> Frame(Gvars gvars, long now)
        {
            try
            {
                //ToolsGame.ProceedFrame(gvars, now);
                return now;
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
