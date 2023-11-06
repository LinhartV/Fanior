using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Fanior.Shared;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Fanior.Client.Pages
{
    public partial class Index
    {
        #region Variables
        //list of keys that are pressed in the current frame
        List<string> pressedKeys = new List<string>();
        //id of this connection
        int id = 0;
        //name of the player
        public string name;
        //number of score needed for next level
        public int nextLevel = 50;
        //intro animation
        private double opacity = 0;
        private int zindex = -100;
        //just for testing
        public int counter = 0;
        public int counter2 = 0;
        private string info = "0";
        //this player
        private Player player;
        //game variables for this session
        private Gvars gvars;
        //dimensions of the window
        private int width = 500;
        private int height = 500;
        //reference to canvas
        public ElementReference mySvg;
        public HubConnection hubConnection;
        long currentMessageId = 0;
        //action, down, now
        public List<(PlayerActions.PlayerActionsEnum, bool)> myActions = new();
        private DotNetObjectReference<Index> selfReference;
        //number of frames that passed during lag between server and client
        private int delay;
        //When the client was connected
        double startTime = 0;
        private readonly object actionLock = new object();
        private readonly object frameLock = new object();
        //0 = during connection, 1 = died and need to reset, 2 = really first connect
        int firstConnect = 2;
        int sendMessageId = 0;
        //measuring frame
        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        Stopwatch pingWatch = new Stopwatch();
        System.Threading.Timer timer;
        //animated movement
        bool animEnd = false;
        //dicionary of actions (pressed or released) waiting for server to confirm with time of execution
        Dictionary<int, (PlayerActions.PlayerActionsEnum, bool, double)> actions = new();
        int actionId = 0;
        #endregion




        #region Frame
        /*protected Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (stoppingToken.IsCancellationRequested == false && animEnd == false)
                {
                    if (movementCounter < 5)
                    {
                        Movement();
                    }
                    await Task.Delay((1), stoppingToken);
                }
            });
        }*/

        /* int movementCounter = 0;
         private void Movement()
         {
             lock (actionLock)
             {
                 foreach (var item in coordinates)
                 {
                     if (movementCounter < 5)
                     {
                         gvars.Items[item.Key].VirtualX += (item.Value.Item1 - gvars.Items[item.Key].X) / 4;
                         gvars.Items[item.Key].VirtualY += (item.Value.Item2 - gvars.Items[item.Key].Y) / 4;
                     }
                 }
                 movementCounter++;
             }
         }*/
        double avg;
        double serverTime;
        bool reping = false;
        double frameDuration = 0;
        private async Task Frame()
        {
            try
            {
                /*if (ping)
                {
                    Ping();
                    ping = false;
                }
                if (reping)
                {
                    reping = false;
                    Console.WriteLine("server time: " + (serverTime - sw.Elapsed.TotalMilliseconds));
                    pingWatch.Stop();
                    Console.WriteLine("total ping: " + pingWatch.Elapsed.TotalMilliseconds);
                    pingWatch.Reset();
                }*/

                await hubConnection.SendAsync("ExecuteList", gvars.GameId, this.id, player.Angle, sendMessageId);

                /* sendMessageId++;
                 gvars.PlayerActions[id] = new List<(PlayerActions.PlayerActionsEnum, bool)>(myActions);
                 myActions.Clear();
                 lock (frameLock)
                 {
                     gvars.PercentageOfFrame = ToolsSystem.GetPercentageOfFrame(frameDuration, sw.Elapsed.TotalMilliseconds);
                     if (gvars.PercentageOfFrame == -1)
                     {
                         //hubConnection.SendAsync("SendGvars", gvars.GameId);
                     }

                     frameDuration = sw.Elapsed.TotalMilliseconds;
                     ToolsGame.ProceedFrame(gvars, gvars.GetNow(), delay, false);
                     if (player.Score >= nextLevel)
                     {
                         //Bonus
                         nextLevel = nextLevel * 5 / 2;
                     }
                     gvars.PlayerActions[id].Clear();
                 }*/


            }
            catch (Exception e)
            {
                //JS.InvokeVoidAsync("Alert", e.Message);
                throw;
            }
        }
        private void GetAllItems(string itemsJson)
        {
            gvars.Items = JsonConvert.DeserializeObject<Dictionary<int, Item>>(itemsJson, ToolsSystem.jsonSerializerSettings);
            player = gvars.Items[this.id] as Player;
        }
        private void ExecuteList(double now, long messageId,/* string actionMethodNamesJson, Dictionary<int, double> playerInfo,*/ string itemsToCreateJson, List<int> itemsToDestroy, Dictionary<int, Dictionary<Item.ItemProperties, double>> info)
        {
            /*if (counter == 0)
            {
                counter = (int)messageId;
            }
            if (messageId != counter)
            {
                counter2++;
            }*/
            if (player == null || !gvars.ready)
            {
                return;
            }
            //delay = (int)(now - gvars.GetNow());
            //int frames = (int)Math.Floor((double)delay / Constants.FRAME_TIME);
            //Console.WriteLine(delay);
            try
            {
                lock (actionLock)
                {
                    //Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerActions.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);
                    List<Item> itemsToCreate = JsonConvert.DeserializeObject<List<Item>>(itemsToCreateJson, ToolsSystem.jsonSerializerSettings);

                    //Dictionary<int, (double, double)> coordinates = JsonConvert.DeserializeObject<Dictionary<int, (double, double)>>(coordsJson, ToolsSystem.jsonSerializerSettings);

                    
                    foreach (var item in itemsToCreate)
                    {
                        if (id != 0)
                        {
                            item.SetItemFromClient(gvars);
                        }
                    }
                    /*foreach (int itemId in actionMethodNames.Keys)
                    {
                        gvars.ItemsPlayers[itemId].SetActions(now, gvars, Constants.DELAY, actionMethodNames[itemId]);
                    }*/
                    /*foreach (var id in playerInfo.Keys)
                    {
                        if (id != this.id)
                        {
                            gvars.ItemsPlayers[id].Angle = playerInfo[id];
                        }
                    }*/
                    foreach (var item in info)
                    {
                        if (item.Value.ContainsKey(Item.ItemProperties.X))
                        {
                            gvars.Items[item.Key].X = (double)item.Value[Item.ItemProperties.X];
                        }
                        if (item.Value.ContainsKey(Item.ItemProperties.Y))
                        {
                            gvars.Items[item.Key].Y = (double)item.Value[Item.ItemProperties.Y];
                        }
                        if (item.Value.ContainsKey(Item.ItemProperties.Angle) && this.id != item.Key)
                        {
                            (gvars.Items[item.Key] as Movable).Angle = (double)item.Value[Item.ItemProperties.Angle];
                        }
                        if (item.Value.ContainsKey(Item.ItemProperties.Lives))
                        {
                            (gvars.Items[item.Key] as ILived).CurLives = (double)item.Value[Item.ItemProperties.Lives];
                        }
                        if (item.Value.ContainsKey(Item.ItemProperties.Score))
                        {
                            (gvars.Items[item.Key] as Player).Score = (int)item.Value[Item.ItemProperties.Score];
                        }
                        if (item.Value.ContainsKey(Item.ItemProperties.Shield))
                        {
                            (gvars.Items[item.Key] as Character).Shield = item.Value[Item.ItemProperties.Shield];
                        }
                    }
                    foreach (var itemId in itemsToDestroy)
                    {
                        gvars.Items[itemId].Dispose();
                        if (itemId == this.id)
                        {
                            EndGame();
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        void ExecuteAction(PlayerActions.PlayerActionsEnum action, bool down, int itemId, double angle, double x, double y, int actionIdReceived)
        {
            lock (frameLock)
            {
                gvars.Items[itemId].X = x;
                gvars.Items[itemId].Y = y;
                if (actions.ContainsKey(actionIdReceived))
                {
                    // (gvars.Items[itemId] as Movable).Move(ToolsSystem.GetPercentageOfFrame(actions[actionId].Item3, gvars.GetNow()));
                }
                //gvars.Items[itemId].SetActions(gvars.GetNow(), gvars, Constants.DELAY, new List<(PlayerActions.PlayerActionsEnum, bool)> { (action, down) });
            }
        }
        #endregion




    }
}
