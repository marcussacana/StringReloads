using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Linq;
using System.Reflection;
class DotNetVM {
    public enum Language {
        CSharp, VisualBasic
    }
    internal DotNetVM(string Content, Language Lang = Language.CSharp) {
        if (System.IO.File.Exists(Content)) {
            DllInitialize(Content);
            return;
        }

        System.IO.StringReader Sr = new System.IO.StringReader(Content);
        string[] Lines = new string[0];
        while (Sr.Peek() != -1) {
            string[] tmp = new string[Lines.Length + 1];
            Lines.CopyTo(tmp, 0);
            tmp[Lines.Length] = Sr.ReadLine();
            Lines = tmp;
        }
        Engine = InitializeEngine(Lines, Lang);
    }

    private void DllInitialize(string Dll) {
        Engine = Assembly.LoadFrom(Dll);
    }
    Assembly Engine;
    internal static void Crash() {
        Crash();
    }

    /// <summary>
    /// Call a Function with arguments and return the result
    /// </summary>
    /// <param name="ClassName">Class o the target function</param>
    /// <param name="FunctionName">Target function name</param>
    /// <param name="Arguments">Function parameters</param>
    /// <returns></returns>
    internal dynamic Call(string ClassName, string FunctionName, params object[] Arguments) {       
        return exec(Arguments, ClassName, FunctionName, Engine);
    }

    private string LastClass;
    private object Instance = null;
    private object exec(object[] Args, string Class, string Function, Assembly assembly) {
        if (LastClass != Class)
            Instance = null;
        LastClass = Class;

        Type fooType = assembly.GetType(Class);

        MethodInfo[] Methods = fooType.GetMethods().Where(x => x.Name == Function).Select(x => x).ToArray();

        foreach (MethodInfo Method in Methods) {
            if (Method.GetParameters().Length == Args.Length) {
                try {
                    if (Instance == null && !Method.IsStatic)
                        Instance = assembly.CreateInstance(Class);

                    return Method?.Invoke(Instance, BindingFlags.InvokeMethod, null, Args, CultureInfo.CurrentCulture);
                } catch (Exception ex) {
                    if (Method == Methods.Last())
                        throw ex;
                }
            }
        }

        throw new Exception("Failed to find the method...");
    }

    const string ImportFlag = "#import ";
    private Assembly InitializeEngine(string[] lines, Language Lang) {
        CodeDomProvider cpd = (Lang == Language.CSharp ? new CSharpCodeProvider() : (CodeDomProvider)new VBCodeProvider());

        var cp = new CompilerParameters();
        string sourceCode = string.Empty;
        int Imports = 0;
        foreach (string line in lines) {
            if (line.ToLower().StartsWith(ImportFlag)) {
                cp.ReferencedAssemblies.Add(line.Substring(ImportFlag.Length, line.Length - ImportFlag.Length).Trim());
                Imports++;
                continue;
            }
            sourceCode += line + "\r\n";
        }
        cp.GenerateExecutable = false;
        CompilerResults cr = cpd.CompileAssemblyFromSource(cp, sourceCode);

        if (cr.Errors.HasErrors) {
            string Log = "Interpreter Error(s) List:";
            foreach (CompilerError Error in cr.Errors) {
                string MSG = string.Format("[{0}]: {1}", Error.Line - Imports, Error.ErrorText);
                Log += "\n" + MSG;
            }

            throw new Exception(Log);
        }

        return cr.CompiledAssembly;
    }

    /*internal string CreateExe(string Source, string MainClass) {
        CodeDomProvider cpd = new CSharpCodeProvider();
        var cp = new CompilerParameters();
        cp.GenerateExecutable = true;
        cp.GenerateInMemory = false;
        cp.MainClass = MainClass;
        cp.IncludeDebugInformation = true;
        cp.OutputAssembly = MainClass;
        string sourceCode = string.Empty;
        System.IO.StringReader Sr = new System.IO.StringReader(Source);
        string[] Lines = new string[0];
        while (Sr.Peek() != -1) {
            string[] tmp = new string[Lines.Length + 1];
            Lines.CopyTo(tmp, 0);
            tmp[Lines.Length] = Sr.ReadLine();
            Lines = tmp;
        }
        foreach (string line in Lines) {
            if (line.StartsWith("using ") && line.EndsWith(";"))
                cp.ReferencedAssemblies.Add(line.Substring(6, line.Length - 7) + ".dll");
            sourceCode += line.Replace("\t", "") + '\n';
        }
        CompilerResults cr = cpd.CompileAssemblyFromSource(cp, sourceCode);
        return cr.PathToAssembly;
    }*/
}