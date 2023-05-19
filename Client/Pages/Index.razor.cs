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
        //list of all actions that are going to be proceeded and sent to server along with information, if key for this action was pressed or released
        List<(PlayerAction.PlayerActionsEnum, bool)> actions = new();
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
            await InvokeAsync(() => this.StateHasChanged());
        }
        #region Input control
        private async Task SendKeyToServer(PlayerAction.PlayerActionsEnum actionMethodName, bool keyDown)
        {
            actions.Add((actionMethodName, keyDown));
        }

        public void DefaultAssingOfKeys()
        {
            //set keys here
            KeyController.AddKey("w", new RegisteredKey(PlayerAction.PlayerActionsEnum.none, PlayerAction.PlayerActionsEnum.none, PlayerAction.PlayerActionsEnum.moveUp, SendKeyToServer));
            /*KeyController.AddKey("s", new RegisteredKey(null, null, PlayerAction.MoveDown, SendKeyToServer));
            KeyController.AddKey("d", new RegisteredKey(null, null, PlayerAction.MoveRight, SendKeyToServer));
            KeyController.AddKey("a", new RegisteredKey(null, null, PlayerAction.MoveLeft, SendKeyToServer));*/

        }

        protected void KeyDown(KeyboardEventArgs e)
        {
            KeyController.GetRegisteredKey(e.Key.ToLower())?.KeyDown(id, gvars);
            if (!pressedKeys.Contains(e.Key.ToLower()))
            {
                pressedKeys.Add(e.Key.ToLower());
            }
        }
        protected void KeyUp(KeyboardEventArgs e)
        {
            KeyController.GetRegisteredKey(e.Key.ToLower())?.KeyUp(id, gvars);
            if (pressedKeys.Contains(e.Key.ToLower()))
            {
                pressedKeys.Remove(e.Key.ToLower());
            }
        }
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
                foreach (var pressedKey in pressedKeys)
                {
                    if (KeyController.GetRegisteredKey(pressedKey).KeyPressed != null)
                    {
                        KeyController.GetRegisteredKey(pressedKey).KeyPressed(id, gvars);
                    }
                }
                if (actions.Count > 0)
                {
                    json = JsonConvert.SerializeObject(actions, ToolsSystem.jsonSerializerSettings);
                    await hubConnection.SendAsync("ExecuteList", json, gvars.GameId, this.id);
                    actions.Clear();
                }
                StateHasChanged();
            }
            catch (Exception e)
            {

                throw;
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
                    await Start();
                }

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
            //list of actions to be proceeded that server sent to client 
            hubConnection.On<long, string>("ExecuteList", (messageId, actionMethodNamesJson) =>
            {
                Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>> actionMethodNames = JsonConvert.DeserializeObject<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>>(actionMethodNamesJson, ToolsSystem.jsonSerializerSettings);

                foreach (int itemId in actionMethodNames.Keys)
                {
                    foreach (var item in actionMethodNames[itemId])
                    {
                        PlayerAction.InvokeAction(item.Item1, item.Item2, itemId, gvars);
                    }
                }
            });
            //this player joined game
            hubConnection.On<int, string>("JoinGame", (idReceived, gvarsJson) =>
            {
                if (this.id == 0)
                {
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
            //Dictionary of list of actions assigned to object id along with information, if it's keydown or keyup (true = keydown).
            //+messageId to check if connection was lost.
            hubConnection.On<Dictionary<int, List<(PlayerAction.PlayerActionsEnum, bool)>>, int>("ExecuteList", (actionMethodNames, messageId) =>
            {
                //skončil jsem tady - teď musím udělat, aby se posílali stisky kláves "stisknuto", "released", aby na serveru to mohlo jet plynule jako tady.
                sw.Start();
                foreach (int playerId in actionMethodNames.Keys)
                {
                    foreach (var action in actionMethodNames[playerId])
                    {
                        PlayerAction.InvokeAction(action.Item1, action.Item2, playerId, gvars);
                    }
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
                sw.Reset();
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

                if (firstConnect)
                {
                    ExecuteAsync(new CancellationToken(false));
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





    }
}
