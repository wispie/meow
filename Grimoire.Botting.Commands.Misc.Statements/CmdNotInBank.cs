using Grimoire.Game;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdNotInBank : StatementCommand, IBotCommand
    {
        public CmdNotInBank()
        {
            Tag = "Item";
            Text = "Is not in bank";
        }

        public Task Execute(IBotEngine instance)
        {
            if (Player.Bank.ContainsItem((IsVar(Value1)  ? Configuration.Tempvariable[GetVar(Value1)] : Value1), (IsVar(Value2)  ? Configuration.Tempvariable[GetVar(Value2)] : Value2)))
            {
                instance.Index++;
            }
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return "Item is not in bank: " + Value1 + ", " + Value2;
        }
    }
}