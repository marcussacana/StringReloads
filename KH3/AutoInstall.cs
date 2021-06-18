using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System.Diagnostics;

namespace KH3
{
    class AutoInstall : IAutoInstall
    {
        SRL Engine;
        public AutoInstall(SRL Engine) {
            this.Engine = Engine;
        }
        public string Name => "KH3";

        DisplayTextHook TextHook;
        DisplayDialogHook DialogHook;

        public void Install()
        {
            if (TextHook == null) {
                TextHook = new DisplayTextHook(Engine);
                DialogHook = new DisplayDialogHook(Engine);
            }

            TextHook.Install();
            DialogHook.Install();
        }

        public bool IsCompatible()
        {
            return Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Trim() == "kingdom hearts iii";
        }

        public void Uninstall()
        {
            TextHook.Uninstall();
            DialogHook.Uninstall();
        }
    }
}
