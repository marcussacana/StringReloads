using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRL.Engine
{
    class Wrapper : IEngine
    {

        IEngine[] Engines => (from Asm in AppDomain.CurrentDomain.GetAssemblies()
                              from Typ in Asm.GetTypes()
                              where typeof(IEngine).IsAssignableFrom(Typ) && !Typ.IsInterface && Typ != typeof(Wrapper)
                              select (IEngine)Activator.CreateInstance(Typ)).OrderBy(x => x.Name).ToArray();

        IEngine Running = null;
        public string Name => Running?.Name ?? "Engine";

        public bool IsCompatible()
        {
            foreach (var Engine in Engines)
                if (Engine.IsCompatible())
                {
                    Running = Engine;
                    return true;
                }
            return false;
        }


        public void InstallStrHook() => Running?.InstallStrHook();
        public void UninstallStrHook() => Running?.UninstallStrHook();
    }


    interface IEngine
    {
        /// <summary>
        /// The Name of the Engine
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Verify if the current process is running this engine
        /// </summary>
        bool IsCompatible();

        /// <summary>
        /// Install the String Hook
        /// </summary>
        void InstallStrHook();

        /// <summary>
        /// Uninstall the String Hook
        /// </summary>
        void UninstallStrHook();

    }
}
