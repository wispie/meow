using Grimoire.UI;
using System.Threading.Tasks;
using Grimoire;
using Grimoire.Botting;
using Newtonsoft.Json.Linq;
using System.Windows;
using Grimoire.Game;

namespace Grimoire.Networking.Handlers
{
    public class HandlerMapJoin : IJsonMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "moveToArea"
        };

        private OptionsManager optionsManager;
        public HandlerMapJoin(OptionsManager newOptions)
        {
            optionsManager = newOptions;
        }

        public void Handle(JsonMessage message)
        {
            if (optionsManager.HideRoom)
                message.DataObject["areaName"] = "discord.gg/aqwbots";
            //JToken jToken = message.DataObject["uoBranch"];
            //if (optionsManager.SetLevelOnJoin != null && jToken.Type != JTokenType.Null)
            //{
            //    int i = 0;
            //    foreach(JToken j in jToken)
            //    {
            //        MessageBox.Show(j.ToString());
            //        //if (j["uoName"].ToString() == player.Username.ToLower())
            //        //    j["intLevel"] = optionsManager.SetLevelOnJoin;
            //    }
            //}
            //MessageBox.Show(botManager.CustomName + " 1 \r\n" + message.RawContent.Split(':')[6].Split('-')[0].Replace("\"", "").ToLower());
        }
    }
}