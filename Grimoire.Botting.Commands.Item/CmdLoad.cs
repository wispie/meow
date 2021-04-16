using Grimoire.Game;
using Grimoire.Game.Data;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdLoad : IBotCommand
    {
        public int ShopId
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Shop shop = instance.shop;
            botData.BotState = BotData.State.Transaction;
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.LoadShop));
            shop.ResetShopInfo();
            shop.Load(ShopId);
            await instance.WaitUntil(() => shop.IsShopLoaded);
        }

        public override string ToString()
        {
            return "Load Shop: " + ShopId;
        }
    }
}