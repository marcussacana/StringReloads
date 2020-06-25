namespace StringReloads.Hook
{
    public unsafe class CreateFileW : Base.Hook<CreateFileWDelegate>
    {
        public override string Library => "kernel32.dll";

        public override string Export => "CreateFileW";

#if x64
        public override bool ProtectRAX => false;
#endif

        public override void Initialize()
        {
            HookDelegate = new CreateFileWDelegate(CreateFileHook);
            Compile();
        }
        public static event CreateFile OnCreateFile;
        private void* CreateFileHook(string FileName, EFileAccess Access, EFileShare Share, void* SecurityAttributes, ECreationDisposition CreationDisposition, EFileAttributes FlagsAndAttributes, void* TemplateFile)
        {
            FileName = OnCreateFile?.Invoke(FileName);
            return Bypass(FileName, Access, Share, SecurityAttributes, CreationDisposition, FlagsAndAttributes, TemplateFile);
        }

        public delegate string CreateFile(string FileName);
    }
}
