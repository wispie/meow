using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.UI;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grimoire.Botting.Commands.Misc
{
    public class CmdLog : IBotCommand
    {
        public string Text
        {
            get;
            set;
        }

        public bool Debug
        {
            get;
            set;
        } = false;

        private bool Clear
        {
            get;
            set;
        } = false;

        public async Task Execute(IBotEngine instance)
        {
            Player player = instance.player;
            World world = instance.world;
            LogForm logForm = instance.logForm;
            string text = Text;
            text = text.Replace("{USERNAME}", player.Username);
            text = text.Replace("{MAP}", player.Map).Replace("{ROOM_ID}", world.RoomId.ToString());
            text = text.Replace("{GOLD}", player.Gold.ToString());
            text = text.Replace("{LEVEL}", player.Level.ToString());
            text = text.Replace("{CELL}", player.Cell).Replace("{PAD}", player.Pad);
            text = text.Replace("{HEALTH}", player.Health.ToString()).Replace("{MANA}", player.Mana.ToString());
            text = text.Replace("{TIME}", $"{DateTime.Now:HH:mm:ss}");
            text = text.Replace("{TIME: 12}", $"{DateTime.Now:hh:mm:ss tt}");
            text = text.Replace("{TIME: 24}", $"{DateTime.Now:HH:mm:ss}");

            text = new Regex(
                "{ITEM:\\s*(.*?)}", RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{(player.Inventory.Items.Find((InventoryItem x) => x.Name == m.Groups[1].Value) ?? new InventoryItem(world)).Quantity}");

            text = new Regex(
                "{ITEM MAX:\\s*(.*?)}",     RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{(player.Inventory.Items.Find((InventoryItem x) => x.Name == m.Groups[1].Value) ?? new InventoryItem(world)).MaxStack}");

            text = new Regex(
                "{REP XP:\\s*(.*?)}",       RegexOptions.IgnoreCase).Replace(text, (Match m) => 
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).Rep}/" +
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).RequiredRep}");

            text = new Regex(
                "{REP CURRENT:\\s*(.*?)}",  RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).Rep}");

            text = new Regex(
                "{REP REMAINING:\\s*(.*?)}",RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).RemainingRep}");

            text = new Regex(
                "{REP REQUIRED:\\s*(.*?)}", RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).RequiredRep}");

            text = new Regex(
                "{REP RANK:\\s*(.*?)}",     RegexOptions.IgnoreCase).Replace(text, (Match m) => 
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).Rank}");

            text = new Regex(
                "{REP TOTAL:\\s*(.*?)}",    RegexOptions.IgnoreCase).Replace(text, (Match m) => 
                $"{(player.Factions.Find((Faction x) => x.Name == m.Groups[1].Value) ?? new Faction()).TotalRep}");

            text = new Regex(
                "{INT VALUE:\\s*(.*?)}",    RegexOptions.IgnoreCase).Replace(text, (Match m) =>
                $"{instance.Configuration.Tempvalues[m.Groups[1].Value]}");

            text = text + "\r\n";
            if (Debug)
                if (Clear)
                    logForm.txtLogDebug.Clear();
                else
                    logForm.AppendDebug(text);
            else if (Clear)
                logForm.txtLogScript.Clear();
            else
                logForm.AppendScript(text);
        }

        public override string ToString()
        {
            string typetxt = Debug ? "Debug" : "Script";
            Clear = Text.Contains("{CLEAR}") ? true : false;
            return Clear ? $"Clear {typetxt}" : $"Log {typetxt}: {Text}";
        }
    }
}