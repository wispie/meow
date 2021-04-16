using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdGetDrop : IBotCommand
    {
        public string ItemName
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            instance.botData.BotState = BotData.State.Others;
            await Task.Delay(500);
            await instance.world.DropStack.GetDrop(ItemName);
            await instance.WaitUntil(() => !instance.world.DropStack.Contains(ItemName));
        }

        public override string ToString()
        {
            return "Get drop: " + ItemName;
        }
    }
}