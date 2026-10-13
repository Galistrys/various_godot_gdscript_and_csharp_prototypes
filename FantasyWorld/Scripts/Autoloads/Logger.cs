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
    private static LogLevel _logLevel;
    private static uint _logNumber = 0;
    private static Logger _instance = new Logger();

    public override void _Ready()
    {
        PauseMode = PauseModeEnum.Process;
    }

    public static void Log(object message, LogLevel logLevel = LogLevel.Trace, bool showPrefix = true)
    {
        #if DEBUG
            StackFrame frame = new StackFrame(1);
            _instance.LogMessage(message.ToString(), frame.GetMethod().Name, frame.GetMethod().DeclaringType.Name, showPrefix, logLevel);
        #endif
    }

    void LogMessage(string message, string methodName, string callerName, bool showPrefix = true, LogLevel logLevel = LogLevel.Trace)
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
                    result += "[ TRACE ]\t";
                    break;
                case LogLevel.Info:
                    result += "[ INFO ]\t";
                    break;
                case LogLevel.Warning:
                    result += "[ WARNING ]\t";
                    break;
                case LogLevel.Error:
                    result += "[ ERROR ]\t";
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
