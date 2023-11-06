using Fanior.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Fanior.Client.Pages
{
    public partial class Index
    {
        /// <summary>
        /// Method containing all stuff that are to be proceeded at the start of the load up.
        /// </summary>
        public async Task Start()
        {
            try
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
                    PlayerActions.SetupActions();
                    LambdaActions.SetupLambdaActions();
                }
                await InvokeAsync(() => this.StateHasChanged());
                animEnd = false;
            }
            catch (Exception e)
            {
                // JS.InvokeVoidAsync("Alert", e.Message);
                throw;
            }
        }
        #region Input control

        public void DefaultAssingOfKeys()
        {
            //set keys here
            KeyController.AddKey("w", new RegisteredKey(PlayerActions.PlayerActionsEnum.moveUp, myActions));
            KeyController.AddKey("s", new RegisteredKey(PlayerActions.PlayerActionsEnum.moveDown, myActions));
            KeyController.AddKey("d", new RegisteredKey(PlayerActions.PlayerActionsEnum.moveRight, myActions));
            KeyController.AddKey("a", new RegisteredKey(PlayerActions.PlayerActionsEnum.moveLeft, myActions));
            KeyController.AddKey(" ", new RegisteredKey(PlayerActions.PlayerActionsEnum.fire, myActions));
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
            if (player == null)
            {
                if (keycode == "enter")
                {
                    Start();
                }
            }
            else
            {
                if (!pressedKeys.Contains(keycode))
                {
                    var key = KeyController.GetRegisteredKey(keycode);
                    if (key != null)
                        SendAction(key.KeyDown(), true);
                    pressedKeys.Add(keycode);
                    //send serialized action, game id, item id and angle of player that is sent every frame

                }
            }
            if (keycode == "p")
            {
                //Ping();
                ping = true;
            }

        }
        bool ping = false;
        [JSInvokable]
        public void HandleKeyUp(string keycode)
        {
            if (player != null)
            {
                keycode = keycode.ToLower();
                if (pressedKeys.Contains(keycode))
                {
                    var key = KeyController.GetRegisteredKey(keycode);
                    if (key != null)
                        SendAction(KeyController.GetRegisteredKey(keycode).KeyUp(), false);


                    pressedKeys.Remove(keycode);
                }
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
            try
            {
                animEnd = true;
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                firstConnect = 1;
                await Animate(false);
                await hubConnection.DisposeAsync();
                player = null;
                gvars = null;
                pressedKeys.Clear();
                hubConnection = null;
                myActions.Clear();
                this.id = 0;
                await Animate(true);
            }
            catch (Exception e)
            {

                throw;
            }

        }
        #endregion

    }
}
