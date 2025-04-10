using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Animals.Common
{
    public static class DebugLogger
    {
        public enum LogLevel { Off, Error, Warn, Info, Verbose }

        public static LogLevel CurrentLevel = LogLevel.Off;

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (level <= CurrentLevel)
            {
                string color = GetColorForLevel(level);
                string coloredMessage = $"<color={color}>{message}</color>";
                Debug.Log(coloredMessage);
            }
        }

        public static void LogError(string message) => Log(message, LogLevel.Error);
        public static void LogWarning(string message) => Log(message, LogLevel.Warn);
        public static void LogInfo(string message) => Log(message, LogLevel.Info);
        public static void LogVerbose(string message) => Log(message, LogLevel.Verbose);

        private static string GetColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Error: return "red";
                case LogLevel.Warn: return "orange";
                case LogLevel.Info: return "blue";
                case LogLevel.Verbose: return "white";
                default: return "white";
            }
        }
    }

}
