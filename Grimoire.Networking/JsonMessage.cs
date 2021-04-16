using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grimoire.Networking
{
    public class JsonMessage : Message
    {
        Player player;
        Bot bot;
        BotManager botManager;
        LogForm logForm;
        World world;
        OptionsManager optionsManager;
        public JsonMessage(BotManager newBotManager, Player newPlayer, Bot newBot, World newWorld, OptionsManager newOptions, LogForm newLogForm)
        {
            world = newWorld;
            logForm = newLogForm;
            player = newPlayer;
            bot = newBot;
            botManager = newBotManager;
            optionsManager = newOptions;
        }

        public JToken Object
        {
            get;
        }

        public JToken DataObject => Object?["b"]?["o"];

        public JsonMessage(string raw)
        {
            try
            {
                RawContent = raw;
                Object = JObject.Parse(raw);
                Command = DataObject?["cmd"]?.Value<string>();
            }
            catch (JsonReaderException)
            {
            }
        }

        public override string ToString()
        {
            return Object.ToString(Formatting.None);
        }
    }
}