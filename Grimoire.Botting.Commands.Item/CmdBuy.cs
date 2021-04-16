using Grimoire.Game;
using Grimoire.Game.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdBuy : IBotCommand
    {
        public int ShopId
        {
            get;
            set;
        }

        public string ItemName
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Shop shop = instance.shop;
            Player player = instance.player;
            botData.BotState = BotData.State.Transaction;
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.BuyItem));
            shop.ResetShopInfo();
            shop.Load(ShopId);
            await instance.WaitUntil(() => shop.IsShopLoaded);
            InventoryItem i = instance.player.Inventory.Items.FirstOrDefault((InventoryItem item) => item.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
            if (i != null)
            {
                shop.BuyItem(ItemName);
                await instance.WaitUntil(() => player.Inventory.Items.FirstOrDefault((InventoryItem it) => it.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase)).Quantity != i.Quantity);
            }
            else
            {
                shop.BuyItem(ItemName);
                await instance.WaitUntil(() => player.Inventory.Items.FirstOrDefault((InventoryItem it) => it.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase)) != null);
            }
        }

        public override string ToString()
        {
            return "Buy item: " + ItemName;
        }
    }
}