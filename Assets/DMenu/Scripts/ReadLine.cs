using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace VARP
{
    public partial class ReadLine : Mode
    {
        public static readonly ReadLine Instance = new ReadLine();

        public delegate object OnReadLineDelegate(string line);

        private static OnReadLineDelegate onReadLineListener;

        public event OnReadLineDelegate OnReadLineListener
        {
            add { onReadLineListener = value; }
            remove { onReadLineListener = null; }
        }


        public delegate string[] OnAutoCompletionDelegate(string line, int caretPosition);

        private OnAutoCompletionDelegate autoCompletionListener;

        public event OnAutoCompletionDelegate AutoCompletionListener
        {
            add { autoCompletionListener = value; }
            remove { autoCompletionListener = null; }
        }


        #region Constructors

        private ReadLine() : base("readline")
        {
            Initialize();
        }

        private ReadLine(string name, string help = null, KeyMap keyMap = null) : base(name, help, keyMap)
        {
            Initialize();
        }

        private ReadLine(Mode parentMode, string name, string help = null, KeyMap keyMap = null)
            : base(parentMode, name, help, keyMap)
        {
            Initialize();
        }

        #endregion

        #region Mode

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        #endregion




        public void Read(string promp, OnReadLineDelegate onReadLine)
        {
            OnReadLineListener += onReadLine;
            VARP.Console.ReadLine(promp, delegate(string text)
            {
                Console.WriteLine(text);
                AddHistory(text);
                if (onReadLineListener != null)
                    onReadLineListener(text);
            });
        }

        #region Navigating trought history

        private static readonly List<string> History = new List<string>(100);
        private static int historyPosition;

        private static void HistoryPositionReset()
        {
            historyPosition = History.Count - 1;
        }

        private static void HistoryPositionNext()
        {
            if (historyPosition < History.Count - 1) historyPosition++;
        }

        private static void HistoryPositionPrevious()
        {
            if (historyPosition > 0) historyPosition--;
        }

        public static string GetCurentHistory()
        {
            return (historyPosition < History.Count && historyPosition >= 0) ? History[historyPosition] : null;
        }

        #endregion

        #region Add elements to history

        public static void AddHistory(string text)
        {
            HistoryPositionReset();
            // only different text will be added
            if (GetCurentHistory() == text) return;
            History.Add(text);
            HistoryPositionReset();
        }

        public static void AddHistory(string[] history)
        {
            foreach (var s in history)
                History.Add(s);
            HistoryPositionReset();
        }

        public static void AddHistory(List<string> history)
        {
            foreach (var s in history)
                History.Add(s);
            HistoryPositionReset();
        }

        public static List<string> GetHistory()
        {
            return History;
        }

        public static void ClearHistory()
        {
            History.Clear();
        }

        #endregion

    }

    public partial class ReadLine : Mode
    {
        #region KeyMap 

        private void Initialize()
        {
            keyMap = new KeyMap();
            keyMap.Define(Kbd.Parse("Tab"), new NativeFunction("AutoComplete", AutoComplete));
            keyMap.Define(Kbd.Parse("UpArrow"), new NativeFunction("HistoryUp", HistoryUp));
            keyMap.Define(Kbd.Parse("DownArrow"), new NativeFunction("HistoryUp", HistoryDown));
            keyMap.Define(Kbd.Parse("C-c"), new NativeFunction("ControlC", ControlC));
        }

        private object AutoComplete(object[] args)
        {
            if (autoCompletionListener == null)
                return null;

            string text;
            int caretPosition;
            if (Console.GetInputLine(out text, out caretPosition))
            {
                if (caretPosition == 0) return null;

                // only if the field is focused
                var variants = autoCompletionListener(text, caretPosition);
                if (variants.Length == 1)
                {
                    var prefix = variants[0];
                    var result = prefix + text.Substring(caretPosition);
                    Console.SetInputLine(result, prefix.Length, true);
                }
                else if (variants.Length > 1)
                {
                    var lines = TerminalTableBuilder.BuildTable(variants, Console.BufferWidth, 2);
                    foreach (var line in lines)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            return null;
        }

        private object HistoryUp(object[] args)
        {
            var s = GetCurentHistory();
            HistoryPositionPrevious();
            if (s == null) return null;
            Console.SetInputLine(s, s.Length, true);
            return null;
        }

        private object HistoryDown(object[] args)
        {
            var s = GetCurentHistory();
            HistoryPositionNext();
            if (s == null) return null;
            Console.SetInputLine(s, s.Length, true);
            return null;
        }

        private object ControlC(object[] args)
        {
            var text = "";
            var caretPosition = 0;
            if (Console.GetInputLine(out text, out caretPosition))
            {
                Console.WriteLine(text + "C-c");
                Console.SetInputLine("", 0, true);
            }
            return null;
        }

        #endregion
    }


    public class TerminalTableBuilder
    {
        private static int GetMaxLength(string[] words)
        {
            var maxSize = 0;
            foreach (var word in words)
                maxSize = word == null ? 0 : Math.Max(maxSize, word.Length);
            return maxSize;
        }

        public static string[] BuildTable(string[] words, int resultWidth, int colspan)
        {
            const int space = 2;
            var colwidth = GetMaxLength(words) + space;
            var colnumber = resultWidth / colwidth;
            var rowsnumber = words.Length / colnumber;
            if (rowsnumber * colnumber < words.Length) rowsnumber++;
            var result = new string[rowsnumber];

            var rowcount = 0;
            var colcount = 0;
            var sentence = string.Empty;
            foreach (var word in words)
            {
                sentence += word.PadRight(colwidth);
                if (++colcount >= colnumber)
                {
                    colcount = 0;
                    result[rowcount++] = sentence;
                    sentence = string.Empty;
                }
            }
            if (rowcount < result.Length)
                result[rowcount] = sentence;
            return result;
        }

    }

}