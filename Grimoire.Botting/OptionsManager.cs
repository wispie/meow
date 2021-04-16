using Grimoire.Game;
using Grimoire.Networking;
using Grimoire.Networking.Handlers;
using Grimoire.Tools;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Grimoire.Botting
{
    

    public class OptionsManager
    {
        private Flash flash;
        private Proxy proxy;
        private Player player;
        private BotData botData;
        private Bot bot;
        public OptionsManager(Player newPlayer, Flash newFlash, Proxy newProxy, BotData newBotData, Bot newBot)
        {
            bot = newBot;
            flash = newFlash;
            player = newPlayer;
            botData = newBotData;
            proxy = newProxy;
        }

        private bool _isRunning;

        private bool _disableAnimations;

        private bool _hidePlayers;

        private bool _infRange;

        private bool _hideYulgar;

        private bool _hideRoom;

        private bool _afk;

        private bool _afk2;

        private bool _infMana;

        public bool InfMana
        {
            get => _infMana;
            set
            {
                _infMana = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                _isRunning = value;
                StateChanged?.Invoke(value);
            }
        }

        public bool Buff
        {
            get;
            set;
        }

        public bool ProvokeMonsters
        {
            get;
            set;
        }

        public bool EnemyMagnet
        {
            get;
            set;
        }

        public bool LagKiller
        {
            get;
            set;
        }

        public bool SkipCutscenes
        {
            get;
            set;
        }
        
        public bool DisableAnimations
        {
            get => _disableAnimations;
            set
            {
                _disableAnimations = value;
                if (value)
                    proxy.RegisterHandler(HandlerDisableAnimations);
                else
                    proxy.UnregisterHandler(HandlerDisableAnimations);
            }
        }

        public bool HidePlayers
        {
            get => _hidePlayers;
            set
            {
                _hidePlayers = value;
                if (value)
                {
                    proxy.RegisterHandler(HandlerHidePlayers);
                    DestroyPlayers();
                }
                else
                {
                    proxy.UnregisterHandler(HandlerHidePlayers);
                }
            }
        }
        
        public bool InfiniteRange
        {
            get => _infRange;
            set
            {
                _infRange = value;
                if (value)
                {
                    SetInfiniteRange();
                }
            }
        }

        public int WalkSpeed
        {
            get;
            set;
        }

        public int Timer
        {
            get;
            set;
        } = 250;

        public bool Packet
        {
            get;
            set;
        }

        public bool Untarget
        {
            get;
            set;
        }

        public bool AFK
        {
            get => _afk;
            set
            {
                _afk = value;
                if (value)
                    proxy.RegisterHandler(HandlerAFK1);
                else
                    proxy.UnregisterHandler(HandlerAFK1);
            }
        }

        public bool AFK2
        {
            get => _afk2;
            set
            {
                _afk2 = value;
                if (value)
                    proxy.RegisterHandler(HandlerAFK2);
                else
                    proxy.UnregisterHandler(HandlerAFK2);

            }
        }

        public bool HideRoom
        {
            get => _hideRoom;
            set
            {
                _hideRoom = value;
                if (value)
                    proxy.RegisterHandler(HandlerHideRoom);
                else
                    proxy.UnregisterHandler(HandlerHideRoom);
            }
        }

        public bool ChangeChat
        {
            get;
            set;
        }

        public int? SetLevelOnJoin
        {
            get;
            set;
        }

        private readonly string[] empty = new string[0];

        public event Action<bool> StateChanged;

        private void SetInfiniteRange() => flash.Call("SetInfiniteRange", empty);

        private void SetProvokeMonsters() => flash.Call("SetProvokeMonsters", empty);

        private void SetEnemyMagnet() => flash.Call("SetEnemyMagnet", empty);

        private void SetLagKiller() => flash.Call("SetLagKiller", LagKiller ? bool.TrueString : bool.FalseString);

        public void DestroyPlayers() => flash.Call("DestroyPlayers", empty);

        private void SetSkipCutscenes() => flash.Call("SetSkipCutscenes", empty);

        public void SetWalkSpeed() => flash.Call("SetWalkSpeed", WalkSpeed.ToString());

        public void Start()
        {
            if (!IsRunning)
            {
                ApplySettings();
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private async Task ApplySettings()
        {
            IsRunning = true;
            while (IsRunning && player.IsLoggedIn)
            {
                bool flagprovoke = ProvokeMonsters && player.IsAlive && botData.BotState != BotData.State.Move && botData.BotState != BotData.State.Rest && botData.BotState != BotData.State.Transaction;
                if (flagprovoke)
                {
                    if (botData.BotState == BotData.State.Quest)
                    {
                        await Task.Delay(1500);
                        SetProvokeMonsters();
                        botData.BotState = BotData.State.Combat;
                    }
                    SetProvokeMonsters();
                }
                if (EnemyMagnet && player.IsAlive)
                    SetEnemyMagnet();
                if (Untarget)
                    player.CancelTargetSelf();
                if (Buff)
                    player.SetBuff();
                if (SkipCutscenes)
                    SetSkipCutscenes();
                SetWalkSpeed();
                SetLagKiller();
                await Task.Delay(millisecondsDelay: Timer);
            }
        }
        
        private IJsonMessageHandler HandlerDisableAnimations
        {
            get;
        }

        private IXtMessageHandler HandlerHidePlayers
        {
            get;
        }

        private IXtMessageHandler HandlerYulgar
        {
            get;
        }

        private IJsonMessageHandler HandlerHideRoom
        {
            get;
        }

        private IJsonMessageHandler HandlerRange
        {
            get;
        }
        
        private IXtMessageHandler HandlerAFK1
        {
            get;
        }
        
        private IXtMessageHandler HandlerAFK2
        {
            get;
        }

        public OptionsManager()
        {
            HandlerDisableAnimations = new HandlerAnimations();
            HandlerHidePlayers = new HandlerPlayers(this, player);
            HandlerYulgar = new HandlerXtCellJoin();
            HandlerAFK1 = new HandlerAFK(bot, player);
            HandlerAFK2 = new HandlerAFK2();
            WalkSpeed = 8;
        }
    }
}