using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Quest
{
    public class CmdCompleteQuest : IBotCommand
    {
        public Game.Data.Quest Quest
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            World world = instance.world;
            await instance.WaitUntil(() => world.IsActionAvailable(LockActions.TryQuestComplete));
            if (instance.player.Quests.CanComplete(Quest.Id))
            {
                return;
            }
            //BotData.BotState = BotData.State.Quest;
            string pCell = instance.player.Cell;
            string pPad = instance.player.Pad;
            /*
            int tried = 0;
            while (instance.Configuration.EnsureComplete && Quest.IsInProgress && tried++ < instance.Configuration.EnsureTries)
            {
                if (instance.Configuration.ExitCombatBeforeQuest && instance.player.CurrentState == instance.player.State.InCombat)
                {
                    instance.player.MoveToCell("Blank", "Left");
                    await Task.Delay(300);
                    instance.player.MoveToCell("Blank", "Left");
                    await Task.Delay(300);
                }
                await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.TryQuestComplete));
                Quest.Complete();
                await Task.Delay(400);
            }
            */
            if (instance.Configuration.ExitCombatBeforeQuest)
            {
                while (instance.IsRunning && instance.player.CurrentState == Player.State.InCombat)
                {
                    botData.BotState = BotData.State.Quest;
                    instance.player.MoveToCell(instance.player.Cell, instance.player.Pad);
                    await Task.Delay(1000);
                }
            }
            if (instance.player.CurrentState == Player.State.InCombat)
            {
                await Task.Delay(1250);
            }
            Quest.Complete();
            await instance.WaitUntil(() => !instance.player.Quests.IsInProgress(Quest.Id));
            /*
            if (instance.Configuration.ExitCombatBeforeQuest && player.Cell != pCell)
            {
                instance.player.MoveToCell(pCell, pPad);
            }
            */
        }

        public override string ToString()
        {
            return "Complete quest: " + ((Quest.ItemId != null && Quest.ItemId != "0") ? $"{Quest.Id}:{Quest.ItemId}" : Quest.Id.ToString());
        }
    }
}