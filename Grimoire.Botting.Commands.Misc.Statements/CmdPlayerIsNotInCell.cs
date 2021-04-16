﻿using Grimoire.Game;
using System;
using System.Linq;
using System.Threading.Tasks;
using Grimoire.Tools;

namespace Grimoire.Botting.Commands.Misc.Statements
{
    public class CmdPlayerIsNotInCell : StatementCommand, IBotCommand
    {
        public CmdPlayerIsNotInCell()
        {
            Tag = "Player";
            Text = "Player is not in cell";
        }

        public Task Execute(IBotEngine instance)
        {
            string Val1;
            string Val2;

            if (IsVar(Value1))
            {
                Val1 = instance.Configuration.Tempvariable[GetVar(Value1)];
            }
            else
            {
                Val1 = Value1;
            }

            if (IsVar(Value2))
            {
                Val2 = instance.Configuration.Tempvariable[GetVar(Value2)];
            }
            else
            {
                Val2 = Value2;
            }

            string reqs = instance.flash.Call<string>("CheckCellPlayer", new string[] {
                Val1,
                Val2
            });
            bool isExists = bool.Parse(reqs);

            if (isExists)
            {
                instance.Index++;
            }

            Console.WriteLine(isExists);
            return Task.FromResult<object>(null);
        }

        public override string ToString()
        {
            return "Player is not in cell: " + Value1;
        }
    }
}