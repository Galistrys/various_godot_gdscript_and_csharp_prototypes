using Godot;
using System;
using HorrorGame.Data;
using Godot.Collections;

namespace HorrorGame.Managers
{
    public class GameSaveManager : Node
    {
        public const string SAVE_DIR = "user://GameSaves";
        public const string SAVE_FILE_EXT = ".save";

        static GameSave _currentGameSave;
        public static GameSave CurrentGameSave { get { return _currentGameSave; } }

        public GameSaveManager()
        {
            if (Init())
            {
                Logger.Log("GameSaveManager initialized.", LogLevel.Info);
            }
            else
            {
                MainManager.Instance.ExitGame(ExitCode.FailedToInitialize);
                return;
            }
        }

        bool Init()
        {
            Directory dir = new Directory();
            if (!dir.DirExists(SAVE_DIR))
            {
                Error err = dir.MakeDirRecursive(SAVE_DIR);
                if (err != Error.Ok)
                {
                    Logger.Log("Saves directory creation failed: " + err.ToString(), LogLevel.Error);
                    return false;
                }
            }

            return true;
        }

        public bool SaveGame()
        {
            if (_currentGameSave == null)
            {
                Logger.Log("Current game save is null.", LogLevel.Error);
                return false;
            }

            return _currentGameSave.SaveData();
        }

        public bool LoadGame(string saveName)
        {
            _currentGameSave = new GameSave(saveName);
            return _currentGameSave.LoadData();
        }

        public static Array<string> GetSaves()
        {
            Directory dir = new Directory();
            Error err = dir.Open(SAVE_DIR);

            if (err != Error.Ok)
            {
                Logger.Log("Cannot open saves directory: " + err.ToString(), LogLevel.Error);
                return null;
            }

            err = dir.ListDirBegin(true, true);
            if (err != Error.Ok)
            {
                Logger.Log("Cannot list saves directory: " + err.ToString(), LogLevel.Error);
                return null;
            }

            Array<string> saves = new Array<string>();
            string saveName = "";
            while (!(saveName = dir.GetNext()).Empty())
            {
                if (saveName.EndsWith(SAVE_FILE_EXT))
                    saves.Add(saveName.Replace(SAVE_FILE_EXT, ""));
            }

            dir.ListDirEnd();

            return saves;
        }

        public static bool IsSaveFileExisting(string saveName)
        {
            File file = new File();
            return file.FileExists(SAVE_DIR + "/" + saveName + SAVE_FILE_EXT);
        }

        public static bool CreateSave(string saveName)
        {
            if (IsSaveFileExisting(saveName))
            {
                Logger.Log($"Save named {saveName} already exists.", LogLevel.Error);
                return false;
            }
            else
            {
                if (!(new GameSave(saveName).SaveData()))
                {
                    Logger.Log("Failed to create save file.", LogLevel.Error);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Creates necessary dictionaries in GameSave object
        /// </summary>
        /// <param name="levelId">Level's id</param>
        /// <param name="name">Entity's path (use LevelManager.ConvertNodePath() method)</param>
        public static void CreateSaveEntityPath(string levelId, string name)
        {
            if (CurrentGameSave.ContainsLevelData(levelId))
            {
                var levelData = CurrentGameSave.GetLevelData(levelId);
                if (!levelData.ContainsKey(name))
                {
                    levelData.Add(name, new Dictionary<string, object>() {});
                }
            }
            else
            {
                CurrentGameSave.AddLevelData(levelId, new Dictionary<string, Dictionary<string, object>>() { { name, new Dictionary<string, object>() {} } });
            }
        }

        /// <summary>
        /// Checks if necessary dictionaries in GameSave object exists
        /// </summary>
        /// <param name="levelId">Level's id</param>
        /// <param name="name">Entity's path (use LevelManager.ConvertNodePath() method)</param>
        public static bool SaveEntityPathExists(string levelId, string name)
        {
            if (CurrentGameSave.ContainsLevelData(levelId))
            {
                if (CurrentGameSave.GetLevelData(levelId).ContainsKey(name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}