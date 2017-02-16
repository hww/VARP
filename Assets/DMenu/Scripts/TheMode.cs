using System.Collections.Generic;
using VARP;
using UnityEngine.UI;

public interface ITheMode
{
    
}
public class TheMode
{
    public static TheMode Null = new TheMode("null", "Empty unused mode", KeyMap.GlobalKeymap);
    /// <summary>
    /// This is the curent mode key map
    /// </summary>
    public readonly KeyMap keyMap;

    /// <summary>
    /// The mode name
    /// </summary>
    public readonly string name;

    /// <summary>
    /// Mode help
    /// </summary>
    public readonly string help;

    /// <summary>
    /// When this mode recogni
    /// </summary>
    private TheMode pareMode;

    public TheMode(string name, string help = null, KeyMap keyMap = null)
    {
        this.name = name;
        this.help = help;
        this.keyMap = keyMap;
    }

    public TheMode(TheMode parentMode, string name, string help = null, KeyMap keyMap = null)
    {
        pareMode = parentMode;
        this.name = name;
        this.help = help;
        this.keyMap = keyMap;
    }

}
