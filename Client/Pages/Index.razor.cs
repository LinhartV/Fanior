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
        int counter = 0;
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
            /*KeyController.AddKey("s", new RegisteredKey(null, null, PlayerAction.MoveDown, SendKeyToServer));
            KeyController.AddKey("d", new RegisteredKey(null, null, PlayerAction.MoveRight, SendKeyToServer));
            KeyController.AddKey("a", new RegisteredKey(null, null, PlayerAction.MoveLeft, SendKeyToServer));*/

        }





        [JSInvokable]
        public void HandleMouseMove(int x, int y)
        {
            this.player.Angle = ToolsMath.GetAngleFromLengts(x - width / 2, height / 2 - y);
            counter = (int)(player.Angle * 180 / Math.PI);
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
                    await Frame();
                    await Task.Delay(1000 / 60, stoppingToken);
                }
            });
        }

        private async Task Frame()
        {
            string json;
            try
            {
                sw.Start();
                //serialization of actions
                json = JsonConvert.SerializeObject(myActions, ToolsSystem.jsonSerializerSettings);
                //send serialized actions, game id, item id and stuff that is sent every frame
                hubConnection.SendAsync("ExecuteList", json, gvars.GameId, this.id, player.Angle);
                myActions.Clear();
                ToolsGame.ProceedFrame(gvars, now);
                gvars.PlayerActions.Clear();
                StateHasChanged();
                sw.Stop();
                if (sw.ElapsedMilliseconds > 0)
                {
                    Console.WriteLine(sw.ElapsedMilliseconds);
                }
                sw.Reset();
            }
            catch (Exception e)
            {
                throw;
            }

        }
        #endregion
        #region Set connection
        bool firstConnect = true;
        Stopwatch sw = new Stopwatch();
        public async Task SetConnection()
        {
            hubConnection = new HubConnectionBuilder()
           .WithUrl(NavigationManager.ToAbsoluteUri("/myhub"))
           .Build();
            hubConnection.On<int>("ReceiveMessage", (str) =>
            {
                // info = str.ToString();
                StateHasChanged();
            });
            //Actions to be proceeded that server sent to client 
            //Dictionary of list of actions assigned to object id along with information, if it's keydown or keyup (true = keydown).
            //+messageId to check if connection was lost.
            hubConnection.On<long, long, string, Dictionary<int, double>>("ExecuteList", (now, messageId, actionMethodNamesJson, angles) =>
            {
                if (currentMessageId + 1 != messageId)
                {
                    JS.InvokeVoidAsync("Alert", "POZOR");
                }
                this.currentMessageId = messageId;
                this.now = now;
                Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);

                foreach (int itemId in actionMethodNames.Keys)
                {
                    gvars.PlayerActions.Add(itemId, actionMethodNames[itemId]);
                    if (actionMethodNames[itemId][0].Item1 == PlayerAction.PlayerActionsEnum.fire)
                    {

                    }
                }
                 foreach (var id in angles.Keys)
                {
                    if (id != this.id)
                    {
                        gvars.ItemsPlayers[id].Angle = angles[id];
                    }
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
                }
            });
            //new player joined game
            hubConnection.On<string, int>("PlayerJoinGame", (playerJson, idConnectedPlayer) =>
            {
                if (id != idConnectedPlayer)
                {
                    ToolsSystem.DeserializePlayer(playerJson, gvars);
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
