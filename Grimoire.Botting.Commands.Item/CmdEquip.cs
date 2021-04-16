using Grimoire.Game;
using Grimoire.Game.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdEquip : IBotCommand
    {
        public string ItemName
        {
            get;
            set;
        }

        public bool Safe
        {
            get;
            set;
        }

        private Player player;
        public async Task Execute(IBotEngine instance)
        {
            player = instance.player;
            BotData botData = instance.botData;
            InventoryItem item = instance.player.Inventory.Items.FirstOrDefault((InventoryItem i) => i.Name.Equals(ItemName, StringComparison.OrdinalIgnoreCase) && i.IsEquippable);
            if (item == null)
            {
                return;
            }
            while (instance.IsRunning && !IsEquipped(item.Id))
            {
                if (!Safe)
                {
                    if (item.Category == "Item")
                        instance.player.EquipPotion(item.Id, item.Description, item.File, item.Name);
                    else
                        instance.player.Equip(item.Id);
                    return;
                }

                botData.BotState = BotData.State.Transaction;
                while (instance.IsRunning && instance.player.CurrentState == Player.State.InCombat)
                {
                    instance.player.MoveToCell(instance.player.Cell, instance.player.Pad);
                    await Task.Delay(1000);
                }
                await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.EquipItem));
                if (item.Category == "Item")
                    instance.player.EquipPotion(item.Id, item.Description, item.File, item.Name);
                else
                    instance.player.Equip(item.Id);
            }
        }

        public bool IsEquipped(int ItemID)
        {
            return player.Inventory.Items.FirstOrDefault((InventoryItem it) => it.IsEquipped && it.Id == ItemID) != null;
        }

        public override string ToString()
        {
            return (Safe ? "Safe" : "Unsafe") + " Equip: " + ItemName;
        }
    }
}