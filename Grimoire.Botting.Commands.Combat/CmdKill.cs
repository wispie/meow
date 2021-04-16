using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Combat
{
    public class CmdKill : IBotCommand
    {
        private CancellationTokenSource _cts;

        private int Index;

        public string Monster
        {
            get;
            set;
        }

        public bool Packet
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Player player = instance.player;
            World world = instance.world;
            botData.BotState = BotData.State.Combat;
            if (botData.BotCell != null && !player.Cell.Equals(botData.BotCell, StringComparison.OrdinalIgnoreCase))
            {
                instance.player.MoveToCell(botData.BotCell, botData.BotPad);
                await Task.Delay(1000);
            }
            await instance.WaitUntil(() => world.IsMonsterAvailable(Monster), null, 3);
            if (instance.Configuration.WaitForAllSkills)
            {
                await Task.Delay(instance.player.AllSkillsAvailable);
            }
            if (instance.IsRunning && instance.player.IsAlive && instance.player.IsLoggedIn)
            {
                instance.player.AttackMonster(Monster);
                await Task.Delay(500);
                Task.Run(() => UseSkills(instance));
                await instance.WaitUntil(() => !player.HasTarget, null, 360);
                _cts?.Cancel(throwOnFirstException: false);
            }
        }

        private async Task UseSkills(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Player player = instance.player;
            World world = instance.world;
            _cts = new CancellationTokenSource();
            int ClassIndex = -1;
            if (botData.SkillSet != null && botData.SkillSet.ContainsKey("[" + botData.BotSkill + "]"))
            {
                ClassIndex = botData.SkillSet["[" + botData.BotSkill + "]"] + 1;
            }
            int Count = instance.Configuration.Skills.Count - 1;
            Index = ClassIndex;
            while (!_cts.IsCancellationRequested)
            {
                if (!instance.player.IsLoggedIn || !instance.player.IsAlive)
                {
                    while (instance.player.HasTarget)
                    {
                        instance.player.CancelTarget();
                        await Task.Delay(500);
                    }
                    return;
                }
                if (Monster.ToLower() == "escherion" && world.IsMonsterAvailable("Staff of Inversion"))
                {
                    instance.player.AttackMonster("Staff of Inversion");
                }
                else if (Monster.ToLower() == "vath" && world.IsMonsterAvailable("Stalagbite"))
                {
                    instance.player.AttackMonster("Stalagbite");
                }
                if (ClassIndex != -1)
                {
                    Skill s = instance.Configuration.Skills[Index];
                    if (s.Type == Skill.SkillType.Label)
                    {
                        Index = ClassIndex;
                        continue;
                    }
                    if (instance.Configuration.WaitForSkill)
                    {
                        instance.botManager.OnSkillIndexChanged(Index);
                        await Task.Delay(instance.player.SkillAvailable(s.Index));
                    }
                    if (s.Type == Skill.SkillType.Safe)
                    {
                        if (s.SafeMp)
                        {
                            if (instance.player.Mana / (double)instance.player.ManaMax * 100.0 <= s.SafeHealth)
                            {
                                instance.player.UseSkill(s.Index);
                            }
                        }
                        else if (instance.player.Health / (double)instance.player.HealthMax * 100.0 <= s.SafeHealth)
                        {
                            instance.player.UseSkill(s.Index);
                        }
                    }
                    else
                    {
                        instance.player.UseSkill(s.Index);
                    }
                    int num = Index = (Index >= Count) ? ClassIndex : (++Index);
                }
                else
                {
                    int[] array = new int[4]
                    {
                        instance.player.SkillAvailable("1"),
                        instance.player.SkillAvailable("2"),
                        instance.player.SkillAvailable("3"),
                        instance.player.SkillAvailable("4")
                    };
                    int num2 = array[0];
                    int MinIndex = 1;
                    for (int i = 1; i < 4; i++)
                    {
                        if (array[i] < num2)
                        {
                            num2 = array[i];
                            MinIndex = i + 1;
                        }
                    }
                    await Task.Delay(num2);
                    instance.player.UseSkill(MinIndex.ToString());
                }
                await Task.Delay(instance.Configuration.SkillDelay);
            }
            while (instance.player.HasTarget)
            {
                instance.player.CancelTarget();
                await Task.Delay(500);
            }
        }

        public override string ToString()
        {
            return "Kill " + Monster;
        }
    }
}