using Grimoire.Game;
using Grimoire.Game.Data;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdLoadTravel : IBotCommand
    {
        public int ShopId
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            Shop shop = instance.shop;
            BotData botData = instance.botData;
            botData.BotState = BotData.State.Transaction;
            await WaitUntil(() => instance.world.IsActionAvailable(LockActions.LoadShop));
            shop.ResetShopInfo();
            shop.Load(ShopId);
            await WaitUntil(() => shop.IsShopLoaded);
        }

        private async Task WaitUntil(Func<bool> condition, int timeout = 15)
        {
            int iterations = 0;
            while (!condition() && (iterations < timeout || timeout == -1))
            {
                await Task.Delay(1000);
                iterations++;
            }
        }
    }
}