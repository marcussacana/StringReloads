using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;
using System.IO;

namespace StringReloads.Mods
{
    class PatchRedir : IMod
    {
        Config Settings => EntryPoint.SRL.Settings;
        public string Name => "PatchRedir";

        public void Install()
        {
            EntryPoint.SRL.EnableHook(new Hook.CreateFileA());
            EntryPoint.SRL.EnableHook(new Hook.CreateFileW());

            Hook.CreateFileA.OnCreateFile += Redirect;
            Hook.CreateFileW.OnCreateFile += Redirect;
        }

        private string Redirect(string FilePath) {
            string FileName = Path.GetFileName(FilePath);
            string PatchFile = Path.Combine(Environment.CurrentDirectory, "Patch", FileName);
            string WorkspaceFile = Path.Combine(Settings.WorkingDirectory, FileName);
            string OriFile = FilePath + ".ori";

            if (File.Exists(PatchFile))
                return PatchFile;

            if (WorkspaceFile != FilePath && File.Exists(WorkspaceFile))
                return WorkspaceFile;

            if (File.Exists(OriFile))
                return OriFile;
            
            return FilePath;
        }

        public void Uninstall()
        {
            Hook.CreateFileA.OnCreateFile -= Redirect;
            Hook.CreateFileW.OnCreateFile -= Redirect;
        }

        public bool IsCompatible()
        {
            return true;
        }
    }
}
