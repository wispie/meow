using AxShockwaveFlashObjects;
using Grimoire.Botting.Commands.Combat;
using Grimoire.Botting.Commands.Item;
using Grimoire.Botting.Commands.Misc;
using Grimoire.Botting.Commands.Misc.Statements;
using Grimoire.Botting.Commands.Quest;
using Grimoire.Game;
using Grimoire.Game.Data;
using Grimoire.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Grimoire.Botting
{
    public class BotUtilities
    {
        private Flash flash;
        private Player player;
        private AxShockwaveFlash aFlash;
        public BotUtilities(Flash newFlash, Player newPlayer, AxShockwaveFlash newAFlash)
        {
            flash = newFlash;
            player = newPlayer;
            aFlash = newAFlash;
            BankLoadEvent = new ManualResetEvent(initialState: false);
        }
        public ManualResetEvent BankLoadEvent;
        
        /**
        public string CallGameFunction(string path, params object[] args)
        {
            return args.Length > 0 ? flash.FlashUtil.Call("callGameFunction", new object[] { path }.Concat(args).ToArray()) : flash.FlashUtil.Call("callGameFunction0", path);
        }

        public void SetGameObject(string path, object value)
        {
            flash.FlashUtil.Call("setGameObject", path, value);
        }

        public string GetGameObject(string path)
        {
            return flash.FlashUtil.Call("getGameObject", path);
        }
        **/

        public string Call(string function, bool base64 = false)
        {
            try
            {
                string request = $"<invoke name=\"{function}\"returntype=\"xml\"></invoke>";
                string text = XElement.Parse(aFlash.CallFunction(request)).FirstNode?.ToString();
                if (text == null)
                {
                    return string.Empty;
                }
                if (!base64)
                {
                    return text.Correct().SanitizeXml();
                }
                return text.FromBase64().Correct().SanitizeXml();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}