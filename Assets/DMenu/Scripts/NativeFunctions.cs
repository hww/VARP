using System;
using System.Collections.Generic;
using DMenu;
using JetBrains.Annotations;

/// <summary>
/// This class give the way to declarate calls from 
/// simple REPL script the list of native functions
/// </summary>
public class NativeFunction
{
    public delegate object Function(params object[] paramList);

    public readonly string name;
    public readonly string help;
    private readonly Function function;

    public NativeFunction(string name, Function function, string help = null)
    {
        this.name = name;
        this.function = function;
        this.help = help;
    }

    public object Call(params object[] paramList)
    {
        if (function == null)
            throw new Exception(string.Format("The function '{0}' does not have method binded", name));
        return function(paramList);
    }

    private static readonly Dictionary<string, NativeFunction> AllFunctions = new Dictionary<string,NativeFunction>();

    public static NativeFunction Define(string name, Function func, string help = null)
    {
        NativeFunction f;
        if (AllFunctions.TryGetValue(name, out f))
            throw new Exception(string.Format("The function '{0}' is already defined", name));
        return AllFunctions[name] = new NativeFunction(name, func, help);
    }

    public static NativeFunction Lockup([NotNull] string name)
    {
        if (name == null) throw new ArgumentNullException("name");
        NativeFunction f;
        if (AllFunctions.TryGetValue(name, out f))
            return f;
        throw new Exception(string.Format("The function '{0}' is not defined", name));
    }

    public static object Call(string name, params object[] paramList)
    {
        var f = Lockup(name);
        return f.Call(paramList);
    }


}
