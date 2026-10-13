using Godot;
using Godot.Collections;
using System.Diagnostics;

public enum LogLevel
{
    Trace,
    Info,
    Warning,
    Error
}

public static class Logger
{
    private static LogLevel _logLevel = LogLevel.Trace;

    private static int _logNumber = 0;

    const string LOG_FILE_DIR = "user://GameLogs";
    const string LOG_FILE_PATH = LOG_FILE_DIR + "/log_file_";
    const string LOG_FILE_EXT = ".log";
    const int NUMBER_OF_LOG_FILES = 10;

    private static string _logFilePath = "";

    private static bool Initialized {
        get
        {
            if (_initialized)
                return true;
            
            Directory dir = new Directory();
            if (!dir.DirExists(LOG_FILE_DIR))
            {
                Error err = dir.MakeDirRecursive(LOG_FILE_DIR);
                if (err != Error.Ok)
                {
                    GD.PushError("Log file directory creation failed: " + err.ToString());
                    return false;
                }
            }

            Dictionary date = OS.GetDatetime();

            string year = GD.Str(date["year"]);
            string month = (int)date["month"] > 9 ? GD.Str(date["month"]) : GD.Str(0, date["month"]);
            string day = (int)date["day"] > 9 ? GD.Str(date["day"]) : GD.Str(0, date["day"]);

            string hour = (int)date["hour"] > 9 ? GD.Str(date["hour"]) : GD.Str(0, date["hour"]);
            string minute = (int)date["minute"] > 9 ? GD.Str(date["minute"]) : GD.Str(0, date["minute"]);
            string second = (int)date["second"] > 9 ? GD.Str(date["second"]) : GD.Str(0, date["second"]);

            _logFilePath = LOG_FILE_PATH + year + "_" + month + "_" + day + "_" + hour + "_" + minute + "_" + day + LOG_FILE_EXT;

            File file = new File();
            var error = file.Open(_logFilePath, File.ModeFlags.Write);
            if (error == Error.Ok)
            {
                file.StoreString("Date\tLogNumber\tLogLevel\t[Caller|Method]\tMessage\n");
                file.Close();
            }
            else
            {
                GD.PushError("Log file save failed: " + error.ToString());
                return false;
            }

            _initialized = true;
            Logger.Log("Logger successfully initialized.", LogLevel.Info);

            if (RemoveOldLogs())
            {
                Log("Removed old log files.", LogLevel.Info);
            }

            return true;
        }
    }

    private static bool _initialized = Initialized;

    public static void Log(object message, LogLevel logLevel = LogLevel.Trace, bool showPrefix = true)
    {
        #if DEBUG
            if (!_initialized)
            {
                GD.PushError("Logger is not initiated!");
                return;
            }

            StackFrame frame = new StackFrame(1);
            if (Engine.EditorHint)
                GD.Print($"[{logLevel}]\t[{frame.GetMethod().DeclaringType.Name}|{frame.GetMethod().Name}]\t{message}");
            else
            {
                LogMessage(message.ToString(), frame.GetMethod().Name, frame.GetMethod().DeclaringType.Name, showPrefix, logLevel);
            }
        #endif
    }

    public static void SetLogLevel(LogLevel level)
    {
        _logLevel = level;
    }

    private static void LogMessage(string message, string methodName, string callerName, bool showPrefix = true, LogLevel logLevel = LogLevel.Trace)
    {
        if (_logLevel > logLevel)
            return;

        string result = "";

        if (showPrefix)
        {
            Godot.Collections.Dictionary date = OS.GetDatetime();

            string year = GD.Str(date["year"]);
            string month = (int)date["month"] > 9 ? GD.Str(date["month"]) : GD.Str(0, date["month"]);
            string day = (int)date["day"] > 9 ? GD.Str(date["day"]) : GD.Str(0, date["day"]);

            string hour = (int)date["hour"] > 9 ? GD.Str(date["hour"]) : GD.Str(0, date["hour"]);
            string minute = (int)date["minute"] > 9 ? GD.Str(date["minute"]) : GD.Str(0, date["minute"]);
            string second = (int)date["second"] > 9 ? GD.Str(date["second"]) : GD.Str(0, date["second"]);

            result += GD.Str("[", year, "-", month, "-", day, " ", hour, ":", minute, ":", second, "]\t");

            result += GD.Str("[", _logNumber++ ,"]\t");

            switch (logLevel)
            {
                case LogLevel.Trace:
                    result += "[TRACE]\t";
                    break;
                case LogLevel.Info:
                    result += "[INFO]\t";
                    break;
                case LogLevel.Warning:
                    result += "[WARN]\t";
                    break;
                case LogLevel.Error:
                    result += "[ERROR]\t";
                    break;
            }

            result += GD.Str("[", callerName, "|", methodName, "]\t");
        }

        message = message.Replace("\\r\\n", "\n");
        message = message.Replace("\\n", "\n");
        message = message.Replace("\\t", "\t");
        
        result += message;

        GD.Print(result);

        if (logLevel > LogLevel.Info)
        {
            string stack = new System.Diagnostics.StackTrace().ToString();
            stack = stack.Remove(0, 4);
            stack = stack.Remove(0, stack.Find("at") + 2);
            stack = stack.Remove(0, stack.Find("  at"));

            GD.Print(stack);
        }

        File file = new File();
        var error = file.Open(_logFilePath, File.ModeFlags.ReadWrite);
        if (error == Error.Ok)
        {
            file.SeekEnd();
            file.StoreString(result + "\n");
            file.Close();
        }
        else
        {
            GD.PushError($"Log file ({_logFilePath}) save failed: " + error.ToString());
        }
    }

    public static bool RemoveOldLogs()
    {
        if (!_initialized)
        {
            GD.PushError("Logger is not initiated!");
            return false;
        }

        Directory dir = new Directory();
        Error error = dir.Open(LOG_FILE_DIR);
        if (error != Error.Ok)
        {
            Logger.Log($"Cannot open log files directory: {error.ToString()}", LogLevel.Error);
            return false;
        }

        error = dir.ListDirBegin(true, true);
        if (error != Error.Ok)
        {
            Logger.Log($"Cannot list log files: {error.ToString()}", LogLevel.Error);
            return false;
        }

        System.Collections.Generic.List<string> logFiles = new System.Collections.Generic.List<string>();
        string logFile;
        while (!(logFile = dir.GetNext()).Empty())
        {
            if (logFile.EndsWith(LOG_FILE_EXT))
                logFiles.Add(logFile);
        }

        dir.ListDirEnd();

        logFiles.Sort();

        while (logFiles.Count > NUMBER_OF_LOG_FILES)
        {
            error = dir.Remove(logFiles[0]);
            if (error != Error.Ok)
            {
                Logger.Log($"Cannot delete log file \"{logFiles[0]}\": {error.ToString()}", LogLevel.Error);
                return false;
            }
            logFiles.RemoveAt(0);
        }

        return true;
    }
}
