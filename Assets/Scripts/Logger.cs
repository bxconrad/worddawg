using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Logger {
    public static void Info(string message,
        [CallerMemberName] string method = "",
        [CallerFilePath] string file = "") {
        Print("INFO", message, method, file);
    }

    public static void Warn(string message,
        [CallerMemberName] string method = "",
        [CallerFilePath] string file = "") {
        Print("WARN", message, method, file);
    }

    public static void Error(string message,
        [CallerMemberName] string method = "",
        [CallerFilePath] string file = "") {
        Print("ERROR", message, method, file);
    }

    private static void Print(string level, string message, string method, string file) {
        var className = Path.GetFileNameWithoutExtension(file);
        Debug.Log($"[{level}] [{className}::{method}] {message}");
    }
}