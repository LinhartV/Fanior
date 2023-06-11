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
        public List<(PlayerAction.PlayerActionsEnum, bool)> myActions = new();
        private DotNetObjectReference<Index> selfReference;
        //number of frames that passed during lag between server and client
        private int delay;
        //When the client was connected
        long startTime = 0;
        private readonly object actionLock = new object();
        private readonly object frameLock = new object();
        //0 = during connection, 1 = died and need to reset, 2 = really first connect
        int firstConnect = 2;
        int sendMessageId = 0;
        //measuring frame
        Stopwatch sw = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        System.Threading.Timer timer;
        #endregion



        /// <summary>
        /// Method containing all stuff that are to be proceeded at the start of the load up.
        /// </summary>
        public async Task Start()
        {
            await Animate(false);
            Task t1 = SetConnection();
            Task t2 = GetDimensions();
            await Task.WhenAll(t1, t2);
            await hubConnection.StartAsync();
            await hubConnection.SendAsync("OnLogin", "@@@", name == null ? "Figher" : name);
           
            if (firstConnect == 2)
            {
                DefaultAssingOfKeys();
                PlayerAction.SetupActions();
                LambdaActions.setupLambdaActions();
            }
            player.Score = 10;
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
                KeyController.GetRegisteredKey(keycode)?.KeyDown(gvars.GetNow());
                pressedKeys.Add(keycode);
            }
        }
        [JSInvokable]
        public void HandleKeyUp(string keycode)
        {
            keycode = keycode.ToLower();
            if (pressedKeys.Contains(keycode))
            {
                KeyController.GetRegisteredKey(keycode)?.KeyUp(gvars.GetNow());
                pressedKeys.Remove(keycode);
            }
        }
        [JSInvokable]
        public void HandleWindowResize(int width, int height)
        {
            this.width = width;
            this.height = height;
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
                    if (sw.ElapsedMilliseconds > Constants.FRAME_TIME-20)
                    {
                        await Task.Delay(Constants.FRAME_TIME - (int)sw.ElapsedMilliseconds, stoppingToken);
                        Frame();
                        sw.Restart();
                    }
                    await Task.Delay((1), stoppingToken);
                }
            });
        }*/
        private async Task Animate(bool down)
        {

            zindex = 100;
            while (true)
            {
                if (down)
                    opacity -= 0.05;
                else
                    opacity += 0.05;
                await Task.Delay(10);
                if (opacity < 0)
                {
                    zindex = -100;
                    opacity = 0;
                    break;
                }
                if (opacity > 1)
                {
                    opacity = 1;
                    break;
                }
                StateHasChanged();
            }
            StateHasChanged();

        }
        private async void EndGame()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            await Animate(false);
            await hubConnection.DisposeAsync();
            player = null;
            gvars = null;
            pressedKeys.Clear();
            hubConnection = null;
            myActions.Clear();
            firstConnect = 1;
            this.id = 0;
            await Animate(true);
        }
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
                    ToolsGame.ProceedFrame(gvars, gvars.GetNow(), delay, false);
                    if (player.Score > nextLevel)
                    {
                        //Bonus
                        nextLevel = nextLevel * 5 / 2;
                    }
                }
                StateHasChanged();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        private void ExecuteList(string actionMethodNamesJson, long messageId, Dictionary<int, (double, double, double)> playerInfo, long now)
        {
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
            delay = (int)(now - gvars.GetNow());
            int frames = (int)Math.Floor((double)delay / Constants.FRAME_TIME);
            //Console.WriteLine(delay);
            Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);

            foreach (int itemId in actionMethodNames.Keys)
            {
                gvars.ItemsPlayers[itemId].SetActions(now, gvars, Constants.DELAY - delay, actionMethodNames[itemId]);
            }
            foreach (var id in playerInfo.Keys)
            {
                if (id != this.id)
                {
                    gvars.ItemsPlayers[id].Angle = playerInfo[id].Item1;
                }
                gvars.ItemsPlayers[id].X = playerInfo[id].Item2;
                gvars.ItemsPlayers[id].Y = playerInfo[id].Item3;
            }
        }

        #endregion
        #region Set connection
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
            hubConnection.On<long, long, string, string>("ExecuteList", (now, messageId, actionMethodNamesJson, playerInfo) =>
            {
                try
                {
                    ExecuteList(actionMethodNamesJson, messageId, JsonConvert.DeserializeObject<Dictionary<int, (double, double, double)>>(playerInfo, ToolsSystem.jsonSerializerSettings), now);
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
                    counter = 0;
                    currentMessageId = messageId;
                    JoinGame(idReceived);
                    ReceiveGvars(gvarsJson);
                    gvars.StartMeasuringTime(now);
                    startTime = now;
                    sw.Start();
                    foreach (var item in gvars.Items.Values)
                    {
                        item.SetItemFromClient(gvars);
                    }
                    await Animate(true);
                    await JS.InvokeVoidAsync("SetFocus", mySvg);
                }
            });
            //new player joined game
            hubConnection.On<string, int>("PlayerJoinGame", async (playerJson, idConnectedPlayer) =>
            {
                counter = 0;
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
            //on login and when connection was lost
            hubConnection.On<string>("PlayerDisconnected", (connectionId) =>
            {
                foreach (Player p in gvars.ItemsPlayers.Values)
                {
                    if (p.ConnectionId == connectionId)
                    {
                        p.Dispose(gvars);
                    }
                }
            });
            
        }

        private void ReceiveGvars(string gvarsJson)
        {
            try
            {
                gvars = JsonConvert.DeserializeObject<Gvars>(gvarsJson, ToolsSystem.jsonSerializerSettings);

                player = gvars.ItemsPlayers[id];
                player.SetItemFromClient(gvars);

                //new Enemy(gvars, 200, 400, new Shape("black", "grey", "grey", "grey", 5, 300, 300, Shape.GeometryEnum.circle), null, 0, 0, 0, 5, null);
                if (firstConnect > 0)
                {
                    Task.Run(async () =>
                    {
                        //ExecuteAsync(new CancellationToken(false));
                        timer = new System.Threading.Timer(async _ =>
                        {
                            if (player.GetCurLives() <= 0)
                                EndGame();
                            else
                                Frame();
                            await InvokeAsync(StateHasChanged);
                            counter = (int)gvars.ItemsPlayers[1].GetCurLives();

                        }, null, 0, Constants.FRAME_TIME);
                    });

                    firstConnect = 0;
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
                    await JS.InvokeVoidAsync("onResize", selfReference);
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
