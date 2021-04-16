using Grimoire.Game;
using Grimoire.Game.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdSell : IBotCommand
    {
        public string ItemName
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Player player = instance.player;
            Shop shop = instance.shop;
            botData.BotState = BotData.State.Transaction;
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.SellItem));
            InventoryItem item = player.Inventory.Items.FirstOrDefault((InventoryItem i) => i.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                shop.SellItem(ItemName);
                await instance.WaitUntil(() => !player.Inventory.ContainsItem(item.Name, item.Quantity.ToString()));
            }
        }

        public override string ToString()
        {
            return "Sell: " + ItemName;
        }
    }
}