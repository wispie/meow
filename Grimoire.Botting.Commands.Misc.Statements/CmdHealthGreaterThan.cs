using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdHealthGreaterThan : StatementCommand, IBotCommand
    {
        public CmdHealthGreaterThan()
        {
            Tag = "This player";
            Text = "Health is greater than";
        }

        public Task Execute(IBotEngine instance)
        {
            if (instance.player.Health <= int.Parse(Value1))
            {
                instance.Index++;
            }
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return "Health is greater than: " + Value1;
        }
    }
}