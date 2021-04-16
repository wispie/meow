using Grimoire.Game;
using System;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Map
{
    public class CmdWalk : IBotCommand
    {
        public string X
        {
            get;
            set;
        }

        public string Y
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public async Task Execute(IBotEngine instance)
        {
            BotData botData = instance.botData;
            Player player = instance.player;
            botData.BotState = BotData.State.Others;
            if (Type == "Random")
            {
                player.WalkToPoint(y: new Random().Next(320, 450).ToString(), x: new Random().Next(150, 700).ToString());
                await Task.Delay(1000);
            }
            else
            {
                player.WalkToPoint(X, Y);
                await instance.WaitUntil(delegate
                {
                    float[] position = player.Position;
                    return position[0].ToString() == X && position[1].ToString() == Y;
                });
            }
        }

        public override string ToString()
        {
            if (Type == "Random")
            {
                return "Walk Randomly";
            }
            return "Walk to: " + X + ", " + Y;
        }
    }
}