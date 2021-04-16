using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.UI;
using System;

namespace Grimoire.Networking.Handlers
{
    public class HandlerQuestComplete : IJsonMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "ccqr"
        };
        private Player player;
        private LogForm logForm;
        public HandlerQuestComplete(LogForm newLogForm, Player newPlayer)
        {
            logForm = newLogForm;
            player = newPlayer;
        }
        public void Handle(JsonMessage message)
        {
            var comp = message.DataObject.ToObject<CompletedQuest>();
            player.Quests.OnQuestCompleted(comp);
            logForm.AppendDebug(string.Format("Quest: {0} Completed at {1}:HH:mm:ss} \r\n", comp.ToString(), DateTime.Now));
        }
    }
}