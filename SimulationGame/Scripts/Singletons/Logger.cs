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

public class Logger : Node
{
    private LogLevel _logLevel;

    private int _logNumber = 0;

    public override void _Ready()
    {
        SetPauseMode(PauseModeEnum.Process);
        
        SINGLETONS.Logger = this;

        Log("Logger initialized.", true, LogLevel.Info);
    }

    public static void Log(string message, bool showPrefix = true, LogLevel logLevel = LogLevel.Trace)
    {
        #if DEBUG
            StackFrame frame = new StackFrame(1);
            SINGLETONS.Logger.LogMessage(message, frame.GetMethod().Name, frame.GetMethod().DeclaringType.Name, showPrefix, logLevel);
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

            result += GD.Str("[", ++_logNumber ,"]\t");

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
    }
}