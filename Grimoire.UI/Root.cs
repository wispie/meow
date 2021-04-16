using AxShockwaveFlashObjects;
using Grimoire.Game;
using Grimoire.Networking;
using Grimoire.Properties;
using Grimoire.Tools;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grimoire.Botting;
using Grimoire.Game.Data;
using System.Diagnostics;
using Unity3.Eyedropper;
using EoL;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using DarkUI.Controls;
using DarkUI.Forms;
using EasyTabs;
using System.Net.NetworkInformation;
using System.Net;
using System.Linq;

namespace Grimoire.UI
{
    public class Root : DarkForm
    {
        protected TitleBarTabs ParentTabs
        {
            get
            {
                return (ParentForm as TitleBarTabs);
            }
        }

        private IContainer components;
        private AxShockwaveFlash flashPlayer;
        private DarkComboBox cbPads;
        private DarkComboBox cbCells;
        private ProgressBar prgLoader;
        private DarkButton btnJump;
        private ToolStripMenuItem botToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem pluginManagerToolStripMenuItem;
        private ToolStripMenuItem logsToolStripMenuItem1;
        public ToolStripMenuItem optionsToolStripMenuItem;
        public ToolStripMenuItem infRangeToolStripMenuItem;
        public ToolStripMenuItem provokeToolStripMenuItem1;
        public ToolStripMenuItem enemyMagnetToolStripMenuItem;
        public ToolStripMenuItem lagKillerToolStripMenuItem;
        public ToolStripMenuItem hidePlayersToolStripMenuItem;
        public ToolStripMenuItem skipCutscenesToolStripMenuItem;
        public ToolStripMenuItem disableAnimationsToolStripMenuItem;
        private ToolStripMenuItem bankToolStripMenuItem1;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripMenuItem setFPSToolStripMenuItem;
        private ToolStripTextBox toolStripTextBox1;
        private DarkMenuStrip darkMenuStrip1;
        private ToolStripMenuItem spammerToolStripMenuItem;
        private ToolStripMenuItem reloadToolStripMenuItem;
        private BotManager botManager;
        public AxShockwaveFlash Client => flashPlayer;
        private PluginManager pluginManager = new PluginManager();
        public Flash flash;
        private Proxy proxy;
        private OptionsManager optionsManager;
        private Player player;
        private Bot bot;
        private World world;
        private BotData botData;
        private AutoRelogin autoRelogin;
        private Shop shop;
        private Configuration configuration;
        private BotUtilities botUtilities;
        private Spammer spammer;
        private PacketSpammer packetSpammer;
        private LogForm logForm;
        private string tabText;
        private KeyboardHook keyboardHook;

        public static bool FindAvailablePort(out int port)
        {
            Random random = new Random();
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] activeTcpConnections;
            IPEndPoint[] activeTcpListeners;
            try
            {
                activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
                activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
            }
            catch (NetworkInformationException)
            {
                port = 0;
                return false;
            }
            int randPort;
            TcpConnectionInformation tcpConnectionInformation;
            IPEndPoint iPEndPoint;
            do
            {
                randPort = random.Next(1001, 65535);
                tcpConnectionInformation = activeTcpConnections.FirstOrDefault((TcpConnectionInformation c) => c.LocalEndPoint.Port == randPort);
                iPEndPoint = activeTcpListeners.FirstOrDefault((IPEndPoint l) => l.Port == randPort);
            }
            while (tcpConnectionInformation != null || iPEndPoint != null);
            port = randPort;
            return true;
        }

        public Root(string TabText)
        {
            if (FindAvailablePort(out int port))
            {
                Bypass.Hook();
                proxy = new Proxy(player, this, flash, logForm, optionsManager, world, botManager, configuration, port);
                flash = new Flash(this, botManager, proxy);
                InitializeComponent();
                player = new Player(flash);
                world = new World(flash, player, logForm);
                botData = new BotData();
                configuration = new Configuration();
                shop = new Shop(flash);
                logForm = new LogForm()
                {
                    Text = tabText + "\'s Logs"
                };
                optionsManager = new OptionsManager(player, flash, proxy, botData, bot);
                botUtilities = new BotUtilities(flash, player, Client);
                botManager = new BotManager(bot, this, proxy, flash, optionsManager, player, configuration, spammer, logForm)
                {
                    Text = tabText + "\'s BotManager"
                };
                autoRelogin = new AutoRelogin(player, botManager, flash, optionsManager, world);
                bot = new Bot(proxy, autoRelogin, flash, world, botData, player, optionsManager, bot, botManager, shop, logForm);
                spammer = new Spammer(proxy);
                packetSpammer = new PacketSpammer(spammer, proxy)
                {
                    Text = tabText + "\'s Packet Spammer"
                };
            }
        }

        private void Root_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(proxy.Start, TaskCreationOptions.LongRunning);
            flashPlayer.FlashCall += flash.ProcessFlashCall;
            this.OnLoadProgress(100);
            InitFlashMovie();
            Config c = Config.Load(Application.StartupPath + "\\config.cfg");
        }

        private void OnLoadProgress(int progress)
        {
            if (progress < prgLoader.Maximum)
            {
                prgLoader.Value = progress;
                return;
            }
            flash.SwfLoadProgress -= OnLoadProgress;
            flashPlayer.Visible = true;
            prgLoader.Visible = false;
        }

        private void botToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(botManager);
        }

        private void pluginManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(pluginManager);
        }
        
        private void spammerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(packetSpammer);
        }
        
        public void ShowForm(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
                form.Show();
                form.BringToFront();
                form.Focus();
                return;
            }
            else if (form.Visible)
            {
                form.Hide();
                return;
            }
            form.Show();
            form.BringToFront();
            form.Focus();
        }

        private MemoryStream memoryStream = new MemoryStream();
        private void InitFlashMovie()
        {
            byte[] aqlitegrimoire;

            if (!System.Diagnostics.Debugger.IsAttached)
                aqlitegrimoire = new Resources().aqlitegrimoire;
            else
                aqlitegrimoire = new Resources().aqlitegrimoire;
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(8 + aqlitegrimoire.Length);
                    binaryWriter.Write(1432769894);
                    binaryWriter.Write(aqlitegrimoire.Length);
                    binaryWriter.Write(aqlitegrimoire);
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    flashPlayer.OcxState = new AxHost.State(memoryStream, 1, manualUpdate: false, null);
                }
            Bypass.Unhook();
        }

        private void btnBank_Click(object sender, EventArgs e)
        {
            player.Bank.Show();
        }

        private void cbCells_Click(object sender, EventArgs e)
        {
            cbCells.Items.Clear();
            ComboBox.ObjectCollection items = cbCells.Items;
            object[] cells = world.Cells;
            object[] items2 = cells;
            items.AddRange(items2);
        }

        private void Root_FormClosing(object sender, FormClosingEventArgs e)
        {
            keyboardHook.Dispose();
            proxy.Stop(appClosing: true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Root));
            this.cbPads = new DarkUI.Controls.DarkComboBox();
            this.cbCells = new DarkUI.Controls.DarkComboBox();
            this.prgLoader = new System.Windows.Forms.ProgressBar();
            this.flashPlayer = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.btnJump = new DarkUI.Controls.DarkButton();
            this.botToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.spammerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.provokeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.enemyMagnetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lagKillerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hidePlayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skipCutscenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAnimationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.bankToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.flashPlayer)).BeginInit();
            this.darkMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbPads
            // 
            this.cbPads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPads.FormattingEnabled = true;
            this.cbPads.Items.AddRange(new object[] {
            "Center",
            "Spawn",
            "Left",
            "Right",
            "Top",
            "Bottom",
            "Up",
            "Down"});
            this.cbPads.Location = new System.Drawing.Point(805, 5);
            this.cbPads.Name = "cbPads";
            this.cbPads.Size = new System.Drawing.Size(91, 21);
            this.cbPads.TabIndex = 17;
            // 
            // cbCells
            // 
            this.cbCells.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCells.FormattingEnabled = true;
            this.cbCells.Location = new System.Drawing.Point(708, 5);
            this.cbCells.Name = "cbCells";
            this.cbCells.Size = new System.Drawing.Size(91, 21);
            this.cbCells.TabIndex = 18;
            this.cbCells.Click += new System.EventHandler(this.cbCells_Click);
            // 
            // prgLoader
            // 
            this.prgLoader.Location = new System.Drawing.Point(12, 276);
            this.prgLoader.Name = "prgLoader";
            this.prgLoader.Size = new System.Drawing.Size(936, 23);
            this.prgLoader.TabIndex = 21;
            // 
            // flashPlayer
            // 
            this.flashPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flashPlayer.Enabled = true;
            this.flashPlayer.Location = new System.Drawing.Point(2, 29);
            this.flashPlayer.Name = "flashPlayer";
            this.flashPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashPlayer.OcxState")));
            this.flashPlayer.Size = new System.Drawing.Size(956, 546);
            this.flashPlayer.TabIndex = 2;
            this.flashPlayer.Visible = false;
            // 
            // btnJump
            // 
            this.btnJump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJump.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.btnJump.Checked = false;
            this.btnJump.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnJump.Location = new System.Drawing.Point(902, 3);
            this.btnJump.Name = "btnJump";
            this.btnJump.Size = new System.Drawing.Size(53, 23);
            this.btnJump.TabIndex = 28;
            this.btnJump.Text = "Jump";
            this.btnJump.Click += new System.EventHandler(this.btnJump_Click);
            // 
            // botToolStripMenuItem
            // 
            this.botToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.botToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.botToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.botToolStripMenuItem.Name = "botToolStripMenuItem";
            this.botToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.botToolStripMenuItem.Text = "Bot";
            this.botToolStripMenuItem.Click += new System.EventHandler(this.botToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.startToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginManagerToolStripMenuItem,
            this.logsToolStripMenuItem1,
            this.spammerToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // pluginManagerToolStripMenuItem
            // 
            this.pluginManagerToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.pluginManagerToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.pluginManagerToolStripMenuItem.Name = "pluginManagerToolStripMenuItem";
            this.pluginManagerToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.pluginManagerToolStripMenuItem.Text = "Plugin manager";
            this.pluginManagerToolStripMenuItem.Click += new System.EventHandler(this.pluginManagerToolStripMenuItem_Click);
            // 
            // logsToolStripMenuItem1
            // 
            this.logsToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.logsToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.logsToolStripMenuItem1.Name = "logsToolStripMenuItem1";
            this.logsToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
            this.logsToolStripMenuItem1.Text = "Logs";
            this.logsToolStripMenuItem1.Click += new System.EventHandler(this.logsToolStripMenuItem1_Click);
            // 
            // spammerToolStripMenuItem
            // 
            this.spammerToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.spammerToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.spammerToolStripMenuItem.Name = "spammerToolStripMenuItem";
            this.spammerToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.spammerToolStripMenuItem.Text = "Spammer";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infRangeToolStripMenuItem,
            this.provokeToolStripMenuItem1,
            this.enemyMagnetToolStripMenuItem,
            this.lagKillerToolStripMenuItem,
            this.hidePlayersToolStripMenuItem,
            this.skipCutscenesToolStripMenuItem,
            this.disableAnimationsToolStripMenuItem,
            this.setFPSToolStripMenuItem});
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // infRangeToolStripMenuItem
            // 
            this.infRangeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.infRangeToolStripMenuItem.CheckOnClick = true;
            this.infRangeToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.infRangeToolStripMenuItem.Name = "infRangeToolStripMenuItem";
            this.infRangeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.infRangeToolStripMenuItem.Text = "Infinite Range";
            this.infRangeToolStripMenuItem.Click += new System.EventHandler(this.infRangeToolStripMenuItem_Click);
            // 
            // provokeToolStripMenuItem1
            // 
            this.provokeToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.provokeToolStripMenuItem1.CheckOnClick = true;
            this.provokeToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.provokeToolStripMenuItem1.Name = "provokeToolStripMenuItem1";
            this.provokeToolStripMenuItem1.Size = new System.Drawing.Size(176, 22);
            this.provokeToolStripMenuItem1.Text = "Provoke";
            this.provokeToolStripMenuItem1.Click += new System.EventHandler(this.provokeToolStripMenuItem1_Click);
            // 
            // enemyMagnetToolStripMenuItem
            // 
            this.enemyMagnetToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.enemyMagnetToolStripMenuItem.CheckOnClick = true;
            this.enemyMagnetToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.enemyMagnetToolStripMenuItem.Name = "enemyMagnetToolStripMenuItem";
            this.enemyMagnetToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.enemyMagnetToolStripMenuItem.Text = "Enemy Magnet";
            this.enemyMagnetToolStripMenuItem.Click += new System.EventHandler(this.enemyMagnetToolStripMenuItem_Click);
            // 
            // lagKillerToolStripMenuItem
            // 
            this.lagKillerToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lagKillerToolStripMenuItem.CheckOnClick = true;
            this.lagKillerToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lagKillerToolStripMenuItem.Name = "lagKillerToolStripMenuItem";
            this.lagKillerToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.lagKillerToolStripMenuItem.Text = "Lag Killer";
            this.lagKillerToolStripMenuItem.Click += new System.EventHandler(this.lagKillerToolStripMenuItem_Click);
            // 
            // hidePlayersToolStripMenuItem
            // 
            this.hidePlayersToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.hidePlayersToolStripMenuItem.CheckOnClick = true;
            this.hidePlayersToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hidePlayersToolStripMenuItem.Name = "hidePlayersToolStripMenuItem";
            this.hidePlayersToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.hidePlayersToolStripMenuItem.Text = "Hide Players";
            this.hidePlayersToolStripMenuItem.Click += new System.EventHandler(this.hidePlayersToolStripMenuItem_Click);
            // 
            // skipCutscenesToolStripMenuItem
            // 
            this.skipCutscenesToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.skipCutscenesToolStripMenuItem.CheckOnClick = true;
            this.skipCutscenesToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.skipCutscenesToolStripMenuItem.Name = "skipCutscenesToolStripMenuItem";
            this.skipCutscenesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.skipCutscenesToolStripMenuItem.Text = "Skip Cutscenes";
            this.skipCutscenesToolStripMenuItem.Click += new System.EventHandler(this.skipCutscenesToolStripMenuItem_Click);
            // 
            // disableAnimationsToolStripMenuItem
            // 
            this.disableAnimationsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.disableAnimationsToolStripMenuItem.CheckOnClick = true;
            this.disableAnimationsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.disableAnimationsToolStripMenuItem.Name = "disableAnimationsToolStripMenuItem";
            this.disableAnimationsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.disableAnimationsToolStripMenuItem.Text = "Disable Animations";
            this.disableAnimationsToolStripMenuItem.Click += new System.EventHandler(this.disableAnimationsToolStripMenuItem_Click);
            // 
            // setFPSToolStripMenuItem
            // 
            this.setFPSToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.setFPSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.setFPSToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.setFPSToolStripMenuItem.Name = "setFPSToolStripMenuItem";
            this.setFPSToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.setFPSToolStripMenuItem.Text = "Set FPS";
            this.setFPSToolStripMenuItem.Click += new System.EventHandler(this.setFPSToolStripMenuItem_Click);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            // 
            // bankToolStripMenuItem1
            // 
            this.bankToolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.bankToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem});
            this.bankToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.bankToolStripMenuItem1.Name = "bankToolStripMenuItem1";
            this.bankToolStripMenuItem1.Size = new System.Drawing.Size(45, 20);
            this.bankToolStripMenuItem1.Text = "Bank";
            this.bankToolStripMenuItem1.Click += new System.EventHandler(this.bankToolStripMenuItem1_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.reloadToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // darkMenuStrip1
            // 
            this.darkMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkMenuStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkMenuStrip1.GripMargin = new System.Windows.Forms.Padding(2);
            this.darkMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.botToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.bankToolStripMenuItem1});
            this.darkMenuStrip1.Location = new System.Drawing.Point(2, 2);
            this.darkMenuStrip1.Name = "darkMenuStrip1";
            this.darkMenuStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.darkMenuStrip1.Size = new System.Drawing.Size(956, 24);
            this.darkMenuStrip1.TabIndex = 35;
            this.darkMenuStrip1.Text = "darkMenuStrip1";
            // 
            // Root
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(960, 575);
            this.Controls.Add(this.btnJump);
            this.Controls.Add(this.prgLoader);
            this.Controls.Add(this.cbCells);
            this.Controls.Add(this.cbPads);
            this.Controls.Add(this.flashPlayer);
            this.Controls.Add(this.darkMenuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Root";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GrimLite";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Root_FormClosing);
            this.Load += new System.EventHandler(this.Root_Load);
            ((System.ComponentModel.ISupportInitialize)(this.flashPlayer)).EndInit();
            this.darkMenuStrip1.ResumeLayout(false);
            this.darkMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Instance_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnBankReload_Click(object sender, EventArgs e)
        {
            proxy.SendToServer($"%xt%zm%loadBank%{world.RoomId}%All%");
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm(logForm);
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            string Cell = (string)this.cbCells.SelectedItem;
            string Pad = (string)this.cbPads.SelectedItem;
            player.MoveToCell(Cell ?? player.Cell, Pad ?? player.Pad);
        }

        private void discordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.io/AQWBots");
        }

        private void botRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSd2NSx1ezF-6bc2jRBuTniIka5z6kA2NbmC8CRCOFtpVxcRCA/viewform");
        }

        private void grimoireSuggestionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://docs.google.com/forms/d/e/1FAIpQLSetfV9zl18G9s7w_XReJ1yJNT9aZwxB1FLzU0l1UhdmXv5rIw/viewform?usp=sf_link");
        }

        private async void btnStop_ClickAsync(object sender, EventArgs e)
        {
            if (configuration.Items != null && configuration.BankOnStop)
            {
                foreach (InventoryItem item in player.Inventory.Items)
                {
                    if (!item.IsEquipped && item.IsAcItem && item.Category != "Class" && item.Name.ToLower() != "treasure potion" && configuration.Items.Contains(item.Name))
                    {
                        player.Bank.TransferToBank(item.Name);
                        await Task.Delay(70);
                        logForm.AppendDebug("Transferred to Bank: " + item.Name + "\r\n");
                    }
                }
                logForm.AppendDebug("Banked all AC Items in Items list \r\n");
            }
            startToolStripMenuItem.Enabled = false;
            botManager.ActiveBotEngine.Stop();
            botManager.MultiMode();
            await Task.Delay(2000);
            botManager.BotStateChanged(IsRunning: false);
            this.BotStateChanged(IsRunning: false);
            startToolStripMenuItem.Enabled = true;
        }

        public void BotStateChanged(bool IsRunning)
        {
            return;
            //if (IsRunning)
            //{
            //    btnStart.Hide();
            //    btnStop.Show();
            //}
            //else
            //{
            //    btnStop.Hide();
            //    btnStart.Show();
            //}
        }

        private void nTray_MouseClick(object sender, MouseEventArgs e)
        {
            ShowForm(this);
        }

        private void logsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowForm(logForm);
        }

        private void infRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = infRangeToolStripMenuItem.Checked;
            optionsManager.InfiniteRange = check;
            botManager.chkInfiniteRange.Checked = check;
        }

        private void provokeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool check = provokeToolStripMenuItem1.Checked;
            optionsManager.ProvokeMonsters = check;
            botManager.chkProvoke.Checked = check;
        }

        private void enemyMagnetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = enemyMagnetToolStripMenuItem.Checked;
            optionsManager.EnemyMagnet = check;
            botManager.chkMagnet.Checked = check;
        }

        private void lagKillerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = lagKillerToolStripMenuItem.Checked;
            optionsManager.EnemyMagnet = check;
            botManager.chkLag.Checked = check;
        }

        private void hidePlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = hidePlayersToolStripMenuItem.Checked;
            optionsManager.EnemyMagnet = check;
            botManager.chkHidePlayers.Checked = check;
        }

        private void skipCutscenesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = skipCutscenesToolStripMenuItem.Checked;
            optionsManager.EnemyMagnet = check;
            botManager.chkSkipCutscenes.Checked = check;
        }

        private void disableAnimationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool check = disableAnimationsToolStripMenuItem.Checked;
            optionsManager.EnemyMagnet = check;
            botManager.chkDisableAnims.Checked = check;
        }

        

        private void bankToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            player.Bank.Show();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proxy.SendToServer($"%xt%zm%loadBank%{world.RoomId}%All%");
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (player.IsAlive && player.IsLoggedIn && botManager.lstCommands.Items.Count > 0)
            {
                botManager.MultiMode();
                botManager.ActiveBotEngine.IsRunningChanged += botManager.OnIsRunningChanged;
                botManager.ActiveBotEngine.IndexChanged += botManager.OnIndexChanged;
                botManager.ActiveBotEngine.ConfigurationChanged += botManager.OnConfigurationChanged;
                botManager.ActiveBotEngine.Start(botManager.GenerateConfiguration());
                botManager.BotStateChanged(IsRunning: true);
                this.BotStateChanged(IsRunning: true);
            }
        }

        private async void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (configuration.Items != null && configuration.BankOnStop)
            {
                foreach (InventoryItem item in player.Inventory.Items)
                {
                    if (!item.IsEquipped && item.IsAcItem && item.Category != "Class" && item.Name.ToLower() != "treasure potion" && configuration.Items.Contains(item.Name))
                    {
                        player.Bank.TransferToBank(item.Name);
                        await Task.Delay(70);
                        logForm.AppendDebug("Transferred to Bank: " + item.Name + "\r\n");
                    }
                }
                logForm.AppendDebug("Banked all AC Items in Items list \r\n");
            }
            startToolStripMenuItem.Enabled = false;
            botManager.ActiveBotEngine.Stop();
            botManager.MultiMode();
            await Task.Delay(2000);
            botManager.BotStateChanged(IsRunning: false);
            this.BotStateChanged(IsRunning: false);
            startToolStripMenuItem.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start("https://adventurequest.life/");
        }

        private void setFPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flash.Call("SetFPS", int.Parse(toolStripTextBox1.Text));
        }
    }
}
