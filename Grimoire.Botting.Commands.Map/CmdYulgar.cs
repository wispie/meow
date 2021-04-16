using Grimoire.Game;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Map
{
    public class CmdYulgar : IBotCommand
    {
        public async Task Execute(IBotEngine instance)
        {
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.Transfer));
            if (instance.player.CurrentState == Player.State.InCombat)
            {
                instance.player.MoveToCell(instance.player.Cell, instance.player.Pad);
                await Task.Delay(1250);
            }
            if (!instance.player.Map.Equals("yulgar", StringComparison.OrdinalIgnoreCase))
            {
                instance.player.JoinMap("yulgar", "Enter", "Spawn");
                await instance.WaitUntil(() => instance.player.Map.Equals("yulgar", StringComparison.OrdinalIgnoreCase));
                await instance.WaitUntil(() => !instance.world.IsMapLoading, null, 40);
            }
            instance.player.WalkToPoint(y: new Random().Next(320, 450).ToString(), x: new Random().Next(150, 700).ToString());
            instance.Stop();
        }

        public override string ToString()
        {
            return string.Concat("Join yulgar");
        }
    }
}