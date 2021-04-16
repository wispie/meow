using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Combat
{
    public class CmdKillFor : IBotCommand
    {
        public string Monster
        {
            get;
            set;
        }

        public string ItemName
        {
            get;
            set;
        }

        public ItemType ItemType
        {
            get;
            set;
        }

        public string Quantity
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            botData.BotState = BotData.State.Combat;
            CmdKill kill = new CmdKill
            {
                Monster = Monster
            };
            if (ItemType == ItemType.Items)
            {
                while (instance.IsRunning && instance.player.IsLoggedIn && instance.player.IsAlive && !instance.player.Inventory.ContainsItem(ItemName, Quantity))
                {
                    await kill.Execute(instance);
                    await Task.Delay(1000);
                }
            }
            else
            {
                while (instance.IsRunning && instance.player.IsLoggedIn && instance.player.IsAlive && !instance.player.TempInventory.ContainsItem(ItemName, Quantity))
                {
                    await kill.Execute(instance);
                    await Task.Delay(1000);
                }
            }
        }

        public override string ToString()
        {
            return "Kill for " + ((ItemType == ItemType.Items) ? "items" : "tempitems") + ": " + Monster;
        }
    }
}