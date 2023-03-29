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
        long actionId = 0;
        List<string> pressedKeys = new List<string>();
        //time for tracking actions which are sent to server
        long now;
        List<string> actions = new();
        int id;
        //just for testing
        int counter = 0;
        private string info = "0";
        private Player player;
        private Gvars gvars;
        private int width = 500;
        private int height = 500;
        public ElementReference mySvg;
        public HubConnection hubConnection;
        #endregion
        public async Task Start()
        {
            Task t1 = SetConnection();
            Task t2 = GetDimensions();
            await Task.WhenAll(t1, t2);
            DefaultAssingOfKeys();
            await InvokeAsync(() => this.StateHasChanged());
        }
        #region Input control
        private async Task SendKeyToServer(string actionMethodName)
        {
            actions.Add(actionMethodName);
        }

        public void DefaultAssingOfKeys()
        {
            //set keys here
            KeyController.AddKey("w", new RegisteredKey(null, null, PlayerAction.MoveUp, SendKeyToServer));
            KeyController.AddKey("s", new RegisteredKey(null, null, PlayerAction.MoveDown, SendKeyToServer));
            KeyController.AddKey("d", new RegisteredKey(null, null, PlayerAction.MoveRight, SendKeyToServer));
            KeyController.AddKey("a", new RegisteredKey(null, null, PlayerAction.MoveLeft, SendKeyToServer));

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
                    await hubConnection.SendAsync("ExecuteList", actions, gvars.GameId, this.id);
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
                info = str.ToString();
                StateHasChanged();
            });
            hubConnection.On<int, long>("JoinGame", (id, now) =>
            {
                JoinGame(id, now);
            });

            hubConnection.On<string, int>("ReceiveCommands", (just, now) =>
            {

            });

            hubConnection.On<string>("ReceiveGvars", (just) =>
            {
                try
                {
                    sw.Start();
                    gvars = JsonConvert.DeserializeObject<Gvars>(just, ToolsGame.jsonSerializerSettings);
                    sw.Stop();
                    Console.WriteLine(sw.ElapsedMilliseconds);
                    sw.Reset();
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

        private void JoinGame(int id, long now)
        {
            this.now = now;
            this.id = id;
            info = "2";
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
