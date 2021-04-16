using Grimoire.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc
{
    public class CmdGotoPlayer : RegularExpression, IBotCommand
    {
        public string PlayerName
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            Player player = instance.player;
            World world = instance.world;
            Configuration configuration = instance.Configuration;
            string TargetName = "";

            if ( IsVar(PlayerName) )
            {
                TargetName = instance.Configuration.Tempvariable[GetVar(PlayerName)];
                Console.WriteLine("Using Variable Goto");
            }
            else
            {
                TargetName = PlayerName;
            }

            List<string> playersInMap = world.PlayersInMap;
            player.GoToPlayer(TargetName);
            if (playersInMap.Any((string p) => p.Equals(TargetName, StringComparison.OrdinalIgnoreCase)))
            {
                await Task.Delay(500);
            }
            else
            {
                await instance.WaitUntil(() => world.PlayersInMap.Any((string p) => p.Equals(TargetName, StringComparison.OrdinalIgnoreCase)) && !instance.world.IsMapLoading, null, 40);
            }
        }

        public override string ToString()
        {
            return "Goto player: " + PlayerName;
        }
    }
}