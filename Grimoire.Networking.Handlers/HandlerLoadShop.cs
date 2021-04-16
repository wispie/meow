using Grimoire.Game;
using Grimoire.Game.Data;
using Newtonsoft.Json.Linq;

namespace Grimoire.Networking.Handlers
{
    public class HandlerLoadShop : IJsonMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "loadShop"
        };

        private World world;
        public HandlerLoadShop(World newWorld)
        {
            world = newWorld;
        }

        public void Handle(JsonMessage message)
        {
            JToken jToken = message.DataObject["shopinfo"];
            if (jToken != null)
            {
                world.OnShopLoaded(jToken.ToObject<ShopInfo>());
            }
        }
    }
}