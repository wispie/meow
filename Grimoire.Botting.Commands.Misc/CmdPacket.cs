using Grimoire.Game;
using Grimoire.Networking;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc
{
    public class CmdPacket : RegularExpression, IBotCommand
    {
        public string Packet
        {
            get;
            set;
        }

        public bool Client
        {
            get;
            set;
        } = false;

        public async Task Execute(IBotEngine instance)
        {
            Player player = instance.player;
            World world = instance.world;
            Proxy proxy = instance.proxy;
            string text;

            if ( IsVar(Packet) )
            {
                text = instance.Configuration.Tempvariable[GetVar(Packet)];
            }
            else
            {
                text = Packet;
            }
            
            text = text.Replace("{ROOM_ID}", world.RoomId.ToString()).Replace("{ROOM_NUMBER}", world.RoomNumber.ToString()).Replace("PLAYERNAME", player.Username);
            text = text.Replace("{GETMAP}", player.Map);
            while (text.Contains("--"))
            {
                text = new Regex("-{1,}", RegexOptions.IgnoreCase).Replace(text, (Match m) => "-");
            }
            text = new Regex("(1e)[0-9]{1,}", RegexOptions.IgnoreCase).Replace(text, (Match m) => "100000");
            if (Client)
                await proxy.SendToClient(text);
            else
                await proxy.SendToServer(text);
            if (text.Contains("%xt%zm%gar%"))
                await Task.Delay(700);
            else
                await Task.Delay(2000);
        }

        public override string ToString()
        {
            return (Client ? "Send client packet: " : "Send packet: ") + Packet;
        }
    }
}