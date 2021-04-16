using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grimoire.Tools
{
    public class AutoRelogin
    {
        private Flash flash;
        private OptionsManager optionsManager;
        private BotManager botManager;
        private Player player;
        private World world;
        public AutoRelogin(Player newPlayer, BotManager newBotManager, Flash newFlash, OptionsManager newOptions, World newWorld)
        {
            world = newWorld;
            flash = newFlash;
            player = newPlayer;
            botManager = newBotManager;
            optionsManager = newOptions;
        }
        public bool IsTemporarilyKicked => flash.Call<bool>("IsTemporarilyKicked", new string[0]);

        public bool AreServersLoaded => flash.Call<bool>("AreServersLoaded", new string[0]);

        public void Login()
        {
            flash.Call("Login", new string[0]);
        }

        public bool ResetServers()
        {
            return flash.Call<bool>("ResetServers", new string[0]);
        }

        public void Connect(Server server)
        {
            flash.Call("Connect", server.Name);
        }

        public async Task Login(Server server, int relogDelay, CancellationTokenSource cts, bool ensureSuccess)
        {
            bool killLag = optionsManager.LagKiller;
            bool disableAnims = optionsManager.DisableAnimations;
            bool hidePlayers = optionsManager.HidePlayers;
            if (killLag)
            {
                optionsManager.LagKiller = false;
            }
            if (disableAnims)
            {
                optionsManager.DisableAnimations = false;
            }
            if (hidePlayers)
            {
                optionsManager.HidePlayers = false;
            }
            if (IsTemporarilyKicked)
            {
                await botManager.ActiveBotEngine.WaitUntil(() => !IsTemporarilyKicked, () => !cts.IsCancellationRequested, 65);
            }
            if (cts.IsCancellationRequested)
            {
                return;
            }
            ResetServers();
            Login();
            await botManager.ActiveBotEngine.WaitUntil(() => AreServersLoaded, () => !cts.IsCancellationRequested, 30);
            if (cts.IsCancellationRequested)
            {
                return;
            }
            Connect(server);
            await botManager.ActiveBotEngine.WaitUntil(() => !world.IsMapLoading, () => !cts.IsCancellationRequested, 40);
            if (!cts.IsCancellationRequested)
            {
                await Task.Delay(relogDelay);
                if (ensureSuccess)
                {
                    Task.Run(() => EnsureLoginSuccess(cts));
                }
                if (killLag)
                {
                    optionsManager.LagKiller = true;
                }
                if (disableAnims)
                {
                    optionsManager.DisableAnimations = true;
                }
                if (hidePlayers)
                {
                    optionsManager.HidePlayers = true;
                }
            }
        }

        private async Task EnsureLoginSuccess(CancellationTokenSource cts)
        {
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);
                if (cts.IsCancellationRequested)
                {
                    return;
                }
                string map = player.Map;
                if (!string.IsNullOrEmpty(map) && !map.Equals("name", StringComparison.OrdinalIgnoreCase) && !map.Equals("battleon", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
            if (player.Map.Equals("battleon", StringComparison.OrdinalIgnoreCase))
            {
                player.Logout();
            }
        }
    }
}