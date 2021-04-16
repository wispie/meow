using Grimoire.Game;
using Grimoire.Networking;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Map
{
    public class CmdJoin : IBotCommand
    {
        public string Map
        {
            get;
            set;
        }

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
            World world = instance.world;
            Proxy proxy = instance.proxy;
            Player player = instance.player;
            BotData botData = instance.botData;
            botData.BotState = BotData.State.Move;
            await instance.WaitUntil(() => world.IsActionAvailable(LockActions.Transfer));
            string cmdMap = Map.Contains("-") ? Map.Split('-')[0] : Map;
            string text = Map.Substring(cmdMap.Length);
            if (text.Contains("Packet"))
            {
                await instance.WaitUntil(() => instance.world.IsActionAvailable(LockActions.Transfer));
                if (!instance.IsRunning || !instance.player.IsAlive || !instance.player.IsLoggedIn)
                {
                    return;
                }
                string username = instance.player.Username;
                await instance.proxy.SendToServer($"%xt%zm%cmd%1%tfer%{username}%{cmdMap}-100000");
                await instance.WaitUntil(() => !world.IsMapLoading, null, 40);
                await Task.Delay(1000);
            }
            if (!cmdMap.Equals(instance.player.Map, StringComparison.OrdinalIgnoreCase))
            {
                if (!text.Contains("Glitch"))
                {
                    await TryJoin(instance, cmdMap, text);
                }
                else
                {
                    int Max = 9999;
                    int Min = 9990;
                    if (text.Contains(":"))
                    {
                        Max = Convert.ToInt16(text.Split(':')[1]);
                        Min = Convert.ToInt16(text.Split(':')[2]);
                    }
                    while (!cmdMap.Equals(instance.player.Map, StringComparison.OrdinalIgnoreCase) && Max >= Min)
                    {
                        if (!instance.IsRunning || !instance.player.IsAlive || !instance.player.IsLoggedIn)
                        {
                            return;
                        }
                        await TryJoin(instance, cmdMap, "-" + Max);
                        Max--;
                    }
                    if (!cmdMap.Equals(instance.player.Map, StringComparison.OrdinalIgnoreCase) || (cmdMap.Equals(instance.player.Map, StringComparison.OrdinalIgnoreCase) && world.PlayersInMap.Count < 2))
                    {
                        await TryJoin(instance, cmdMap);
                    }
                }
            }
            if (cmdMap.Equals(instance.player.Map, StringComparison.OrdinalIgnoreCase))
            {
                if (!instance.player.Cell.Equals(Cell, StringComparison.OrdinalIgnoreCase) && !text.Contains("Packet"))
                {
                    instance.player.MoveToCell(Cell, Pad);
                    await Task.Delay(500);
                }
                instance.world.SetSpawnPoint();
                botData.BotMap = cmdMap;
                botData.BotCell = Cell;
                botData.BotPad = Pad;
            }
        }

        public async Task TryJoin(IBotEngine instance, string MapName, string RoomProp = "")
        {
            World world = instance.world;
            Player player = instance.player;
            await instance.WaitUntil(() => world.IsActionAvailable(LockActions.Transfer));
            if (instance.player.CurrentState == Player.State.InCombat)
            {
                player.MoveToCell(instance.player.Cell, player.Pad);
                await Task.Delay(1250);
            }
            RoomProp = new Regex("-{1,}", RegexOptions.IgnoreCase).Replace(RoomProp, (Match m) => "-");
            RoomProp = new Regex("(1e)[0-9]{1,}", RegexOptions.IgnoreCase).Replace(RoomProp, (Match m) => "100000");
            player.JoinMap(MapName + RoomProp, Cell, Pad);
            await instance.WaitUntil(() => player.Map.Equals(MapName, StringComparison.OrdinalIgnoreCase), null, 5);
            await instance.WaitUntil(() => !world.IsMapLoading, null, 40);
        }

        public override string ToString()
        {
            return "Join: " + Map + ", " + Cell + ", " + Pad;
        }
    }
}