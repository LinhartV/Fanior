using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Fanior.Client.Pages
{
    public partial class Index
    {
        #region Variables
        //list of keys that are pressed in the current frame
        List<string> pressedKeys = new List<string>();
        long now;
        //id of this connection
        int id = 0;
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
        public List<(PlayerAction.PlayerActionsEnum, bool)> myActions = new();
        private ManualResetEvent mre = new ManualResetEvent(false);
        private ManualResetEvent mre2 = new ManualResetEvent(false);
        private DotNetObjectReference<Index> selfReference;
        //number of frames that passed during lag between server and client
        private int delay;
        //States of game for each frame 500 ms to the past for restoring actions - in JSON format.
        private Queue<string> pastGvars = new Queue<string>();
        #endregion




        /// <summary>
        /// Method containing all stuff that are to be proceeded at the start of the load up.
        /// </summary>
        public async Task Start()
        {
            Task t1 = SetConnection();
            Task t2 = GetDimensions();
            await Task.WhenAll(t1, t2);
            DefaultAssingOfKeys();
            PlayerAction.SetupActions();
            LambdaActions.setupLambdaActions();
            await InvokeAsync(() => this.StateHasChanged());
        }
        #region Input control

        public void DefaultAssingOfKeys()
        {
            //set keys here
            KeyController.AddKey("w", new RegisteredKey(PlayerAction.PlayerActionsEnum.moveUp, myActions));
            KeyController.AddKey("s", new RegisteredKey(PlayerAction.PlayerActionsEnum.moveDown, myActions));
            KeyController.AddKey("d", new RegisteredKey(PlayerAction.PlayerActionsEnum.moveRight, myActions));
            KeyController.AddKey("a", new RegisteredKey(PlayerAction.PlayerActionsEnum.moveLeft, myActions));
            KeyController.AddKey(" ", new RegisteredKey(PlayerAction.PlayerActionsEnum.fire, myActions));
        }





        [JSInvokable]
        public void HandleMouseMove(int x, int y)
        {
            if (player != null)
            {
                this.player.Angle = ToolsMath.GetAngleFromLengts(x - width / 2, height / 2 - y);
            }
            //counter = (int)(player.Angle * 180 / Math.PI);
        }
        [JSInvokable]
        public void HandleKeyDown(string keycode)
        {
            keycode = keycode.ToLower();
            if (!pressedKeys.Contains(keycode))
            {
                KeyController.GetRegisteredKey(keycode)?.KeyDown(now);
                pressedKeys.Add(keycode);
            }
        }
        [JSInvokable]
        public void HandleKeyUp(string keycode)
        {
            keycode = keycode.ToLower();
            if (pressedKeys.Contains(keycode))
            {
                KeyController.GetRegisteredKey(keycode)?.KeyUp(now);
                pressedKeys.Remove(keycode);
            }
        }
        public void Dispose()
        {
            selfReference?.Dispose();
            timer?.Dispose();
        }
        protected async Task MouseDown(MouseEventArgs e)
        {


        }
        #endregion
        #region Frame
        /*protected Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    Frame();
                    await Task.Delay((1), stoppingToken);
                }
            });
        }*/
        int sendMessageId = 0;
        private async Task Frame()
        {
            try
            {
                //send serialized actions, game id, item id and angle of player that is sent every frame
                await Task.Run(() => hubConnection.SendAsync("ExecuteList", JsonConvert.SerializeObject(myActions, ToolsSystem.jsonSerializerSettings), gvars.GameId, this.id, player.Angle, sendMessageId));
                sendMessageId++;
                myActions.Clear();
                lock (frameLock)
                {
                    /*if (frameDelta >= Constants.FRAME_TIME)
                    {
                        
                        Console.WriteLine("teď");
                        frameDelta -= Constants.FRAME_TIME;
                        ToolsGame.ProceedFrame(gvars, now, true);
                    }
                    else
                    {
                        ToolsGame.ProceedFrame(gvars, now, false);
                    }*/
                    ToolsGame.ProceedFrame(gvars, now, delay);
                    /*sw2.Start();
                    //pastGvars.Enqueue(JsonConvert.SerializeObject(gvars, ToolsSystem.jsonSerializerSettings));
                    sw2.Stop();
                    Console.WriteLine("Serialize: "+sw2.ElapsedMilliseconds);
                    sw2.Reset();*/
                    gvars.PlayerActions.Clear();
                }
                StateHasChanged();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        Stopwatch sw2 = new Stopwatch();
        private void ExecuteList(string actionMethodNamesJson, long messageId, Dictionary<int, double> angles, long now)
        {
            //await JS.InvokeVoidAsync("Alert", "just");
            /*if (counter == 0)
            {
                counter = (int)messageId;
            }
            if (messageId != counter)
            {
                counter2++;
            }*/
            if (player == null)
            {
                return;
            }
            delay = (int)(now - this.now);
            Console.WriteLine(delay);
            this.now = now;
            Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);

            foreach (int itemId in actionMethodNames.Keys)
            {
                if (gvars.PlayerActions.ContainsKey(itemId))
                {
                    gvars.PlayerActions[itemId].AddRange(actionMethodNames[itemId]);
                }
                else
                    gvars.PlayerActions.Add(itemId, actionMethodNames[itemId]);
                /*var tempList = new List<(PlayerAction.PlayerActionsEnum, bool)>(actionMethodNames[itemId]);
                foreach (var action in tempList)
                {
                    if (true)
                    {
                        action
                    }
                }
                for (int i = 0; i < delay; i++)
                {
                    gvars.PlayerActions.Add(itemId, new List<(PlayerAction.PlayerActionsEnum, bool)>)
                }*/
            }
            foreach (var id in angles.Keys)
            {
                if (id != this.id)
                {
                    gvars.ItemsPlayers[id].Angle = angles[id];
                }
            }
        }

        #endregion
        #region Set connection
        private readonly object actionLock = new object();
        private readonly object frameLock = new object();
        bool firstConnect = true;
        Stopwatch sw = new Stopwatch();
        public async Task SetConnection()
        {
            hubConnection = new HubConnectionBuilder()
           .WithUrl(NavigationManager.ToAbsoluteUri("/myhub"))
           .Build();
            hubConnection.On<String>("ReceiveMessage", (str) =>
            {
                // info = str.ToString();
                counter++;
                StateHasChanged();
            });
            //Actions to be proceeded that server sent to client 
            //Dictionary of list of actions assigned to object id along with information, if it's keydown or keyup (true = keydown).
            //+messageId to check if connection was lost.
            hubConnection.On<long, long, string, Dictionary<int, double>>("ExecuteList", (now, messageId, actionMethodNamesJson, angles) =>
            {
                try
                {
                    ExecuteList(actionMethodNamesJson, messageId, angles, now);
                }
                catch (Exception e)
                {

                    throw;
                }

            });
            //this player joined game
            hubConnection.On<int, string, long, long>("JoinGame", async (idReceived, gvarsJson, now, messageId) =>
            {
                if (this.id == 0)
                {
                    currentMessageId = messageId;
                    counter = (int)now;
                    this.now = now;
                    sw.Start();
                    JoinGame(idReceived);
                    ReceiveGvars(gvarsJson);
                    foreach (var item in gvars.Items.Values)
                    {
                        item.SetItemFromClient(gvars);
                    }
                }
            });
            //new player joined game
            hubConnection.On<string, int>("PlayerJoinGame", async (playerJson, idConnectedPlayer) =>
            {
                if (id != idConnectedPlayer)
                {
                    ToolsSystem.DeserializePlayer(playerJson, gvars);
                    await Task.Delay(1);
                }
            });



            //on login and when connection was lost
            hubConnection.On<string>("ReceiveGvars", (gvarsJson) =>
            {
                ReceiveGvars(gvarsJson);
            });
            try
            {
                await hubConnection.StartAsync();
                await hubConnection.SendAsync("OnLogin", "@@@");
            }
            catch (Exception e)
            {
                await JS.InvokeVoidAsync("Alert", e.Message);
            }
            info = "1";
            await JS.InvokeVoidAsync("SetFocus", mySvg);
        }

        private void ReceiveGvars(string gvarsJson)
        {
            try
            {
                gvars = JsonConvert.DeserializeObject<Gvars>(gvarsJson, ToolsSystem.jsonSerializerSettings);

                player = gvars.ItemsPlayers[id];
                player.SetItemFromClient(gvars);

                //new Enemy(gvars, 200, 400, new Shape("black", "grey", "grey", "grey", 5, 300, 300, Shape.GeometryEnum.circle), null, 0, 0, 0, 5, null);
                if (firstConnect)
                {
                    Task.Run(async () =>
                    {
                        //ExecuteAsync(new CancellationToken(false));
                        timer = new System.Threading.Timer(async _ =>  // async void
                        {
                            // we need StateHasChanged() because this is an async void handler
                            // we need to Invoke it because we could be on the wrong Thread
                            Frame();
                            await InvokeAsync(StateHasChanged);

                        }, null, 0, Constants.FRAME_TIME);
                    });

                    firstConnect = false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            StateHasChanged();
        }
        private void JoinGame(int id)
        {
            this.id = id;
            info = id.ToString();
            this.StateHasChanged();
        }

        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
        #endregion

        /*List<IWorker> workers = new List<IWorker>();
        List<IWorkerBackgroundService<TaskService>> backgroundServices =
                new List<IWorkerBackgroundService<TaskService>>();
        public async Task Try()
        {
            try
            {
                var worker = await workerFactory.CreateAsync();
                workers.Add(await workerFactory.CreateAsync());
                var service = await worker.CreateBackgroundServiceAsync<TaskService>();
                backgroundServices.Add(service);
                await service.RegisterEventListenerAsync(nameof(TaskService.Num),
                    (object s, Return eventInfo) =>
                    {
                        counter = eventInfo.Progress;
                        StateHasChanged();
                    });
                Task.Run(()=>service.RunAsync(s => s.TryMethod(5)));
            }
            catch (Exception e)
            {

                throw;
            }
            
        }*/

        #region Other
        async Task GetDimensions()
        {
            try
            {
                var dimension = await new DimensionReader.BrowserService(JS).GetDimensions();
                height = dimension.Height;
                width = dimension.Width;
            }
            catch (Exception)
            {
            }
        }
        System.Threading.Timer timer;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    selfReference = DotNetObjectReference.Create(this);
                    var minInterval = 20;
                    await JS.InvokeVoidAsync("onThrottledMouseMove",
                        mySvg, selfReference, minInterval);
                    await JS.InvokeVoidAsync("onKeyDown",
                        mySvg, selfReference);
                    await JS.InvokeVoidAsync("onKeyUp",
                        mySvg, selfReference);
                    await Start();

                }

            }
            catch (Exception e)
            {

                throw;
            }
        }
        #endregion



    }
}
