using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.UI;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace Grimoire.Networking.Handlers
{
    public class HandlerXtJoin : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "server"
        };
        private OptionsManager optionsManager;
        private LogForm logForm;
        public HandlerXtJoin(OptionsManager newOptions, LogForm newLogForm)
        {
            optionsManager = newOptions;
            logForm = newLogForm;
        }

        public void Handle(XtMessage message)
        {
            if (!message.RawContent.Contains("You joined "))
                return;
            if (optionsManager.HideRoom)
            {
                Config c = Config.Load(Application.StartupPath + "\\config.cfg");
                message.Arguments[4] =  c.Get("JoinMessage") ?? "You joined a place but... where?";
            }
            logForm.AppendChat(string.Format("[{0:hh:mm:ss}] {1} \r\n", DateTime.Now, message.Arguments[4]));
        }
    }

    public class HandlerXtCellJoin : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "moveToCell"
        };

        public void Handle(XtMessage message)
        {
            //if (message.player.Map.ToLower() == "yulgar" && message.player.Cell.ToLower() == "upstairs" && message.optionsManager.HideYulgar)
            //  message.optionsManager.DestroyPlayers();
        }
    }
}