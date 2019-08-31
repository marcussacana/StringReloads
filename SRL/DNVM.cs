using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Linq;
using System.Reflection;

internal class DotNetVM
{
    public enum Language
    {
        CSharp, VisualBasic
    }
    internal DotNetVM(byte[] File)
    {
        Engine = Assembly.Load(File);
    }

    internal DotNetVM(string Code)
    {
        System.IO.StringReader Sr = new System.IO.StringReader(Code);
        string[] Lines = new string[0];
        while (Sr.Peek() != -1)
        {
            string[] tmp = new string[Lines.Length + 1];
            Lines.CopyTo(tmp, 0);
            tmp[Lines.Length] = Sr.ReadLine();
            Lines = tmp;
        }
        Engine = InitializeEngine(Lines, Language.CSharp);
    }

    internal DotNetVM(string Code, Language Lang)
    {
        System.IO.StringReader Sr = new System.IO.StringReader(Code);
        string[] Lines = new string[0];
        while (Sr.Peek() != -1)
        {
            string[] tmp = new string[Lines.Length + 1];
            Lines.CopyTo(tmp, 0);
            tmp[Lines.Length] = Sr.ReadLine();
            Lines = tmp;
        }
        Engine = InitializeEngine(Lines, Lang);
    }


    Assembly Engine;

    internal static void Crash() =>
        Crash();

    /// <summary>
    /// Call a Function with arguments and return the result
    /// </summary>
    /// <param name="ClassName">Class o the target function</param>
    /// <param name="FunctionName">Target function name</param>
    /// <param name="Arguments">Function parameters</param>
    /// <returns></returns>
    internal dynamic Call(string ClassName, string FunctionName, params object[] Arguments) =>
        Exec(Arguments, ClassName, FunctionName, Engine);


    private object Instance = null;

    internal void StartInstance(string Class, params object[] Arguments)
    {
        Type fooType = Engine.GetType(Class);
        Instance = Activator.CreateInstance(fooType, Arguments);
    }

    private object Exec(object[] Args, string Class, string Function, Assembly assembly)
    {
        Type fooType = assembly.GetType(Class);
        if (Instance == null)
            Instance = assembly.CreateInstance(Class);
        MethodInfo[] Methods = fooType.GetMethods().Where(x => x.Name == Function && x.GetParameters().Length == Args.Length).Select(x => x).ToArray();

        Exception Error = new Exception("Failed to search for the method.");
        foreach (MethodInfo Method in Methods)
        {
            try
            {
                return Method?.Invoke(Instance, BindingFlags.InvokeMethod, null, Args, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
        }

        throw Error.InnerException;
    }

    private Assembly InitializeEngine(string[] lines, Language Lang)
    {
        CodeDomProvider cpd = (Lang == Language.CSharp ? (CodeDomProvider)new CSharpCodeProvider() : new VBCodeProvider());
        var cp = new CompilerParameters();
        string sourceCode = string.Empty;
        foreach (string line in lines)
        {
            if (line.StartsWith("#IMPORT "))
            {
                string dll = line.Substring(8, line.Length - 8).Replace("%CD%", AssemblyDirectory);
                cp.ReferencedAssemblies.Add(dll);
                continue;
            }
            sourceCode += line.Replace("\t", "") + '\n';
        }
        cp.GenerateExecutable = false;
        CompilerResults cr = cpd.CompileAssemblyFromSource(cp, sourceCode);
        return cr.CompiledAssembly;

    }

    public static string AssemblyDirectory {
        get {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return System.IO.Path.GetDirectoryName(path);
        }
    }
}