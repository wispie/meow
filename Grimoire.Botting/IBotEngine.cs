using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.Networking;
using Grimoire.Tools;
using Grimoire.UI;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting
{
    public interface IBotEngine
    {
        Task WaitUntil(Func<bool> condition, Func<bool> prerequisite = null, int timeout = 15);
        void LoadAllQuests();
        void LoadBankItems();
        
        LogForm logForm
        {
            get;
            set;
        }

        Flash flash
        {
            get;
            set;
        }

        BotData botData
        {
            get;
            set;
        }

        World world
        {
            get;
            set;
        }

        Shop shop
        {
            get;
            set;
        }

        Bot bot
        {
            get;
            set;
        }

        AutoRelogin autoRelogin
        {
            get;
            set;
        }

        Proxy proxy
        {
            get;
            set;
        }

        BotManager botManager
        {
            get;
            set;
        }

        Player player
        {
            get;
            set;
        }

        OptionsManager optionsManager
        {
            get;
            set;
        }

        bool IsRunning
        {
            get;
            set;
        }

        int Index
        {
            get;
            set;
        }

        Configuration Configuration
        {
            get;
            set;
        }

        event Action<bool> IsRunningChanged;

        event Action<int> IndexChanged;

        event Action<Configuration> ConfigurationChanged;

        void Start(Configuration config);

        void Stop();
    }
}