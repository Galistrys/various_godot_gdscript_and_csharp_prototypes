using Godot;
using Godot.Collections;
using HorrorGame.Managers;

namespace HorrorGame.Data.Databases
{
    public abstract class GameDataDatabase
    {
        public abstract bool LoadData();

        protected bool LoadData(string path)
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
                        return ConvertData(jsonResult.Result as Dictionary);
                    }
                    else
                    {
                        Logger.Log($"Failed to parse data file! Path: {path}", LogLevel.Error);
                        MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
                        return false;
                    }
                }
                else
                {
                    Logger.Log($"Failed to open data file! Path: {path}", LogLevel.Error);
                    MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
                    return false;
                }
            }
            else
            {
                Logger.Log($"Data file does not exists! Path: {path}", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);
                return false;
            }
        }

        protected abstract bool ConvertData(Dictionary dictionary);
    }
}