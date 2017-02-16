using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VARP
{
    public class Console
    {
        public static ITerminal terminal;

        public delegate void OnReadLineDelegate(string line);
        public delegate string OnAutoCompleteDelegate(string line, int caretPosition);
        public delegate void OnKeyDownDelegate(int c);

        private static OnReadLineDelegate onReadLine;
        private static OnAutoCompleteDelegate onAutoComplete;
        private static OnKeyDownDelegate onKeyDownDelegate;

        #region UiTerminal interface

        public static void OnKeyDown(int evt)
        {
            if (onKeyDownDelegate != null)
                onKeyDownDelegate(evt);
        }

        // Receive call back from terminal when we enter line of text
        public static void OnReadLine(string text)
        {
            var function = onReadLine; onReadLine = null;
            if (function != null)
                function(text);
        }

        // Receive call back from terminal when we hit the TAB button
        public static string OnAutocomplete(string text, int caretPosition)
        {
            if (onAutoComplete != null)
                return onAutoComplete(text, caretPosition);
            return text;
        }

        #endregion

        #region Console

        public void Beep()
        {
            if (terminal != null) terminal.Beep();
        }
        public void Clear()
        {
            if (terminal != null) terminal.Clear();
        }
        public static void ResetColor()
        {
            if (terminal != null) terminal.ResetColor();
        }
        public static void SetColor(Color color)
        {
            if (terminal != null) terminal.SetColor(color);
        }
        public static void Write(string message)
        {
            if (terminal != null) terminal.Write(message);
        }
        public static void WriteLine(string message)
        {
            if (terminal != null) terminal.WriteLine(message);
        }
        public static void WritePrompt(string message)
        {
            if (terminal != null) terminal.WritePrompt(message);
        }

        #endregion

        // Read single character
        public static int ReadKey()
        {
            return 0;
        }


        ///Read line
        public static void ReadLine(string prompt, OnReadLineDelegate onReadLine)
        {
            WritePrompt(prompt);
            Console.onReadLine = onReadLine;
        }

    }

}
