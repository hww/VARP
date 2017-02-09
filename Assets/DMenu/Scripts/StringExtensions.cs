using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public static class StringExtensions
{
    public static string ParseOutClone(ref string str)
    {
        if (str.EndsWith(" (Clone)"))
            str = str.Replace(" (Clone)", string.Empty);
        else if (str.EndsWith("(Clone)"))
            str = str.Replace("(Clone)", string.Empty);

        return str;
    }

    public static byte[] StringToByteArray(this string me)
    {
        var e = new ASCIIEncoding();
        return e.GetBytes(me);
    }

    public static string ParseOutSpacesAndSymbols(this string me)
    {
        return Regex.Replace(me, "[^A-Za-z0-9]", string.Empty);
    }

    /// <summary>
    /// gets variable name as a string, good for saving data by name Example use ' StringExtensions.GetVariableName(() => myVar) '
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string GetVariableName<T>(Expression<Func<T>> expr)
    {
        var body = ((MemberExpression)expr.Body);
        return body.Member.Name;
    }

    /// <summary>
    /// Will return all ints from a string. EX "gg78gg" will return 78
    /// </summary>
    /// <param name="myInt"></param>
    /// <param name="phrase"></param>
    /// <returns></returns>
    public static int GetIntFromString(this string phrase)
    {
        var a = phrase;
        var b = string.Empty;

        for (var i = 0; i < a.Length; i++)
        {
            if (char.IsDigit(a[i]))
                b += a[i];
        }

        if (b.Length > 0)
            return int.Parse(b);
        else
            return 0;
    }

    /// <summary>
    /// Alphabetize the characters in the string.
    /// </summary>
    public static string Alphabetize(this string s)
    {
        // 1.
        // Convert to char array.
        var a = s.ToCharArray();

        // 2.
        // Sort letters.
        Array.Sort(a);

        // 3.
        // Return modified string.
        return new string(a);
    }


    public static string Humanize(this string str, bool capitalize = true)
    {
        var output = string.Empty;
        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            if (c == '-')
            {
                capitalize = true;
                output += ' ';
            }
            else if (c >= 'A' && c <= 'Z')
            {
                if (i > 0)
                    output += ' ';
                output += c;
                capitalize = false;
            }
            else
            {
                if (capitalize)
                {
                    output += char.ToUpper(c);
                    capitalize = false;
                }
                else
                {
                    output += c;
                }
            }
        }
        return output;
    }
    public static string[] Humanize(this string[] str, bool capitalize = true)
    {
        var res = new string[str.Length];
        for (var i = 0; i < str.Length; i++)
            res[i] = str[i].Humanize(capitalize);
        return res;
    }
    public static string Camelize(this string str)
    {
        return Camelize(str, true);
    }

    public static string Camelize(this string str, bool capitalize)
    {
        var output = string.Empty;
        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            if (c == '-')
            {
                capitalize = true;
            }
            else
            {
                if (capitalize)
                {
                    output += char.ToUpper(c);
                    capitalize = false;
                }
                else
                {
                    output += c;
                }
            }
        }
        return output;
    }

    public static string Decamelize(this string str)
    {
        var output = string.Empty;
        var small = false;
        var space = false;
        for (var i = 0; i < str.Length; i++)
        {
            var c = str[i];
            if (char.IsUpper(c))
            {
                if (small)
                    output += '-';
                output += char.ToLower(c);
                small = false;
                space = false;
            }
            else if (c == ' ')
            {
                small = true; // make - if next capital
                space = true; // make - if nex down
            }
            else
            {
                if (space)
                    output += '-';
                output += c;
                small = true; // make - if next capital
                space = false;// do not make - if next small
            }
        }
        return output;
    }

    public static string Pluralize(this string singularForm, int howMany)
    {
        return singularForm.Pluralize(howMany, singularForm + "s");
    }

    public static string Pluralize(this string singularForm, int howMany, string pluralForm)
    {
        return howMany == 1 ? singularForm : pluralForm;
    }


    /// <summary>
    /// Returns a list of strings no larger than the max length sent in.
    /// </summary>
    /// <remarks>useful function used to wrap string text for reporting.</remarks>
    /// <param name="text">Text to be wrapped into of List of Strings</param>
    /// <param name="maxLength">Max length you want each line to be.</param>
    /// <returns>List of Strings</returns>
    public static string Wrap(this string text, int maxLength, bool multiline = true)
    {

        // Return empty list of strings if the text was empty
        if (text.Length == 0)
            return string.Empty;

        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;

        foreach (var currentWord in words)
        {

            if ((currentLine.Length > maxLength) ||
                    ((currentLine.Length + currentWord.Length) > maxLength))
            {
                lines.Add(currentLine);
                currentLine = string.Empty;
            }

            if (currentLine.Length > 0)
                currentLine += " " + currentWord;
            else
                currentLine += currentWord;

        }

        if (currentLine.Length > 0)
            lines.Add(currentLine);

        if (multiline)
            return string.Join("\n", lines.ToArray());
        else
            return lines[0];
    }

    public static string WrapColor(string s, Color c)
    {
        return ColorTagOpen(c) + s + ColorTagClose();
    }

    public static string ColorTagOpen(Color c)
    {
        return "<color=#" + ColorToHex(c) + ">";
    }

    public static string ColorTagClose()
    {
        return "</color>";
    }

    public static string ColorToHex(Color32 color)
    {
        var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static string LineHr(int width, char character = '▬')
    {
        return new string(character, width);
    }

    public static int AlphanumCompare(this object x, object y)
    {
        var s1 = x as string;
        if (s1 == null)
        {
            return 0;
        }
        var s2 = y as string;
        if (s2 == null)
        {
            return 0;
        }

        var len1 = s1.Length;
        var len2 = s2.Length;
        var marker1 = 0;
        var marker2 = 0;

        // Walk through two the strings with two markers.
        while (marker1 < len1 && marker2 < len2)
        {
            var ch1 = s1[marker1];
            var ch2 = s2[marker2];

            // Some buffers we can build up characters in for each chunk.
            var space1 = new char[len1];
            var loc1 = 0;
            var space2 = new char[len2];
            var loc2 = 0;

            // Walk through all following characters that are digits or
            // characters in BOTH strings starting at the appropriate marker.
            // Collect char arrays.
            do
            {
                space1[loc1++] = ch1;
                marker1++;

                if (marker1 < len1)
                {
                    ch1 = s1[marker1];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

            do
            {
                space2[loc2++] = ch2;
                marker2++;

                if (marker2 < len2)
                {
                    ch2 = s2[marker2];
                }
                else
                {
                    break;
                }
            } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

            // If we have collected numbers, compare them numerically.
            // Otherwise, if we have strings, compare them alphabetically.
            var str1 = new string(space1);
            var str2 = new string(space2);

            int result;

            if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
            {
                var thisNumericChunk = int.Parse(str1);
                var thatNumericChunk = int.Parse(str2);
                result = thisNumericChunk.CompareTo(thatNumericChunk);
            }
            else
            {
                result = str1.CompareTo(str2);
            }

            if (result != 0)
            {
                return result;
            }
        }
        return len1 - len2;
    }
}
