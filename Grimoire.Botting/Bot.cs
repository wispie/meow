using Grimoire.Botting.Commands.Combat;
using Grimoire.Botting.Commands.Misc;
using Grimoire.Botting.Commands.Misc.Statements;
using Grimoire.Botting.Commands.Quest;
using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.Networking;
using Grimoire.Tools;
using Grimoire.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grimoire.Botting
{
    public class Bot : IBotEngine
    {
        private Flash flash;
        private OptionsManager optionsManager;
        private Player player;
        private Proxy proxy;
        private BotData botData;
        private World world;
        private AutoRelogin autoRelogin;
        private Bot bot;
        private Shop shop;
        private BotManager botManager;
        private LogForm logForm;
        public Bot(Proxy newProxy, AutoRelogin newAutoRelogin ,Flash newFlash, World newWorld, BotData newBotData, 
            Player newPlayer, OptionsManager newOptions, Bot newBot, BotManager newBotManager, Shop newShop, LogForm newLogForm)
        {
            logForm = newLogForm;
            shop = newShop;
            bot = newBot;
            botData = newBotData;
            autoRelogin = newAutoRelogin;
            flash = newFlash;
            proxy = newProxy;
            world = newWorld;
            optionsManager = newOptions;
            player = newPlayer;
            botManager = newBotManager;
        }

        Flash IBotEngine.flash { get => flash; set => flash = value; }
        Player IBotEngine.player { get => player; set => player = value; }
        OptionsManager IBotEngine.optionsManager { get => optionsManager; set => optionsManager = value; }
        Proxy IBotEngine.proxy { get => proxy; set => proxy = value; }
        AutoRelogin IBotEngine.autoRelogin { get => autoRelogin; set => autoRelogin = value; }
        Bot IBotEngine.bot { get => bot; set => bot = value; }
        World IBotEngine.world { get => world; set => world = value; }
        BotData IBotEngine.botData { get => botData; set => botData = value; }
        BotManager IBotEngine.botManager { get => botManager; set => botManager = value; }
        Shop IBotEngine.shop { get => shop; set => shop = value; }
        LogForm IBotEngine.logForm { get => logForm; set => logForm = value; }
        private int _index;

        private Configuration _config;

        private bool _isRunning;

        private CancellationTokenSource _ctsBot;

        private Stopwatch _questDelayCounter;

        private Stopwatch _boostDelayCounter;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = (value < Configuration.Commands.Count) ? value : 0;
            }
        }

        public Configuration Configuration
        {
            get
            {
                return _config;
            }
            set
            {
                if (value != _config)
                {
                    _config = value;
                    this.ConfigurationChanged?.Invoke(_config);
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                this.IsRunningChanged?.Invoke(_isRunning);
            }
        }

        public event Action<bool> IsRunningChanged;

        public event Action<int> IndexChanged;

        public event Action<Configuration> ConfigurationChanged;

        public void Start(Configuration config)
        {
            IsRunning = true;
            Configuration = config;
            Index = 0;
            botData.BotState = BotData.State.Others;
            _ctsBot = new CancellationTokenSource();
            _questDelayCounter = new Stopwatch();
            _boostDelayCounter = new Stopwatch();
            world.ItemDropped += OnItemDropped;
            player.Quests.QuestsLoaded += OnQuestsLoaded;
            player.Quests.QuestCompleted += OnQuestCompleted;
            _questDelayCounter.Start();
            this.LoadAllQuests();
            this.LoadBankItems();
            CheckBoosts();
            _boostDelayCounter.Start();
            optionsManager.Start();
            Task.Factory.StartNew(Activate, _ctsBot.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            botData.BotMap = null;
            botData.BotCell = null;
            botData.BotPad = null;
            botData.BotSkill = null;
            botData.BotState = BotData.State.Others;
            botData.SkillSet.Clear();
            for (int i = 0; i < Configuration.Skills.Count; i++)
            {
                if (Configuration.Skills[i].Type == Skill.SkillType.Label)
                {
                    botData.SkillSet.Add(Configuration.Skills[i].Text.ToUpper(), i);
                }
            }
            if (config.Items.Count > 0)
            {
                player.Bank.LoadItems();
                foreach (string item in config.Items)
                {
                    if (!player.Inventory.ContainsItem(item, "*") && player.Bank.ContainsItem(item))
                    {
                        player.Bank.TransferFromBank(item);
                        Task.Delay(70);
                        logForm.AppendDebug("Transferred from Bank: " + item + "\r\n");
                    }
                    else if (player.Inventory.ContainsItem(item, "*"))
                    {
                        logForm.AppendDebug("Item Already exists in Inventory: " + item + "\r\n");
                    }
                }
            }
            List<InventoryItem> inventory = player.Inventory.Items;
            int num = (from i in Enumerable.Range(0, config.Items.Count)
                       where inventory.Find((InventoryItem x) => x.Name.ToLower() == config.Items[i].ToLower()) == null
                       select i).Count();
            if (config.Items != null && num > player.Inventory.AvailableSlots)
            {
                int num2 = config.Items.Count - num - player.Inventory.AvailableSlots;
                MessageBox.Show(string.Concat
                    (
                        "You don't have enough available inventory slots to use this bot, please bank some items, you need ",
                        config.Items.Count, " Free Inventory spots in total (you need ", num2, " more),"
                    ), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
        }
        
        public void Stop()
        {
            _ctsBot?.Cancel(throwOnFirstException: false);
            world.ItemDropped -= OnItemDropped;
            player.Quests.QuestsLoaded -= OnQuestsLoaded;
            player.Quests.QuestCompleted -= OnQuestCompleted;
            _questDelayCounter.Stop();
            _boostDelayCounter.Stop();
            optionsManager.Stop();
            IsRunning = false;
            botData.BotState = BotData.State.Others;
        }

        private async Task Activate()
        {
            while (true)
            {
                if (_ctsBot.IsCancellationRequested)
                {
                    return;
                }
                if (!player.IsLoggedIn)
                {
                    if (!Configuration.AutoRelogin)
                    {
                        break;
                    }
                    optionsManager.Stop();
                    await autoRelogin.Login(Configuration.Server, Configuration.RelogDelay, _ctsBot, Configuration.RelogRetryUponFailure);
                    Index = 0;
                    this.LoadAllQuests();
                    this.LoadBankItems();
                    optionsManager.Start();
                }
                if (!_ctsBot.IsCancellationRequested)
                {
                    if (player.IsLoggedIn && !player.IsAlive)
                    {
                        world.SetSpawnPoint();
                        await this.WaitUntil(() => player.IsAlive, () => IsRunning && player.IsLoggedIn, -1);
                        Index = (!Configuration.RestartUponDeath) ? (Index - 1) : 0;
                    }
                    if (!_ctsBot.IsCancellationRequested)
                    {
                        if (player.IsAfk)
                        {
                            player.MoveToCell(player.Cell, player.Pad);
                            Index = Index > 0 ? Index-- : Index;
                        }
                        if (Configuration.RestIfHp)
                        {
                            await RestHealth();
                        }
                        if (!_ctsBot.IsCancellationRequested)
                        {
                            if (Configuration.RestIfMp)
                            {
                                await RestMana();
                            }
                            if (!_ctsBot.IsCancellationRequested)
                            {
                                this.IndexChanged?.Invoke(Index);
                                IBotCommand cmd = Configuration.Commands[Index];
                                await cmd.Execute(this);
                                if (!_ctsBot.IsCancellationRequested)
                                {
                                    if (Configuration.BotDelay > 0 && (!Configuration.SkipDelayIndexIf || (Configuration.SkipDelayIndexIf) && !(
                                    cmd is StatementCommand ||
                                    cmd is CmdIndex ||
                                    cmd is CmdLabel ||
                                    cmd is CmdGotoLabel ||
                                    cmd is CmdBlank ||
                                    cmd is CmdSkillSet)))
                                    {
                                        await Task.Delay(_config.BotDelay);
                                    }
                                    if (!_ctsBot.IsCancellationRequested)
                                    {
                                        if (Configuration.Quests.Count > 0)
                                        {
                                            await CheckQuests();
                                        }
                                        if (!_ctsBot.IsCancellationRequested)
                                        {
                                            if (Configuration.Boosts.Count > 0)
                                            {
                                                CheckBoosts();
                                            }
                                            if (!_ctsBot.IsCancellationRequested)
                                            {
                                                Index++;
                                                continue;
                                            }
                                            return;
                                        }
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                return;
            }
            Stop();
        }

        private async Task RestHealth()
        {
            if (player.Health / (double)player.HealthMax <= Configuration.RestHp / 100.0)
            {
                BotData.State TempState = botData.BotState;
                botData.BotState = BotData.State.Rest;
                if (Configuration.ExitCombatBeforeRest)
                {
                    player.MoveToCell(player.Cell, player.Pad);
                    await Task.Delay(500);
                }
                player.Rest();
                await this.WaitUntil(() => player.Health >= player.HealthMax);
                botData.BotState = TempState;
            }
        }

        private async Task RestMana()
        {
            if (player.Mana / (double)player.ManaMax <= Configuration.RestMp / 100.0)
            {
                BotData.State TempState = botData.BotState;
                botData.BotState = BotData.State.Rest;
                if (Configuration.ExitCombatBeforeRest)
                {
                    player.MoveToCell(player.Cell, player.Pad);
                    await Task.Delay(500);
                }
                player.Rest();
                await this.WaitUntil(() => player.Mana >= player.ManaMax);
                botData.BotState = TempState;
            }
        }

        private void CheckBoosts()
        {
            if (_boostDelayCounter.ElapsedMilliseconds >= 10000)
            {
                foreach (InventoryItem boost in Configuration.Boosts)
                {
                    if (!player.HasActiveBoost(boost.Name))
                    {
                        player.UseBoost(boost.Id);
                    }
                }
                _boostDelayCounter.Restart();
            }
        }

        private async Task CheckQuests()
        {
            if (!world.IsActionAvailable(LockActions.TryQuestComplete) || _questDelayCounter.ElapsedMilliseconds < 3000)
            {
                return;
            }
            Quest quest = Configuration.Quests.FirstOrDefault((Quest q) => q.CanComplete);
            if (quest == null)
            {
                return;
            }
            BotData.State TempState = botData.BotState;
            botData.BotState = BotData.State.Quest;
            string pCell = player.Cell;
            string pPad = player.Pad;
            if (_config.ExitCombatBeforeQuest)
            {
                while (player.CurrentState == Player.State.InCombat)
                {
                    player.MoveToCell("Blank", "Left");
                    await Task.Delay(2200);
                }
            }
            quest.Complete();
            if (_config.ExitCombatBeforeQuest && player.Cell != pCell)
            {
                player.MoveToCell(pCell, pPad);
            }
            botData.BotState = TempState;
            _questDelayCounter.Restart();
        }

        public int DropDelay { get; set; } = 1000;

        private void OnItemDropped(InventoryItem drop)
        {
            NotifyDrop(drop);
            bool flag = Configuration.Drops.Any((string d) => d.Equals(drop.Name, StringComparison.OrdinalIgnoreCase));
            if (Configuration.EnablePickupAll)
            {
                Task.Delay(DropDelay);
                world.DropStack.GetDrop(drop.Id);
            }
            else if (Configuration.EnablePickup && flag)
            {
                Task.Delay(DropDelay);
                world.DropStack.GetDrop(drop.Id);
            }

            if (Configuration.EnablePickupAcTagged)
            {
                Task.Delay(DropDelay);
                if (drop.IsAcItem)
                {
                    world.DropStack.GetDrop(drop.Id);
                }
            }

            //else if (Configuration.EnableRejectAll)
            //{
            //    instance.world.DropStack.RemoveAll(drop.Id);
            //}
        }

        private void NotifyDrop(InventoryItem drop)
        {
            if (Configuration.NotifyUponDrop.Count > 0 && Configuration.NotifyUponDrop.Any((string d) => d.Equals(drop.Name, StringComparison.OrdinalIgnoreCase)))
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.Beep();
                }
            }
        }

        private void OnQuestsLoaded(List<Quest> quests)
        {
            List<Quest> qs = quests.Where((Quest q) => Configuration.Quests.Any((Quest qq) => qq.Id == q.Id)).ToList();
            int count = qs.Count;
            if (qs.Count <= 0)
            {
                return;
            }
            if (count == 1)
            {
                qs[0].Accept();
                return;
            }
            for (int i = 0; i < count; i++)
            {
                int ii = i;
                Task.Run(async delegate
                {
                    await Task.Delay(1000 * ii);
                    qs[ii].Accept();
                });
            }
        }

        private void OnQuestCompleted(CompletedQuest quest)
        {
            Configuration.Quests.FirstOrDefault((Quest q) => q.Id == quest.Id)?.Accept();
        }

        public async Task WaitUntil(Func<bool> condition, Func<bool> prerequisite = null, int timeout = 15)
        {
            int iterations = 0;
            while ((prerequisite ?? (() => this.IsRunning && player.IsLoggedIn && player.IsAlive))() && !condition() && (iterations < timeout || timeout == -1))
            {
                await Task.Delay(1000);
                iterations++;
            }
        }

        public void LoadAllQuests()
        {
            List<int> list = new List<int>();
            foreach (IBotCommand command in Configuration.Commands)
            {
                if (command is CmdAcceptQuest cmdAcceptQuest)
                {
                    list.Add(cmdAcceptQuest.Quest.Id);

                }
                else if (command is CmdCompleteQuest cmdCompleteQuest)
                {
                    list.Add(cmdCompleteQuest.Quest.Id);
                }
            }
            list.AddRange(Configuration.Quests.Select((Quest q) => q.Id));
            if (list.Count > 0)
            {
                player.Quests.Get(list);
            }
        }

        public void LoadBankItems()
        {
            if (Configuration.Commands.Any((IBotCommand c) => c is CmdBankSwap || c is CmdBankTransfer))
            {
                player.Bank.LoadItems();
            }
        }
    }
}