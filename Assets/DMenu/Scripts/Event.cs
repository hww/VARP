using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DMenu
{

    public static partial class Event
    {

        public static readonly int Meta = 1 << 27;
        public static readonly int Control = 1 << 26;
        public static readonly int Shift = 1 << 25;
        public static readonly int Hyper = 1 << 24;
        public static readonly int Super = 1 << 23;
        public static readonly int Alt = 1 << 22;

        /// <summary>
        /// Use for masking the modifyer bits
        /// </summary>
        public static readonly int AllModifyers = Control | Shift | Alt | Hyper | Super | Meta;

        /// <summary>
        /// Used internaly for iteration over modyfier. It is replacement for System.Enum.Values(typeof(Modifyers)).
        /// </summary>
        public static readonly int[] AllModifyersList = new int[] {Control, Alt, Shift};

        /// <summary>
        /// Check if the given keycode is with the given modifyer mask
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="modifyers"></param>
        /// <returns></returns>
        public static bool IsModifyer(int evt, int modifyers)
        {
            return (evt & modifyers) == modifyers;
        }

        /// <summary>
        /// Check if code is valid
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static bool IsValid(int evt)
        {
            return evt>= 0 && evt < (2 << 28);
        }

        /// <summary>
        /// Create nev event from code and use new modifyers
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="modifyers"></param>
        public static int MakeEvent(int keyCode, int modifyers)
        {
            var code = keyCode & ~AllModifyers;
            if (code > 32 && code < 255)
            {
                // ASCII

                if ((modifyers & AllModifyers) == Control)
                    return code & 0x1F;
                else
                    return code | modifyers;
            }
            else
            {
                return code | modifyers;
            }
        }

        /// <summary>
        /// Get modifyers of this event
        /// </summary>
        /// <returns></returns>
        public static int GetModifyers(int evt)
        {
            return evt & AllModifyers;
        }

        /// <summary>
        /// Get code of this event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static int GetKeyCode(int evt)
        {
            return evt & ~AllModifyers;
        }



    }

    public static partial class Event
    {
        public static readonly int Pseudo = 1 << 28;    // start of pseudocodes
        private static int pseudoCodeIndex = 0;         // pseudocodes generator
        static Dictionary<string, int> nameToKeyCodeTable;
        static Dictionary<int, string> keyCodeToNameTable;

        public static Dictionary<string, int> NameToKeyCodeTable
        {
            get
            {
                if (nameToKeyCodeTable == null) Initialize();
                return nameToKeyCodeTable;
            }
        }

        public static Dictionary<int, string> KeyCodeToNameTable
        {
            get {
                if (keyCodeToNameTable == null) Initialize();
                return keyCodeToNameTable;
            }
        }

        /// <summary>
        /// Initialize the class
        /// </summary>
        public static void Initialize()
        {
            nameToKeyCodeTable = new Dictionary<string, int>();
            keyCodeToNameTable = new Dictionary<int, string>();

            foreach (var keyCode in Enum.GetValues(typeof(KeyCode)))
                SetName((int)keyCode, keyCode.ToString());

            for (var i = (int)'a'; i < (int)('z'); i++)
                SetName(i, ((char)i).ToString());

            SetName(Control, "C-");
            SetName(Alt, "A-");
            SetName(Shift, "S-");
            SetName(Meta, "M-");
            SetName(Super, "s-");
            SetName(Hyper, "H-");

            SetName((int)KeyCode.LeftControl, "\\C-");
            SetName((int)KeyCode.LeftAlt, "\\A-");
            SetName((int)KeyCode.LeftShift, "\\S-");
            SetName((int)KeyCode.LeftWindows, "\\W-");

            SetName((int)KeyCode.RightControl, "\\C-");
            SetName((int)KeyCode.RightAlt, "\\A-");
            SetName((int)KeyCode.RightShift, "\\S-");
            SetName((int)KeyCode.RightWindows, "\\W-");
        }


        public static int GetPseudocodeName(string name)
        {
            int code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            code = pseudoCodeIndex++ | Pseudo;
            SetName(code, name);
            return code;
        }

        /// <summary>
        /// Declarate new key code name
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="name"></param>
        public static void SetName(int keyCode, string name)
        {
            var modifyers = keyCode & AllModifyers;
            var keyCodeOnly = keyCode - modifyers;
            // The key code can be modifyer or the key
            // but it can't be bought
            Debug.Assert(modifyers == 0 || keyCodeOnly == 0, keyCode);
            NameToKeyCodeTable[name] = keyCode;
            KeyCodeToNameTable[keyCode] = name;
        }

        public static string GetName(int keyCode)
        {
            var keyModifyers = keyCode & AllModifyers;
            var keyCodeOnly = keyCode - keyModifyers;

            var modifyerName = string.Empty;
            if (keyModifyers != 0)
            {
                foreach (var m in AllModifyersList)
                {
                    if (IsModifyer(keyModifyers, m))
                    {
                        string name;
                        if (KeyCodeToNameTable.TryGetValue(m, out name))
                            modifyerName += name;
                        else
                            throw new Exception(string.Format("Unexpected modifyer in keycode '{0:X}'", keyCode));
                    }
                }
            }


            string keyCodeName;
            if (KeyCodeToNameTable.TryGetValue(keyCodeOnly, out keyCodeName))
                return modifyerName + keyCodeName;
            else if (keyCodeOnly < 32 && keyModifyers == 0)
                return string.Format("^{0}", (char)(keyCodeOnly + 0x40));
            else
                throw new Exception(string.Format("Unexpected keycode '{0:X}'", keyCode));
        }

        /// <summary>
        /// Get keycode by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static int GetKeyCodeInternal(string name)
        {
            int code;
            if (NameToKeyCodeTable.TryGetValue(name, out code))
                return code;
            throw new Exception(string.Format("Expected key code name, found '{0}'", name));
        }

        /// <summary>
        /// Parse the expression without spaces
        /// </summary>
        /// <param name="expression"></param>
        public static int ParseExpression([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (expression == string.Empty) throw new ArgumentException("expression");

            // There is the testing for the modifyer C- A- S-
            var m = expression[0];
            if (m == 'C' || m == 'A' || m == 'S')
            {
                var evt = ParseWordWithModifyers(expression);
                if (evt >= 0) return evt;
            }

            // There is test for named character Shift, LeftAlt, Space
            return GetKeyCodeInternal(expression);
        }


        /// <summary>
        /// Support only single token with '-' character inside
        /// Such as C- A- S- the function produce an exception
        /// it the expression is badly formate
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>0 means nothing</returns>
        private static int ParseWordWithModifyers([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            var modifyers = 0;
            var sufix = string.Empty;

            var index = 0;
            while (index < expression.Length)
            {
                var c1 = expression[index];                    // C,A,S,...
                var i2 = index + 1;

                if (i2 < expression.Length)
                {
                    var c2 = expression[i2];                   // Here is '-'

                    if (c2 == '-')
                    {
                        index += 2;                             // skip c1 and c2
                        switch (c1)
                        {
                            case 'C':
                                modifyers |= Control;
                                break;

                            case 'S':
                                modifyers |= Shift;
                                break;

                            case 'A':
                                modifyers |= Alt;
                                break;

                            case 'M':
                                modifyers |= Meta;
                                break;

                            case 's':
                                modifyers |= Super;
                                break;

                            case 'H':
                                modifyers |= Hyper;
                                break;

                            default:
                                sufix = expression.Substring(index);
                                goto DONE;
                        }
                    }
                    else
                    {
                        sufix = expression.Substring(index); // include c1
                        goto DONE;
                    }
                }
                else
                {
                    sufix = expression.Substring(index);
                    goto DONE;
                }
            }

            DONE:
            if (sufix != string.Empty)
            {
                var tmp = GetKeyCodeInternal(sufix);
                if (tmp >= 0) return tmp;
                throw new Exception(string.Format("Expected character after C-,A-,S- found '{0}' in expression '{0:X}'", sufix, expression));
            }
            else
            {
                return modifyers;
            }

        }
    }

}

  