using System;
using System.Threading.Tasks;
using Grimoire.Botting;
using Grimoire.Game;

namespace Grimoire.Botting.Commands.Combat
{
    public class CmdUseSkill : IBotCommand
    {
        public string Skill { get; set; }
        
        public string Index { get; set; }
        
        public int SafeHp { get; set; }
        
        public int SafeMp { get; set; }
        
        public bool Wait { get; set; }
        
        public async Task Execute(IBotEngine instance)
        {
            Player player = instance.player;
            if (this.Wait)
            {
                await Task.Delay(player.SkillAvailable(this.Index));
            }
            if (player.Health / (double)player.HealthMax * 100.0 <= SafeHp)
            {
                if (player.Mana / (double)player.ManaMax * 100.0 <= SafeMp)
                {
                    if (this.Index != "5")
                    {
                        player.AttackMonster("*");
                    }
                    player.UseSkill(this.Index);
                }
            }
        }
        
        public override string ToString()
        {
            return "Skill " + this.Skill;
        }
    }
}