using Fanior.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Fanior.Shared.Item;

namespace Fanior.Client.Pages
{
    public partial class Index
    {

        #region Set connection
        public async Task SetConnection()
        {
            hubConnection = new HubConnectionBuilder()
           .WithUrl(NavigationManager.ToAbsoluteUri("myhub"))
           .Build();
            hubConnection.On<string>("ReceiveMessage", (str) =>
            {
                // info = str.ToString();
                counter++;
                StateHasChanged();
            });
            hubConnection.On<double>("Ping", (time) =>
            {
                /*Console.WriteLine("server time: " + (time - sw.Elapsed.TotalMilliseconds));
                pingWatch.Stop();
                Console.WriteLine("total ping: "+pingWatch.Elapsed.TotalMilliseconds);
                pingWatch.Reset();*/
                serverTime = time;
                reping = true;
            });
            hubConnection.On<string>("ReceiveRandomNumbers", (listJson) =>
            {
                List<Gvars.Message.RandomNumbers> list = JsonConvert.DeserializeObject<List<Gvars.Message.RandomNumbers>>(listJson, ToolsSystem.jsonSerializerSettings);
                foreach (var item in list)
                {
                    if (item.purpose == "randomAI")
                    {
                        ((gvars.ItemsStep[item.id] as Enemy).AI as RandomGoingAI).ReceiveRandomNumbers(item.numbers);
                    }

                }
            });
            /*hubConnection.On<int>("PlayerDied", (id) =>
            {
                lock (actionLock)
                {
                    gvars.ItemsPlayers[id].Dispose(gvars);
                    if (id == this.id)
                    {
                        EndGame();
                    }
                }
            });*/
            /*hubConnection.On<string>("CreateNewItem", (itemJson) =>
            {
                if (id != 0)
                {
                    Item item = JsonConvert.DeserializeObject<Item>(itemJson, ToolsSystem.jsonSerializerSettings);
                    item.SetItemFromClient(gvars);
                }
            });*/

            hubConnection.On<PlayerActions.PlayerActionsEnum, bool, int, double, double, double, int>("ExecuteAction", (action, down, itemId, angle, x, y, actionIdReceived) =>
            {
                ExecuteAction(action, down, itemId, angle, x, y, actionIdReceived);
            });
            //Actions to be proceeded that server sent to client 
            //Dictionary of list of actions assigned to object id along with information, if it's keydown or keyup (true = keydown).
            //+messageId to check if connection was lost.
            //time, messageId, /*PlayerActions, angles,*/ itemsToCreate, itemsToDestroy (id), info
            hubConnection.On<double, long/*, string, Dictionary<int, double>*/, string, List<int>, Dictionary<int, Dictionary<ItemProperties, double>>>("ExecuteList", (now, messageId/*, actionMethodNamesJson, playerInfo*/, itemsToCreate, itemsToDestroy, info) =>
            {
                try
                {
                    sw2.Start();
                    ExecuteList(now, messageId/*, actionMethodNamesJson, playerInfo*/, itemsToCreate, itemsToDestroy, info);

                    StateHasChanged();
                    sw2.Stop();
                    Console.WriteLine("list:" + sw2.Elapsed.TotalMilliseconds);

                    sw2.Reset();
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
                }

            });
            hubConnection.On<string>("GetItems", (itemsJson) =>
            {
                try
                {
                    sw2.Start();
                    GetAllItems(itemsJson);

                    StateHasChanged();
                    sw2.Stop();
                    Console.WriteLine("list:" + sw2.Elapsed.TotalMilliseconds);

                    sw2.Reset();
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
                }

            });
            //this player joined game
            hubConnection.On<int, string, double, long>("JoinGame", async (idReceived, gvarsJson, now, messageId) =>
            {
                if (this.id == 0)
                {
                    counter = 0;
                    currentMessageId = messageId;
                    JoinGame(idReceived);
                    ReceiveGvars(gvarsJson);
                    gvars.StartMeasuringTime(now);
                    gvars.PlayerActions.Add(id, null);
                    startTime = now;
                    frameDuration = 0;
                    sw.Start();
                    if (firstConnect > 0)
                    {
                        //ExecuteAsync(new CancellationToken(false));
                        Task framTask = Task.Run(async () =>
                        {
                            timer = new System.Threading.Timer(async _ =>
                            {
                                if (firstConnect == 0)
                                    Frame();
                                await InvokeAsync(StateHasChanged);


                            }, null, 0, Constants.CONTROL_FRAME_TIME);
                        });
                        firstConnect = 0;
                    }
                    await Animate(true);
                    await JS.InvokeVoidAsync("SetFocus", mySvg);
                    gvars.ready = true;
                    JsSetup();
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
                        p.Dispose();
                    }
                }
            });
        }
        public async void Ping()
        {
            await hubConnection.SendAsync("Ping", sw2.Elapsed.TotalMilliseconds);
            pingWatch.Start();
        }

        private void SendAction(PlayerActions.PlayerActionsEnum action, bool down)
        {

            actions.Add(actionId, (action, down, gvars.GetNow()));
            hubConnection.SendAsync("ExecuteAction", action, down, gvars.GameId, this.id, player.Angle, actionId);
            actionId++;
        }
        private void ReceiveGvars(string gvarsJson)
        {
            try
            {
                gvars = JsonConvert.DeserializeObject<Gvars>(gvarsJson, ToolsSystem.jsonSerializerSettings);
                gvars.server = false;
                player = gvars.ItemsPlayers[id];
                gvars.DeleteNonClientActions();

                foreach (var item in gvars.Items.Values)
                {
                    item.SetItemFromClient(gvars);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
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
