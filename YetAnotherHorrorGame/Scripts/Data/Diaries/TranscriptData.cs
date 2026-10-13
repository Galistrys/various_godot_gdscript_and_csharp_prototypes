using Godot;
using Godot.Collections;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Data.Diaries
{
    public class TranscriptData
    {
        public class Entry
        {
            public float StartTime { get; private set; }
            public float EndTime { get; private set; }
            public string Text { get; private set; }

            public Entry(float start, float end, string text)
            {
                StartTime = start;
                EndTime = end;
                Text = text;
            }
        }

        public System.Collections.Generic.List<Entry> Entries { get; private set; } = new System.Collections.Generic.List<Entry>();
        public const string TRANSCRIPT_EXT = "json";

        public const string START_KEY = "start";
        public const string END_KEY = "end";
        public const string TEXT_KEY = "text";

        public TranscriptData(string path)
        {
            File file = new File();
            if (file.FileExists(path))
            {
                if (file.Open(path, File.ModeFlags.Read) == Error.Ok)
                {
                    string data = file.GetAsText();
                    file.Close();

                    var jsonResult = JSON.Parse(data);
                    if (jsonResult.Error == Error.Ok)
                    {
                        Array<Dictionary<string, object>> result = new Array<Dictionary<string, object>>(jsonResult.Result as Godot.Collections.Array);
                        foreach (var entryDict in result)
                        {
                            Entries.Add(
                                new Entry(
                                    (float)GD.Str2Var(entryDict[START_KEY] as string),
                                    (float)GD.Str2Var(entryDict[END_KEY] as string),
                                    (string)entryDict[TEXT_KEY]
                                )
                            );
                        }
                    }
                    else
                    {
                        Logger.Log($"Failed to parse transcript file! Path: {path}", LogLevel.Error);
                        MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
                    }
                }
                else
                {
                    Logger.Log($"Failed to open Transcript file! Path: {path}", LogLevel.Error);
                    MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
                }
            }
            else
            {
                Logger.Log($"Transcript file does not exists! Path: {path}", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
            }
        }
    }
}