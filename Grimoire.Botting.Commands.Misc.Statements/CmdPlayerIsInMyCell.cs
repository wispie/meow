using Grimoire.Game;
using System;
using System.Linq;
using System.Threading.Tasks;
using Grimoire.Tools;
using System.Text.RegularExpressions;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdPlayerIsInMyCell : StatementCommand, IBotCommand
    {
        private Flash flash;
        public CmdPlayerIsInMyCell(Flash newFlash)
        {
            Tag = "Player";
            Text = "Player is in my cell";
            flash = newFlash;
        }

        public Task Execute(IBotEngine instance)
        {
            string reqs;
            if ( IsVar(Value1) )
            {
                reqs = flash.Call<string>("GetCellPlayers", new string[] { instance.Configuration.Tempvariable[GetVar(Value1)] });
            }
            else
            {
                reqs = flash.Call<string>("GetCellPlayers", new string[] { Value1 });
            }

            bool isExists = bool.Parse(reqs);

            if (!isExists)
            {
                instance.Index++;
            }

            Console.WriteLine(isExists);
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return "Player is in my cell: " + Value1;
        }
    }
}