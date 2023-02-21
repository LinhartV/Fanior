using System;
using System.Collections.Generic;
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

        //time for tracking actions
        long now;
        List<Action> actions = new List<Action>();
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

        public async Task Start()
        {
            Task t1 = SetConnection();
            Task t2 = GetDimensions();
            await Task.WhenAll(t1, t2);
            DefaultAssingOfKeys();
            await InvokeAsync(() => this.StateHasChanged());
        }

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

        private async Task SendKeyToServer(string actionMethodName)
        {
            await hubConnection.SendAsync("Execute", actionMethodName, gvars.GameId, this.id);
        }

        public void DefaultAssingOfKeys()
        {
            //set keys here
            KeyController.AddKey("w", new RegisteredKey(PlayerAction.MoveUp, null, null, SendKeyToServer));
            KeyController.AddKey("s", new RegisteredKey(null, null, PlayerAction.MoveDown, SendKeyToServer));
        }

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

            hubConnection.On<string, int>("ReceiveGvars", (just, now) =>
            {
                try
                {
                    this.now = now;
                    var jsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Formatting = Newtonsoft.Json.Formatting.Indented,
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore

                    };

                    gvars = JsonConvert.DeserializeObject<Gvars>(just, jsonSerializerSettings);

                    player = gvars.ItemsPlayers[id];

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
            ExecuteAsync(new CancellationToken(false));
        }

        protected void KeyDown(KeyboardEventArgs e)
        {
            KeyController.GetRegisteredKey(e.Key)?.KeyDown(id, gvars);
        }
        protected async Task KeyUp(KeyboardEventArgs e)
        {
            //Commands.KeyDown(e.Key.ToString(), gvars, Player);
            await hubConnection.SendAsync("KeyUp", e.Key.ToString());
        }
        string color = "white";
        bool ready = true;
        protected async Task MouseDown(MouseEventArgs e)
        {

            //await JS.InvokeVoidAsync("Alert", "Pressed");
            //Commands.KeyDown(e.Key.ToString(), gvars, Player);
            if (ready == true)
            {
                await hubConnection.SendAsync("MouseDown", this.id);
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
        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

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
            //await hubConnection.SendAsync("ClientActions", actions, id);
        }

    }
}
