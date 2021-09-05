using System;
using System.ComponentModel;
using System.Threading;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Hook.Base;

namespace Overlay
{
    unsafe class Plugin : IPlugin
    {
        public string Name => "Overlay";

        public IAutoInstall[] GetAutoInstallers() => null;

        public IEncoding[] GetEncodings() => null;

        public Hook[] GetHooks() => null;

        public IMatch[] GetMatchs() => null;

        public IStringModifier[] GetModifiers() => null;

        public IMod[] GetMods() => null;

        public IReloader[] GetReloaders() => null;

        public void Initialize(SRL Engine)
        {
            Engine.OnFlagTriggered += OnFlagTriggered;
        }

        private void OnFlagTriggered(LSTFlag Entry, CancelEventArgs Args)
        {
            if (Entry.Name.ToUpperInvariant() != "OVERLAY")
                return;

            Args.Cancel = true;
            Exports.TriggerEvent(Entry.Value);
        }
    }
}
