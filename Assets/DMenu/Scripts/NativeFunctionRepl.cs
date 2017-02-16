using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VARP;

public class NativeFunctionRepl : MonoBehaviour
{
    private TheMode replMode;

    private void Start()
    {
        replMode = new TheMode("repl");
        NativeFunction.Define("help", FuncHelp);
        ReadLine.Read("Hello", OnEnterText);
    }


    public void OnEnterText(string text)
    {
        ReadLine.Read("Hello", OnEnterText);
    }

    public void OnEnterTab(string text)
    {

    }

    private object FuncHelp(params object[] args)
    {
        var sb = new StringBuilder();
        foreach (var nativeFunction in NativeFunction.AllFunctions)
        {
            sb.AppendLine(string.Format("{0:16} {1}", nativeFunction.Key, nativeFunction.Value.help));
        }
        return sb.ToString();
    }
}
