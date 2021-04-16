using System;
using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.Tools;
using Grimoire.UI;

namespace Grimoire.Networking.Handlers
{
    public class HandlerLogin : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "loginResponse"
        };

        private Configuration configuration;
        private LogForm logForm;
        public HandlerLogin(Configuration newConfig, LogForm newLogForm)
        {
            configuration = newConfig;
            logForm = newLogForm;
        }

        public void Handle(XtMessage message)
        {
            //foreach (string n in Configuration.BlockedPlayers)
            //{
            //    if (instance.player.Username.ToLower() == n)
            //    {
            //        Environment.Exit(0);
            //    }
            //}
            logForm.AppendDebug($"Relogin to server: {configuration.Server.Name} at {DateTime.Now:hh:mm:ss tt} \r\n");
        }
    }
}