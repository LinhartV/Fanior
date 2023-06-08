using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                KeyController.GetRegisteredKey(keycode)?.KeyDown();
                pressedKeys.Add(keycode);
            }
        }
        [JSInvokable]
        public void HandleKeyUp(string keycode)
        {
            keycode = keycode.ToLower();
            if (pressedKeys.Contains(keycode))
            {
                KeyController.GetRegisteredKey(keycode)?.KeyUp();
                pressedKeys.Remove(keycode);
            }
        }
        public void Dispose() => selfReference?.Dispose();
        protected async Task MouseDown(MouseEventArgs e)
        {


        }
        #endregion
        #region Frame

        protected Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    //sw.Start();
                    Frame();
                    await Task.Delay((1), stoppingToken);
                    /*sw.Stop();
                    Console.WriteLine(sw.ElapsedMilliseconds);
                    sw.Reset();*/
                }
            });
        }
        int sendMessageId = 0;
        private async Task Frame()
        {
            try
            {
                
                //serialization of actions
                //send serialized actions, game id, item id and angle of player that is sent every frame
                
                await Task.Run(() => hubConnection.SendAsync("ExecuteList", JsonConvert.SerializeObject(myActions, ToolsSystem.jsonSerializerSettings), gvars.GameId, this.id, player.Angle, sendMessageId));
                sendMessageId++;
                myActions.Clear();
                lock (frameLock)
                {
                    ToolsGame.ProceedFrame(gvars, now);
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
            }
            counter += 1;*/
            if (player == null)
            {
                return;
            }
            try
            {
                this.now = now;
                Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);
                    
                foreach (int itemId in actionMethodNames.Keys)
                {
                    if (actionMethodNames[itemId].Count > 0)
                    {
                        Console.WriteLine(actionMethodNames[itemId][0].Item1.ToString());
                        //counter += 1;
                    }
                    if (gvars.PlayerActions.ContainsKey(itemId))
                    {
                        gvars.PlayerActions[itemId].AddRange(actionMethodNames[itemId]);
                    }
                    else
                        gvars.PlayerActions.Add(itemId, actionMethodNames[itemId]);
                }
                foreach (var id in angles.Keys)
                {
                    if (id != this.id)
                    {
                        gvars.ItemsPlayers[id].Angle = angles[id];
                    }
                }
                
            }
            catch (Exception ex)
            {

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
                    //sw2.Start();
                    ExecuteList(actionMethodNamesJson, messageId, angles, now);
                    /*sw2.Stop();
                    Console.WriteLine("ExecuteList: "+sw2.ElapsedMilliseconds);
                    sw2.Reset();*/
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
                    this.now = now;
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
                Console.Clear();
                Console.WriteLine("New player");
                sw2.Start();
                if (id != idConnectedPlayer)
                {
                    ToolsSystem.DeserializePlayer(playerJson, gvars);
                    await Task.Delay(1);
                }
                sw2.Stop();
                Console.WriteLine("ExecuteList: " + sw2.ElapsedMilliseconds);
                sw2.Reset();
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
                        ExecuteAsync(new CancellationToken(false));
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
