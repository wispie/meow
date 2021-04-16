using Grimoire.Botting;
using Grimoire.Game;
using Grimoire.UI;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace Grimoire.Networking.Handlers
{
    public class HandlerWarningsXt : IXtMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "logoutWarning"
        };

        private LogForm logForm;
        public HandlerWarningsXt(LogForm newLogForm)
        {
            logForm = newLogForm;
        }

        public void Handle(XtMessage message)
        {
            logForm.AppendChat(string.Format("[{0:hh:mm:ss}] {1} \r\n", DateTime.Now, message.Arguments[4]));
            //message.Send = false;
        }
    }

    public class HandlerWarningsXml : IXmlMessageHandler
    {
        public string[] HandledCommands
        {
            get;
        } = new string[1]
        {
            "logout"
        };

        public void Handle(XmlMessage message)
        {
            //message.Send = false;
        }
    }
}