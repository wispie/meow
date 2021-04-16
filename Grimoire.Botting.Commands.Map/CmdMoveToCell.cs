using Grimoire.Game;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Map
{
    public class CmdMoveToCell : IBotCommand
    {
        public string Cell
        {
            get;
            set;
        }

        public string Pad
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Player player = instance.player;
            botData.BotState = BotData.State.Others;
            while (!player.Cell.Equals(Cell, StringComparison.OrdinalIgnoreCase))
            {
                player.MoveToCell(Cell, Pad);
                await Task.Delay(500);
            }
            botData.BotCell = Cell;
            botData.BotPad = Pad;
        }

        public override string ToString()
        {
            return "Move to cell: " + Cell + ", " + Pad;
        }
    }
}