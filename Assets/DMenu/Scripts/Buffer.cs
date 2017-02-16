using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace VARP
{
    public interface IBuffer
    {
        void OnKeyDown(int evt);
        void OnEnable();
        void OnDisable();
        string Name { get; }
        string Help { get; }
        void EnabeMajorMode(TheMode mode);
        void DisableMajorMode();
        void EnabeMinorMode(TheMode mode);
        void DisableMinorMode(TheMode mode);
        KeyMapItem Lockup(int[] sequence);
    }

    public class Buffer : IBuffer
    {
        private static Buffer curentBuffer;
        private static readonly Buffer Null = new Buffer("null", "Empty unused buffer");

        private readonly string name;
        private readonly string help;
        private TheMode majorMode;
        private readonly List<TheMode> minorModes = new List<TheMode>();
        private readonly InputBuffer inputBuffer = new InputBuffer();

        public Buffer([NotNull] string name, string help = null)
        {
            if (name == null) throw new ArgumentNullException("name");
            this.name = name;
            this.help = help;
            this.majorMode = TheMode.Null;
        }

        #region IBuffer


        public void OnKeyDown(int evt)
        {
            inputBuffer.OnKeyDown(evt);
        }

        public void OnEnable()
        {

        }

        public void OnDisable()
        {

        }

        public string Name {
            get { return name; } 
        }

        public string Help
        {
            get { return help; } 
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

        public void EnabeMajorMode(TheMode mode)
        {
            majorMode = mode;
        }

        public void DisableMajorMode()
        {
            majorMode = TheMode.Null;
        }

        public KeyMapItem Lockup(int[] sequence)
        {
            foreach (var minorMode in minorModes)
            {
                var minorItem = minorMode.keyMap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, false);
                if (minorItem != null)
                    return minorItem;
            }

            var majorItem = majorMode.keyMap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, false);
            if (majorItem != null)
                return majorItem;

            return KeyMap.GlobalKeymap.LokupKey(inputBuffer.buffer, 0, inputBuffer.Count, false);
        }

        #endregion

        public static Buffer CurentBuffer
        {
            get { return curentBuffer; }
            set
            {
                curentBuffer.OnDisable();
                curentBuffer = value ?? Null;
                curentBuffer.OnEnable();
            }
        }

    }

}

public class InputBuffer
{

    public readonly int[] buffer = new int[32];

    public InputBuffer()
    {
        Count = 0;
    }

    public virtual void OnKeyDown(int evt)
    {
        if (Count >= buffer.Length)
            Clear();
        buffer[Count++] = evt;
    }

    public void Clear()
    {
        Count = 0;
    }

    public int Count { get; private set; }

    public override string ToString()
    {
        var s = "";
        for (var i = 0; i < Count; i++)
        {
            s += VARP.Event.GetName(buffer[i]);
        }
        return s;
    }
}

/*
 *

/// <summary>
/// This is mini text editor. The buffer have to have 
/// single major mode and multiple minor modes
/// </summary>
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
    /// When we type the character to the input field
    /// </summary>
    /// <param name="str">input string</param>
    /// <param name="cursorPosition">cursorPosition</param>
    /// <param name="c">input character</param>
    /// <returns></returns>
    public virtual char OnValidateText(string str, int cursorPosition, char c)
    {
        return c;
    }

    /// <summary>
    /// When we finish editing field by enter
    /// or by focusing other window
    /// </summary>
    /// <param name="input"></param>
    public virtual void OnEndEdit(InputField input)
    {
        
    }

    #region Input Stream

    private static readonly int[] eventBuffer = new int[10000];
    private static int inputPosition = 0;
    private static TheMode fundamentalMode;

    public virtual void OnKeyDown(int evt)
    {
        if (inputPosition< eventBuffer.Length)
            eventBuffer[inputPosition++] = evt;

        var s = "";
        for (var i = 0; i<inputPosition; i++)
        {
            s += VARP.Event.GetName(eventBuffer[i]);
        }
       // Debug.Log(s);
    }

    #endregion
}

    */
