using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Item
{
    public class CmdMapItem : IBotCommand
    {
        public int ItemId
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            World world = instance.world;
            Player player = instance.player;
            botData.BotState = BotData.State.Others;
            await instance.WaitUntil(() => world.IsActionAvailable(LockActions.GetMapItem));
            player.GetMapItem(ItemId);
            await Task.Delay(2000);
        }

        public override string ToString()
        {
            return $"Get map item: {ItemId}";
        }
    }
}