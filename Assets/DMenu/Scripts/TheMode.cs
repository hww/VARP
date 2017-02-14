using System.Collections;
using System.Collections.Generic;
using DMenu;
using UnityEditor;
using UnityEngine;

public class TheMode
{
    /// <summary>
    /// This is the curent mode key map
    /// </summary>
    private KeyMap keyMap;

    /// <summary>
    /// The mode name
    /// </summary>
    private string name;

    /// <summary>
    /// Mode help
    /// </summary>
    private string help;

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
        this.pareMode = parentMode;
        this.name = name;
        this.help = help;
        this.keyMap = keyMap;
    }
}

public class InputBuffer
{
    private readonly string name;
    private TheMode majorMode;
    private List<TheMode> minorModes;

    public InputBuffer(string name, TheMode majorMode)
    {
        this.name = name;
    }

    public void EnabeMinorMode(TheMode mode)
    {
        if (minorModes.Contains(mode))
            return;
         minorModes.Add(mode);
    }

    public void DisableMinorMode(TheMode mode)
    {
        if (minorModes.Contains(mode))
            return;
        minorModes.Add(mode);
    }

    /// <summary>
    /// Call when buffer text was changed
    /// </summary>
    /// <param name="str"></param>
    /// <param name="cursorPosition"></param>
    /// <returns></returns>
    public virtual string OnChangeText(string str, int cursorPosition)
    {
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c">input character</param>
    /// <param name="str">input string</param>
    /// <param name="cursorPosition">cursorPosition</param>
    /// <returns></returns>
    public virtual char OnValidateText(char c, string str, int cursorPosition)
    {
        return c; 
    }


}