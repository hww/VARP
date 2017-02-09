using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DMenu
{
    public class Log
    {
        /// <summary>
        /// Format string with adding the full path to the object
        /// </summary>
        /// <param name="format"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string Format(string format, params object[] paramList)
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder();
            path.Append(string.Format("[INFO] [{0:yyyyMMdd-HHmmss}] ", System.DateTime.Now));
            path.Append(string.Format(format, paramList));
            return path.ToString();
        }

        /// <summary>
        /// Format string with adding the full path to the object
        /// </summary>
        /// <param name="format"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string ErrorFormat(string format, params object[] paramList)
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder();
            path.Append(string.Format("[ERRO] [{0:yyyyMMdd-HHmmss}] ", System.DateTime.Now));
            path.Append(string.Format(format, paramList));
            return path.ToString();
        }

        /// <summary>
        /// Format string with adding the full path to the object
        /// </summary>
        /// <param name="format"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string ExceptionFormat(string format, params object[] paramList)
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder();
            path.Append(string.Format("[EXCP] [{0:yyyyMMdd-HHmmss}] ", System.DateTime.Now));
            path.Append(string.Format(format, paramList));
            return path.ToString();
        }

        /// <summary>
        /// Format string with adding the full path to the object
        /// </summary>
        /// <param name="format"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static string WarningFormat(string format, params object[] paramList)
        {
            System.Text.StringBuilder path = new System.Text.StringBuilder();
            path.Append(string.Format("[WARN] [{0:yyyyMMdd-HHmmss}] ", System.DateTime.Now));
            path.Append(string.Format(format, paramList));
            return path.ToString();
        }
    }
}