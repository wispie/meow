using System.Windows.Forms;

namespace Grimoire.Tools
{
    public class Hotkey
    {
        private KeyboardHook keyboardHook;
        public Hotkey(KeyboardHook newKeyboardHook)
        {
            keyboardHook = newKeyboardHook;
        }

        public string Text
        {
            get;
            set;
        }

        public Keys Key
        {
            get;
            set;
        }

        public int ActionIndex
        {
            get;
            set;
        }

        public void Install()
        {
            keyboardHook.TargetedKeys.Add(Key);
        }

        public void Uninstall()
        {
            keyboardHook.TargetedKeys.Remove(Key);
        }
    }
}