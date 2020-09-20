namespace StringReloads.Hook
{
    public unsafe class CreateFileA : Base.Hook<CreateFileADelegate>
    {
        public static CreateFileA Instance;

        public override string Library => "kernel32.dll";

        public override string Export => "CreateFileA";

        public override void Initialize()
        {
            if (Instance != null)
                return;

            Instance = this;
            HookDelegate = new CreateFileADelegate(CreateFileHook);
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
