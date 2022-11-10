using UnityEngine;
using System;
using JJS.Log;
using System.Diagnostics;
public static class Debug
{
    public static bool isDebugBuild
    {
        get { return UnityEngine.Debug.isDebugBuild; }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityEngine.Debug.DrawRay(start, dir, color);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, [UnityEngine.Internal.DefaultValue("Color.white")] Color color, [UnityEngine.Internal.DefaultValue("0.0f")] float duration, [UnityEngine.Internal.DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityEngine.Debug.DrawRay(start, dir);
    }
    public static void Log(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public static void LogAssertion(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogAssertion(message, context);
    }

    public static void LogAssertion(object message)
    {
        UnityEngine.Debug.LogAssertion(message);
    }

    public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat(context, format, args);
    }

    public static void LogAssertionFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat( format, args);
    }

    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(context, format, args);
    }

    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    public static void LogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);
    }

    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(context, format, args);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

}