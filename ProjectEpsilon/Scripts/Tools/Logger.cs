using Godot;
using System;
using System.Diagnostics;

namespace KayEth.Tools
{
    public class Logger
    {
        public const string DEFAULT_CONFIG_FILE_PATH = "user://logger_config.cfg";

        public const string GENERAL_SECTION = "General";
        public const string LOG_LEVEL_KEY = "Level";

        public const string SEPARATOR = " # ";

        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        static Logger _instance = null;
        static LogLevel _logLevel = LogLevel.Error;
        static long _count = 0;

        static bool _initialized = false;

        static ConfigFile _configFile = null;

        public void Initialize(string path = DEFAULT_CONFIG_FILE_PATH)
        {
            _configFile = new ConfigFile();
            var err = _configFile.Load(path);
            if (err == Godot.Error.Ok)
            {
                _logLevel = (LogLevel)_configFile.GetValue(GENERAL_SECTION, LOG_LEVEL_KEY, LogLevel.Error);
            }
            else if (err == Godot.Error.FileNotFound)
            {
                _configFile = new ConfigFile();

                _configFile.SetValue(GENERAL_SECTION, LOG_LEVEL_KEY, _logLevel);

                _configFile.Save(DEFAULT_CONFIG_FILE_PATH);
            }
            else
            {
                LogToConsole(LogLevel.Error, "", "Logger", "Initialize", new object[] { "Failed to read ConfigFile of Logger. Using default settings. Error: " + err } );
                return;
            }

            LogToConsole(LogLevel.Info, "", "Logger", "Initialize", new object[] { "Logger initialized." } );

            _instance = this;
            _initialized = true;
        }

        public void SetLogLevel(LogLevel level) => _logLevel = level;

        static void Log(LogLevel level, string separator, object[] data)
        {
            if (!_initialized)
                return;
            
            if (level < _logLevel)
                return;

            StackFrame frame = new StackFrame(2);
            _instance.LogToConsole(level, separator, frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name, data);
        }

        void LogToConsole(LogLevel level, string separator, string caller, string method, object[] data)
        {
            string line = "";
            for (int i = 0; i < data.Length; i++)
            {
                line += (data[i] == null ? "Null" : data[i].ToString());
                if (i < data.Length - 1)
                    line += separator;
            }
            string prefix = "[" + GetUtcTimestamp() + "] [" + level.ToStr() + "] [" + _count++.ToString() + "] [" + caller + " | " + method + "] ";

            if (level == LogLevel.Debug)
                GD.Print(prefix + line);
            else if (level == LogLevel.Info)
                GD.Print(prefix + line);
            else if (level == LogLevel.Warning)
            {   
                // string stack = new System.Diagnostics.StackTrace(2, true).ToString();
                // stack = stack.Remove(0, 4);
                // stack = stack.Remove(0, stack.Find("at") + 2);
                // stack = stack.Remove(0, stack.Find("  at"));

                GD.Print(prefix + line + "\n" + GetStack());
            }
            else if (level == LogLevel.Error)
            {
                // string stack = new System.Diagnostics.StackTrace(2, true).ToString();
                // stack = stack.Remove(0, 4);
                // stack = stack.Remove(0, stack.Find("at") + 2);
                // stack = stack.Remove(0, stack.Find("  at"));

                GD.PrintErr(prefix + line + "\n" + GetStack());
            }
        }

        static string GetTimestamp()
        {
            return DateTime.Now.ToString("o");
        }

        static string GetUtcTimestamp()
        {
            return DateTime.UtcNow.ToString("o");
        }

        public static void Debug(params object[] data)
        {
            Log(LogLevel.Debug, "", data);
        }

        public static void DebugS(params object[] data)
        {
            Log(LogLevel.Debug, " ", data);
        }

        public static void DebugT(params object[] data)
        {
            Log(LogLevel.Debug, "\t", data);
        }

        public static void Info(params object[] data)
        {
            Log(LogLevel.Info, "", data);
        }

        public static void InfoS(params object[] data)
        {
            Log(LogLevel.Info, " ", data);
        }

        public static void InfoT(params object[] data)
        {
            Log(LogLevel.Info, "\t", data);
        }

        public static void Warning(params object[] data)
        {
            Log(LogLevel.Warning, "", data);
        }

        public static void WarningS(params object[] data)
        {
            Log(LogLevel.Warning, " ", data);
        }

        public static void WarningT(params object[] data)
        {
            Log(LogLevel.Warning, "\t", data);
        }

        public static void Error(params object[] data)
        {
            Log(LogLevel.Error, "", data);
        }

        public static void ErrorS(params object[] data)
        {
            Log(LogLevel.Error, " ", data);
        }

        public static void ErrorT(params object[] data)
        {
            Log(LogLevel.Error, "\t", data);
        }

        static string GetStack()
        {
            string stack = new System.Diagnostics.StackTrace(3, true).ToString();
            stack = stack.Remove(0, 4);
            stack = stack.Remove(0, stack.Find("at") + 2);
            stack = stack.Remove(0, stack.Find("  at"));
            return stack;
        }
    }

    static class LogLevelExtensions
    {
        public static string ToStr(this Logger.LogLevel logLevel)
        {
            if (logLevel == Logger.LogLevel.Debug)
                return "DEBUG";
            if (logLevel == Logger.LogLevel.Info)
                return "INFO ";
            if (logLevel == Logger.LogLevel.Warning)
                return "WARN ";
            if (logLevel == Logger.LogLevel.Error)
                return "ERROR";
            return "     ";
        }
    }
}
