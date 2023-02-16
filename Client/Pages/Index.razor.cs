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
        long now;
        List<Action> actions = new List<Action>();
        Dictionary<string, ClientKey> keys = new Dictionary<string, ClientKey>();
        int id;
        private string info = "0";
        private Player player;
        private Gvars gvars;
        private int width = 500;
        private int height = 500;
        public ElementReference mySvg;
        private HubConnection hubConnection;
        public async Task Start()
        {
            Task t1 = SetConnection();
            Task t2 = GetDimensions();
            await Task.WhenAll(t1, t2);
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

        public void SetKeys()
        {
            //set keys here
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
                    if (player == null)
                    {
                        player = gvars.ItemsPlayers[id];
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
        }

        private void JoinGame(int id, long now)
        {
            this.now = now;
            this.id = id;
            info = "2";
            this.StateHasChanged();
            ExecuteAsync(new CancellationToken(false));
        }

        protected async Task KeyDown(KeyboardEventArgs e)
        {
            //Commands.KeyDown(e.Key.ToString(), gvars, Player);
            await hubConnection.SendAsync("KeyDown", e.Key.ToString());
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
            if (firstRender)
            {
                await JS.InvokeVoidAsync("SetFocusToElement", mySvg);
                await Start();
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
            await hubConnection.SendAsync("ClientActions", actions, id);
        }

    }
}
