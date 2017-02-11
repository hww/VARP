using System;
using System.Text;

public class Dbg
{
    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string Log(string message)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[INFO] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(message);
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="format"></param>
    /// <param name="paramList"></param>
    /// <returns></returns>
    public static string LogFormat(string format, params object[] paramList)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[INFO] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(string.Format(format, paramList));
        return path.ToString();
    }


    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string LogError(string message)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[ERRO] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(message);
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="format"></param>
    /// <param name="paramList"></param>
    /// <returns></returns>
    public static string LogErrorFormat(string format, params object[] paramList)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[ERRO] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(string.Format(format, paramList));
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string LogException(string message)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[EXCP] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(message);
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="format"></param>
    /// <param name="paramList"></param>
    /// <returns></returns>
    public static string LogExceptionFormat(string format, params object[] paramList)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[EXCP] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(string.Format(format, paramList));
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string LogWarning(string message, params object[] paramList)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[WARN] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(message);
        return path.ToString();
    }

    /// <summary>
    /// Format string with adding the full path to the object
    /// </summary>
    /// <param name="format"></param>
    /// <param name="paramList"></param>
    /// <returns></returns>
    public static string LogWarningFormat(string format, params object[] paramList)
    {
        var path = new StringBuilder();
        path.Append(string.Format("[WARN] [{0:yyyyMMdd-HHmmss}] ", DateTime.Now));
        path.Append(string.Format(format, paramList));
        return path.ToString();
    }
}
