using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdIsNotMaxStack : StatementCommand, IBotCommand
    {
        public CmdIsNotMaxStack()
        {
            Tag = "Item";
            Text = "Is not max in inventory";
        }

        public Task Execute(IBotEngine instance)
        {
            if (instance.player.Inventory.ContainsMaxItem(Value1))
            {
                instance.Index++;
            }
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return $"Is not maxed out: {Value1}";
        }
    }
}
