using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.UI;

namespace Grimoire.Networking.Handlers
{
    public class HandlerChat : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[4]
        {
            "chatm",
            "warning",
            "whisper",
            "message"
        };

        private Proxy proxy;
        private World world;
        private Player player;
        private OptionsManager optionsManager;
        private LogForm logForm;
        public HandlerChat(Proxy newProxy, World newWorld, Player newPlayer, OptionsManager newOptions, LogForm newLogForm)
        {
            optionsManager = newOptions;
            logForm = newLogForm;
            proxy = newProxy;
            player = newPlayer;
            world = newWorld;
        }
        Dictionary<string, string> tercesstravels = new Dictionary<string, string>
        {
                { ".oblivion",  "Enter,     Spawn"  },
                { ".twins",     "Twins,     Left"   },
                { ".swindle",   "Swindle,   Left"   },
                { ".nulgath",   "Boss2,     Right"  },
                { ".carnage",   "m4,        Top"    },
                { ".lae",       "m5,        Top"    },
                { ".polish",    "m12,       Top"    }
        };
        
        Dictionary<string, string> chatcommands = new Dictionary<string, string>
        {
                { ".server", "Server, {arg5}"},
                { ".client", "Client, {arg5}"}
        };

        public void Handle(XtMessage message)
		{
            if (message.Arguments[2] == "zm" && message.Arguments[5].StartsWith("."))
            {
                string roomnum = "-";
                string text = message.Arguments[5];
                string[] texts;
                try
                {
                    texts = message.Arguments[5].Split(' ');
                }
                catch
                {
                    texts = new string[0];
                }
                if (chatcommands.ContainsKey(text) || chatcommands.ContainsKey(texts[0]))
                {
                    string[] cmdargs = chatcommands[text].Split(',');
                    string tosend = cmdargs[1];
                    tosend = new Regex("{arg(.)}", RegexOptions.IgnoreCase).Replace(tosend, (Match m) => texts[1].Replace("#037:", "%"));
                    if (cmdargs[0] == "Client")
                    {
                        proxy.SendToClient(tosend);
                    }
                    else
                    {
                        proxy.SendToServer(tosend);
                    }
                    MessageBox.Show(tosend);
                }
                else if (tercesstravels.ContainsKey(text))
                {
                    try
                    {
                        roomnum += message.Arguments[5].Split('-')[1];
                        roomnum = roomnum.Split('-')[1].Contains("e") ? "-100000" : roomnum;
                        text = message.Arguments[5].Split('-')[0];
                    }
                    catch
                    {
                        roomnum += "1";
                    }
                    if (player.Map.ToLower() != "citadel")
                    {
                        player.ExecuteTravel(new List<IBotCommand>{
                            player.CreateJoinCommand(
                                "citadel-100000", 
                                "m22", "Left"),
                        });
                    }
                    player.ExecuteTravel(new List<IBotCommand>{
                        player.CreateJoinCommand(
                            map: "tercessuinotlim" + roomnum, 
                            cell: tercesstravels[text].Split(',')[0], 
                            pad: tercesstravels[text].Split(',')[1])
                    });
                }
                message.Send = false;
            }
            string type = message.Arguments[2];
            string tolog = message.Arguments[4];
            message.Arguments[5] = (message.Arguments[5] == player.Username) && optionsManager.ChangeChat ? "You" : message.Arguments[5];
            switch (type)
            {
                case "chatm":
                    tolog = (message.Arguments[5] + message.Arguments[4]).Replace("zone~", ": ");
                    break;

                case "whisper":
                    tolog = message.Arguments[6] == player.Username ? "From " + message.Arguments[5] : "To " + message.Arguments[6];
                    tolog = $"{tolog}: {message.Arguments[4]}";
                    break;
            }
            logForm.AppendChat(string.Format("[{0:hh:mm:ss}] {1} \r\n", DateTime.Now, tolog));
        }
	}
}
