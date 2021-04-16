using System;
using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.UI;

namespace Grimoire.Networking.Handlers
{
    public class HandlerAFK : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "afk"
        };

        private Player player;
        private Bot bot;
        public HandlerAFK(Bot newBot, Player newPlayer)
        {
            bot = newBot;
            player = newPlayer;
        }

        public void Handle(XtMessage message)
        {
            if (message.Arguments[5] == "true" && bot.IsRunning)
                player.Logout();
        }
    }

    public class HandlerAFK2 : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "afk"
        };

        public void Handle(XtMessage message)
        {
            if (message.Arguments[5] == "true")
                message.Send = false;
        }
    }
}