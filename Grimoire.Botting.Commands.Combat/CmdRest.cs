using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Combat
{
    public class CmdRest : IBotCommand
    {
        public bool Full
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            Player player = instance.player;
            instance.botData.BotState = BotData.State.Rest;
            await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.Rest), () => instance.IsRunning && player.IsLoggedIn);
            if (instance.Configuration.ExitCombatBeforeRest)
            {
                while (instance.player.CurrentState == Player.State.InCombat)
                {
                    instance.player.MoveToCell(instance.player.Cell, instance.player.Pad);
                    await Task.Delay(1250);
                }
            }
            instance.player.Rest();
            if (Full)
            {
                await instance.WaitUntil(() => player.Mana >= player.ManaMax && player.Health >= player.HealthMax);
            }
        }

        public override string ToString()
        {
            if (!Full)
            {
                return "Rest";
            }
            return "Rest fully";
        }
    }
}