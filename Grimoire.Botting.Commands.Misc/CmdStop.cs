using System.Threading.Tasks;
using Grimoire.Botting;
using Grimoire;
using Grimoire.Game;
using Grimoire.UI;
using Grimoire.Game.Data;
namespace Grimoire.Botting.Commands.Misc
{
    public class CmdStop : IBotCommand
    {
        public Task Execute(IBotEngine instance)
        {
            LogForm logForm = instance.logForm;
            Configuration configuration = instance.Configuration;
            if (configuration.BankOnStop)
            {
                foreach (InventoryItem item in instance.player.Inventory.Items)
                {
                    if (!item.IsEquipped && item.IsAcItem && item.Category != "Class" && item.Name.ToLower() != "treasure potion" && configuration.Items.Contains(item.Name))
                    {
                        instance.player.Bank.TransferToBank(item.Name);
                        Task.Delay(70);
                        logForm.AppendDebug("Transferred to Bank: " + item.Name + "\r\n");
                    }
                }
                logForm.AppendDebug("Banked all AC Items in Items list \r\n");
            }
            Task.Delay(2000);
            instance.Stop();
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return "Stop bot";
        }
    }
}