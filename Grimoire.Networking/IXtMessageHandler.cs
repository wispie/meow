using Grimoire.Botting;
using Grimoire.Tools;

namespace Grimoire.Networking
{
    public interface IXtMessageHandler
    {

        string[] HandledCommands
        {
            get;
        }

        void Handle(XtMessage message);
    }
}