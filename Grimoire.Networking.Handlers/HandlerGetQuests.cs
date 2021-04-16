using Grimoire.Game;
using Grimoire.Game.Data;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Networking.Handlers
{
    public class HandlerGetQuests : IJsonMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "getQuests"
        };

        private Player player;
        public HandlerGetQuests(Player newPlayer)
        {
            player = newPlayer;
        }
        public void Handle(JsonMessage message)
        {
            Dictionary<int, Quest> dictionary = message.DataObject?["quests"]?.ToObject<Dictionary<int, Quest>>();
            if (dictionary != null && dictionary.Count > 0)
            {
                player.Quests.OnQuestsLoaded(dictionary.Select((KeyValuePair<int, Quest> q) => q.Value).ToList());
            }
        }
    }
}