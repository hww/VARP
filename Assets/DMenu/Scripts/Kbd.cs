using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace DMenu
{

    /// <summary>
    /// This class alwo to convert string expression to the 
    /// keycode sequence. The sequence is the aray of integers.
    /// Each character is the 32 bits word.
    /// </summary>
    public static class Kbd
    {
        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertToString(int[] sequence, string separator = null)
        {
            Debug.Assert(sequence != null);
            return ConvertToString(sequence, 0, sequence.Length, separator);
        }

        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="starts"></param>
        /// <param name="quantity"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertToString(int[] sequence, int starts, int quantity, string separator = null)
        {
            Debug.Assert(sequence != null);
            Debug.Assert(starts < sequence.Length);
            Debug.Assert(starts + quantity - 1 < sequence.Length);

            if (sequence.Length == 0)
                return string.Empty;

            var idx = starts;
            var cnt = 0;

            var sb = new StringBuilder();
            if (separator != null)
            {
                var addSeparator = false;
                while (cnt < quantity)
                {
                    if (addSeparator) sb.Append(separator);
                    sb.Append(Event.GetName(sequence[idx++]));
                    addSeparator = true;
                    cnt++;
                }
            }
            else
            {
                while (cnt < quantity)
                {
                    sb.Append(Event.GetName(sequence[idx++]));
                    cnt++;
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Convert the keycode sequence to the string
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static string[] ConvertToStringList(int[] sequence)
        {
            Debug.Assert(sequence != null);

            var result = new string[sequence.Length];
            if (sequence.Length == 0)
                return result;
            var i = 0;
            foreach (var code in sequence)
                result[i++] = Event.GetName(code);
            return result;
        }

        /// <summary>
        /// Supports multiple or single tokens
        /// When multiple tokens each one separated with ' ' space
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static int[] Parse([NotNull] string expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (expression == string.Empty) throw new ArgumentException("expression");

            var sequence = new List<int>();
            var tags = expression.Split(' ');

            foreach (var s in tags)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                int evt = Event.ParseExpression(s);

                if (evt >= 0)
                {
                    sequence.Add(evt);
                }
                else
                {
                    // This case will be translated as string
                    // "abcd" => "abcd"
                    foreach (var c in s)
                    {
                        sequence.Add(c);
                    }
                }
            }
            return sequence.ToArray();
        }



        public static int[] ParsePseudo(string[] sequence)
        {
            var idx = 0;
            var result = new int[sequence.Length];
            foreach (var s in sequence)
                result[idx++] = Event.GetPseudocodeName(s);
            return result;
        }
    }
}