using Grimoire.Game;
using Grimoire.Tools.Buyback;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdBuyBack : IBotCommand
    {
        public string ItemName
        {
            get;
            set;
        }

        public int PageNumberCap
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            botData.BotState = BotData.State.Transaction;
            if (!instance.player.Inventory.ContainsItem(ItemName, "*"))
            {
                try
                {
                    await Task.Run(async delegate
                    {
                        using (AutoBuyBack abb = new AutoBuyBack())
                        {
                            await abb.Perform(ItemName, PageNumberCap);
                        }
                    });
                    instance.player.Logout();
                }
                catch
                {
                }
            }
        }

        public override string ToString()
        {
            return "Buy back: " + ItemName;
        }
    }
}