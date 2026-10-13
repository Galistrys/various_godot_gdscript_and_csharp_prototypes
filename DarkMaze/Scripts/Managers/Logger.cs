using Godot;
using System;
using System.Diagnostics;

public enum LogLevel
{
    Trace,
    Info,
    Warning,
    Error
}

public class Logger : Node, ISingleton
{
    private LogLevel _logLevel;

    private int _logNumber = 0;

    const string LOG_FILE_PATH = "user://logs/log_file_";
    const string LOG_FILE_EXT = ".log";

    string _logFilePath = "";

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
        Godot.Collections.Dictionary date = OS.GetDatetime();

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
            file.StoreString("Date\tLogNumber\tLogLevel\tCaller|Method\tMessage\n");
            file.Close();
        }
        else
        {
            GD.PushError("Log file save failed: " + error.ToString());
        }

        Log("Logger initialized.", LogLevel.Info);
    }

    public static void Log(object message, LogLevel logLevel = LogLevel.Trace, bool showPrefix = true)
    {
        #if DEBUG
            StackFrame frame = new StackFrame(1);
            if (Engine.EditorHint)
                GD.Print($"[{logLevel}]\t[{frame.GetMethod().DeclaringType.Name}|{frame.GetMethod().Name}]\t{message}");
            else
            {
                Singletons.Logger.LogMessage(message.ToString(), frame.GetMethod().Name, frame.GetMethod().DeclaringType.Name, showPrefix, logLevel);
            }
        #endif
    }

    /// <summary>
    /// Changes the log level of Logger.
    /// Everything benetah below level won't be displayed
    /// </summary>
    /// <param name="level">Log level</param>
    public void SetLogLevel(LogLevel level)
    {
        _logLevel = level;
    }

    /// <summary>
    /// Prints the message to the console
    /// </summary>
    /// <param name="message">Messsage, that will be printed</param>
    /// <param name="showPrefix">If true, it will add perfix with timestemp and log level info</param>
    /// <param name="logLevel">To determine log level</param>
    public void LogMessage(string message, string methodName, string callerName, bool showPrefix = true, LogLevel logLevel = LogLevel.Trace)
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
                    result += "[WARNING]\t";
                    break;
                case LogLevel.Error:
                    result += "[ERROR]\t";
                    break;
            }

            result += GD.Str("[", callerName, "|", methodName,"]\t");
        }

        message = message.Replace("\r\n", "\\n");
        message = message.Replace("\n", "\\n");
        message = message.Replace("\t", "\\t");
        
        result += message;
        GD.Print(result);

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
            GD.PushError("Log file save failed: " + error.ToString());
        }
    }

    public void ResetSingleton()
    {
        
    }
}
