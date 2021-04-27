﻿using Grimoire.Game;
using Grimoire.Game.Data;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdFactionRankGreaterThan : StatementCommand, IBotCommand
    {
        public CmdFactionRankGreaterThan()
        {
            Tag = "This player";
            Text = "Faction Rank is greater than";
        }

        public Task Execute(IBotEngine instance)
        {
            if (Player.Factions.Find((Faction m) => m.Name == (IsVar(Value1)  ? Configuration.Tempvariable[GetVar(Value1)] : Value1)).Rank < int.Parse((IsVar(Value2)  ? Configuration.Tempvariable[GetVar(Value2)] : Value2)))
            {
                instance.Index++;
            }
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return $"{Value1} Rank is greater than: {Value2}";
        }
    }
}