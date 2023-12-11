using Fanior.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;
using System.Threading;
using Newtonsoft.Json;

namespace Fanior.Client.Pages
{
    public partial class Index
    {
        bool showUpgrades = false;

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
                // JS.InvokeVoidAsync("Alert", e.Message + " " + e.Source + " " + e.StackTrace);
                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
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
            KeyController.AddKey("c", new RegisteredKey(PlayerActions.PlayerActionsEnum.cheat, myActions));
            KeyController.AddKey("e", new RegisteredKey(PlayerActions.PlayerActionsEnum.abilityE, myActions, AbilityPressedE));
            KeyController.AddKey("q", new RegisteredKey(PlayerActions.PlayerActionsEnum.abilityQ, myActions, AbilityPressedQ));
            KeyController.AddKey(" ", new RegisteredKey(PlayerActions.PlayerActionsEnum.fire, myActions));
            KeyController.AddKey("click", new RegisteredKey(PlayerActions.PlayerActionsEnum.fire, myActions));
        }

        void AbilityPressedE()
        {
            if (player.AbilityE != null)
                AbilityPressed(player.AbilityE);
        }
        void AbilityPressedQ()
        {
            if (player.AbilityQ != null)
                AbilityPressed(player.AbilityQ);
        }
        void AbilityPressed(Ability ability)
        {
            ability.BeingUsed = true;
            ability.Reloaded = false;
            cah.AddAction(gvars, new ItemAction("abilityRunOut", ToolsMath.TimeToFrames(ability.Duration), ItemAction.ExecutionType.OnlyFirstTime, true, ability), ability == player.AbilityE ? "abilityE" : "abilityQ", 0);
            cah.AddAction(gvars, new ItemAction("abilityReload", ToolsMath.TimeToFrames(ability.ReloadTime + ability.Duration), ItemAction.ExecutionType.OnlyFirstTime, true, ability), ability == player.AbilityE ? "abilityE" : "abilityQ", 0);
        }



        [JSInvokable]
        public async void HandleMouseMove(int x, int y)
        {
            if (player != null)
            {
                this.player.Angle = ToolsMath.GetAngleFromLengts(x - width / 2, height / 2 - y);
            }
            if (x < 168)
            {
                upgradeDivClass = "active";
                showUpgrades = false;
            }
            else if (!showUpgrades)
            {
                upgradeDivClass = "";
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

        async void JsSetup()
        {
            try
            {
                selfReference = DotNetObjectReference.Create(this);

                var minInterval = 20;
                await JS.InvokeVoidAsync("onThrottledMouseMove",
                     root, selfReference, minInterval);
                await JS.InvokeVoidAsync("onKeyDown",
                    mySvg, selfReference);
                await JS.InvokeVoidAsync("onKeyUp",
                    mySvg, selfReference);
                await JS.InvokeVoidAsync("onMouseDown",
                    mySvg, selfReference);
                await JS.InvokeVoidAsync("onMouseUp",
                    mySvg, selfReference);
                await JS.InvokeVoidAsync("onResize", selfReference);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
            }
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { await JS.InvokeVoidAsync("SetFocus", textBox); }

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
                scoreGained = player.GetScore();
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
                await JS.InvokeVoidAsync("SetFocus", textBox);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " " + e.Source + " " + e.StackTrace);
            }

        }
        #endregion
        #region Upgrades
        public async void UpgradeStat(int statNum)
        {
            if (player.UpgradePoints > 0 && player.Upgrades[statNum] < 10)
            {
                player.IncreaseStatPoint(statNum);
                player.UpgradePoints--;
                await hubConnection.SendAsync("UpgradeStat", gvars.GameId, this.id, statNum);
            }
        }
        public async void ObtainAbility(int abilityNum)
        {
            //Deep copy
            var ability = JsonConvert.DeserializeObject<Ability>(JsonConvert.SerializeObject(ToolsGame.abilities[abilityNum]));
            if (player.UpgradePoints >= ability.Cost && (player.AbilityE == null || player.AbilityQ == null))
            {
                if (player.AbilityE == null)
                {
                    player.AbilityE = ability;
                }
                else if (player.AbilityQ == null)
                {
                    player.AbilityQ = ability;
                }
                player.UpgradePoints -= ability.Cost;
                await hubConnection.SendAsync("ObtainAbility", gvars.GameId, this.id, abilityNum);
            }
        }
        public async void UpgradeWeapon(int childNum)
        {
            if (player.PointsGained >= 5)
            {
                player.PointsGained -= 5;
                player.WeaponNode = player.WeaponNode.Children[childNum];
                await hubConnection.SendAsync("UpgradeWeapon", gvars.GameId, this.id, childNum);
                StateHasChanged();
            }
        }
        #endregion

    }
}
