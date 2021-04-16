using Grimoire.Game;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Map
{
    public class CmdTravel : IBotCommand
    {
        public string Map
        {
            get;
            set;
        }

        public string Cell
        {
            get;
            set;
        }

        public string Pad
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            World world = instance.world;
            BotData botData = instance.botData;
            Player player = instance.player;
            botData.BotState = BotData.State.Others;
            await WaitUntil(() => world.IsActionAvailable(LockActions.Transfer));
            string cmdMap = Map.Contains("-") ? Map.Split('-')[0] : Map;
            string map = player.Map;
            if (!cmdMap.Equals(map, StringComparison.OrdinalIgnoreCase))
            {
                await WaitUntil(() => world.IsActionAvailable(LockActions.Transfer));
                if (instance.player.CurrentState == Player.State.InCombat)
                {
                    player.MoveToCell(instance.player.Cell, player.Pad);
                    await WaitUntil(() => player.CurrentState != Player.State.InCombat);
                }
                player.JoinMap(Map, Cell, Pad);
                await WaitUntil(() => player.Map.Equals(cmdMap, StringComparison.OrdinalIgnoreCase));
                await WaitUntil(() => !world.IsMapLoading, 40);
            }
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