using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Linq;
using System.Reflection;

class DotNetVM
{
    public enum Language
    {
        CSharp, VisualBasic
    }

    internal DotNetVM(byte[] Data)
    {
        Assembly = Assembly.Load(Data);
    }
    internal DotNetVM(string Content, Language Lang = Language.CSharp, string FileName = null, bool Debug = false)
    {
        if (System.IO.File.Exists(Content))
        {
            DllInitialize(Content);
            return;
        }

        System.IO.StringReader Sr = new System.IO.StringReader(Content);
        string[] Lines = new string[0];
        while (Sr.Peek() != -1)
        {
            string[] tmp = new string[Lines.Length + 1];
            Lines.CopyTo(tmp, 0);
            tmp[Lines.Length] = Sr.ReadLine();
            Lines = tmp;
        }
        Assembly = InitializeEngine(Lines, Lang, FileName, Debug);
    }

    private void DllInitialize(string Dll)
    {
        Assembly = Assembly.LoadFrom(Dll);
    }
    public Assembly Assembly { get; private set; }


    string DLL;
    public string AssemblyPath {
        get {
            return DLL;
        }
    }

    public string AssemblyDebugSymbols {
        get {
            return System.IO.Path.GetDirectoryName(DLL) + "\\" + System.IO.Path.GetFileNameWithoutExtension(DLL) + ".pdb";
        }
    }

    internal static void Crash()
    {
        Crash();
    }

    /// <summary>
    /// Call a Function with arguments and return the result
    /// </summary>
    /// <param name="ClassName">Class o the target function</param>
    /// <param name="FunctionName">Target function name</param>
    /// <param name="Arguments">Function parameters</param>
    /// <returns></returns>
    internal dynamic Call(string ClassName, string FunctionName, params object[] Arguments)
    {
        return exec(Arguments, ClassName, FunctionName, Assembly);
    }
    internal void StartInstance(string Class, params object[] Arguments)
    {
        Type fooType = Assembly.GetType(Class);
        Instance = Activator.CreateInstance(fooType, Arguments);
        LastClass = Class;
    }

    private string LastClass;
    private object Instance = null;
    private object exec(object[] Args, string Class, string Function, Assembly assembly)
    {
        if (LastClass != Class)
            Instance = null;
        LastClass = Class;

        Type fooType = assembly.GetType(Class);

        MethodInfo[] Methods = fooType.GetMethods().Where(x => x.Name == Function).Select(x => x).ToArray();

        foreach (MethodInfo Method in Methods)
        {
            if (Method.GetParameters().Length == Args.Length)
            {
                try
                {
                    if (Instance == null && !Method.IsStatic)
                        Instance = assembly.CreateInstance(Class);

                    return Method?.Invoke(Instance, BindingFlags.InvokeMethod, null, Args, CultureInfo.CurrentCulture);
                }
                catch (Exception ex)
                {
                    if (Method == Methods.Last())
                        throw ex;
                }
            }
        }

        throw new Exception("Failed to find the method...");
    }

    const string ImportFlag = "#import ";
    private Assembly InitializeEngine(string[] lines, Language Lang, string FileName = null, bool Debug = false)
    {
        CodeDomProvider cpd = (Lang == Language.CSharp ? new CSharpCodeProvider() : (CodeDomProvider)new VBCodeProvider());

        var cp = new CompilerParameters();
        string sourceCode = string.Empty;
        int Imports = 0;
        foreach (string line in lines)
        {
            if (line.ToLower().StartsWith(ImportFlag))
            {
                string ReferenceName = line.Substring(ImportFlag.Length, line.Length - ImportFlag.Length).Trim();
                if (ReferenceName.Contains("//"))
                    ReferenceName = ReferenceName.Substring(0, ReferenceName.IndexOf("//")).Trim();
                ReferenceName = ReferenceName.Replace("%CD%", AppDomain.CurrentDomain.BaseDirectory);
                cp.ReferencedAssemblies.Add(ReferenceName);
                Imports++;
                continue;
            }
            sourceCode += line + "\r\n";
        }
        cp.GenerateExecutable = false;

        if (Debug)
        {
            cp.IncludeDebugInformation = true;
            cp.TempFiles = new TempFileCollection(Environment.GetEnvironmentVariable("TEMP"), true);
            cp.TempFiles.KeepFiles = true;
        }

        if (FileName != null)
            cp.OutputAssembly = FileName;

        cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
        CompilerResults cr = cpd.CompileAssemblyFromSource(cp, sourceCode);

        if (cr.Errors.HasErrors)
        {
            string Log = "Interpreter Error(s) List:";
            foreach (CompilerError Error in cr.Errors)
            {
                string MSG = string.Format("[{0}]: {1}", Error.Line - Imports, Error.ErrorText);
                Log += "\n" + MSG;
            }

            throw new Exception(Log);
        }

        DLL = cr.PathToAssembly;
        return cr.CompiledAssembly;
    }

    public static string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path).TrimEnd('\\', '/');
        }
    }
}