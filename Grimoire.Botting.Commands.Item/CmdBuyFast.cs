using Grimoire.Game;
using Grimoire.Game.Data;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdBuyFast : IBotCommand
    {
        public string ItemName
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Shop shop = instance.shop;
            botData.BotState = BotData.State.Transaction;
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.BuyItem));
            shop.BuyItem(ItemName);
        }

        public override string ToString()
        {
            return "Buy item fast: " + ItemName;
        }
    }
}