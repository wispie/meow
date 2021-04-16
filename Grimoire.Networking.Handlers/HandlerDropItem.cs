using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grimoire.Networking.Handlers
{
    public class HandlerDropItem : IJsonMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "dropItem"
        };

        private BotManager botManager;
        private World world;
        public HandlerDropItem(BotManager newBotManager, World newWorld)
        {
            botManager = newBotManager;
            world = newWorld;
        }

        public void Handle(JsonMessage message)
        {
            JToken jToken = message.DataObject?["items"];
            if (jToken != null)
            {
                InventoryItem item = jToken.ToObject<Dictionary<int, InventoryItem>>().First().Value;
                if (botManager.ActiveBotEngine.IsRunning)
                {
                    Configuration configuration = botManager.ActiveBotEngine.Configuration;
                    message.Send = !configuration.EnableRejection || !configuration.Drops.All((string d) => !d.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                }
                world.OnItemDropped(item);
            }
        }
    }
}