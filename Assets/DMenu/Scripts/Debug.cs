using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VARP
{
    public sealed class Debug
    {
        private static string Wrap(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Error:
                    return string.Format("[ERRO] [{0:yyyyMMdd-HHmmss}] {1}", DateTime.Now, message);

                case LogType.Assert:
                    return string.Format("[ASSR] [{0:yyyyMMdd-HHmmss}] {1}", DateTime.Now, message);

                case LogType.Warning:
                    return string.Format("[WARN] [{0:yyyyMMdd-HHmmss}] {1}", DateTime.Now, message);
 
                case LogType.Log:
                    return string.Format("[LOG] [{0:yyyyMMdd-HHmmss}] {1}", DateTime.Now, message);

                case LogType.Exception:
                    return string.Format("[EXP] [{0:yyyyMMdd-HHmmss}] {1}", DateTime.Now, message);

                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private static string WrapFormat(LogType type, string format, params object[] args)
        {
            return Wrap(type, string.Format(format, args));
        }

        private static object Wrap(LogType type, object message)
        {
            return Wrap(type, message.ToString());
        }



        public static void Log(object message)
        {
            UnityEngine.Debug.logger.Log(LogType.Log, Wrap(LogType.Log, message));
        }

        public static void Log(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.logger.Log(LogType.Log, Wrap(LogType.Log, message), context);
        }

        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Log, WrapFormat(LogType.Log, format, args));
        }

        public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Log, context, WrapFormat(LogType.Log, format, args));
        }

        public static void LogError(object message)
        {
            UnityEngine.Debug.logger.Log(LogType.Error, Wrap(LogType.Error, message));
        }

        public static void LogError(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.logger.Log(LogType.Error, Wrap(LogType.Error, message), context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Error, WrapFormat(LogType.Error, format, args));
        }

        public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Error, context, WrapFormat(LogType.Error, format, args));
        }

        public static void LogException(Exception exception)
        {
            UnityEngine.Debug.logger.LogException(exception, null);
        }

        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            UnityEngine.Debug.logger.LogException(exception, context);
        }


        public static void LogWarning(object message)
        {
            UnityEngine.Debug.logger.Log(LogType.Warning, Wrap(LogType.Warning, message));
        }

        public static void LogWarning(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.logger.Log(LogType.Warning, Wrap(LogType.Warning, message), context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Warning, WrapFormat(LogType.Warning, format, args));
        }

        public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Warning, context, WrapFormat(LogType.Warning, format, args));
        }


        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, "Assertion failed"));
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, UnityEngine.Object context)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, "Assertion failed" as object), context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message));
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message));
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message, UnityEngine.Object context)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message), context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message, UnityEngine.Object context)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message) as object, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.LogFormat(LogType.Assert, WrapFormat(LogType.Assert, format, args));
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
        {
            if (!condition)
            {
                UnityEngine.Debug.logger.LogFormat(LogType.Assert, context, WrapFormat(LogType.Assert, format, args));
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message)
        {
            UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.logger.Log(LogType.Assert, Wrap(LogType.Assert, message), context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Assert, WrapFormat(LogType.Assert, format, args));
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.logger.LogFormat(LogType.Assert, context, WrapFormat(LogType.Assert, format, args));
        }

    }
}