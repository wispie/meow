using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.Tools;

namespace Grimoire.Networking.Handlers
{
    public class HandlerPlayers : IXtMessageHandler
    {
        private OptionsManager optionsManager;
        private Player player;
        public HandlerPlayers(OptionsManager newOptions, Player newPlayer)
        {
            optionsManager = newOptions;
            player = newPlayer;
        }
        public string[] HandledCommands
        {
            get;
        } = new string[2]
        {
            "retrieveUserData",
            "retrieveUserDatas"
        };

        public void Handle(XtMessage message)
        {
            if (optionsManager.HidePlayers && player.Inventory.Items.Count > 0)
            {
                message.Send = message.RawContent.Contains(player.UserID.ToString()) ? true : false;
                optionsManager.DestroyPlayers();
            }
        }
    }
}