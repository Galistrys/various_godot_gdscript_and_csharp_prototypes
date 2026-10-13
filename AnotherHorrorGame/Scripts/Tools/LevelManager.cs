using Godot;
using Godot.Collections;
using HorrorGame.Entities;
using HorrorGame.Levels;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Tools
{
    public class LevelManager : Node
    {
        public const string LEVELS_DIR = "res://Resources/Levels";
        public const string LEVELS_EXT = ".res";

        string _oldLevelId = null;

        Level CurrentLevel
        {
            get
            {
                if (GetChildCount() == 0)
                    return null;
                return GetChildOrNull<Level>(0);
            }
        }

        Thread _levelThread;

        public override void _Ready()
        {

        }

        string GetLevelPath(string levelId)
        {
            return LEVELS_DIR + "/" + levelId + "/" + levelId + LEVELS_EXT;
        }

        public void ChangeLevel(string levelId)
        {
            Logger.Log($"Changing level to: {levelId}");

            if (!ResourceLoader.Exists(GetLevelPath(levelId)))
            {
                Logger.Log($"Level with id \"{levelId}\" does not exists.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.LevelDoNotExists);
                return;
            }

            _levelThread = new Thread();

            if (CurrentLevel != null)
                _oldLevelId = GameSaveManager.CurrentGameSave.CurrentMapId;

            _levelThread.Start(this, nameof(ChangeLevelInThread), levelId);
        }

        Node ChangeLevelInThread(string levelId)
        {
            if (_oldLevelId != null)
            {
                SaveLevelData();

                Logger.Log("Removing old level.");
                CurrentLevel.QueueFree();
            }

            string path = GetLevelPath(levelId);

            Logger.Log("Loading new level.");
            using (var loader = ResourceLoader.LoadInteractive(path))
            {
                Error err;

                do
                {
                    OS.DelayUsec(16000);
                    err = loader.Poll();
                } while (err == Error.Ok);

                if (err != Error.FileEof)
                {
                    Logger.Log("ResourceInteractiveLoader failed with error: " + err.ToString(), LogLevel.Error);
                    return null;
                }

                Logger.Log("Level loaded into PackedScene.");
                Node node = (loader.GetResource() as PackedScene).Instance() as Node;
                CallDeferred(nameof(EndLevelChanging), levelId);
                return node;
            }
        }

        void EndLevelChanging(string levelId)
        {
            Node lvl = _levelThread.WaitToFinish() as Node;
            if (lvl == null)
                MainManager.Instance.ExitGame(ExitCode.LoadDataFailed);

            Logger.Log("Change level thread finished.");

            lvl.PauseMode = PauseModeEnum.Stop;
            _levelThread = null;

            AddChild(lvl);
            
            _levelThread = new Thread();
            _levelThread.Start(this, nameof(LoadDataInThread), levelId);
        }

        bool LoadDataInThread(string levelId)
        {
            if (_oldLevelId != null)
            {
                Position3D position = CurrentLevel.GetSpawnPoint(_oldLevelId);
                if (position == null)
                {
                    Logger.Log($"Position at level {CurrentLevel.Name} from level {_oldLevelId} not found.", LogLevel.Error);
                    return false;
                }
                MainManager.Instance.GameManager.PlayerController.Translation = position.Translation;
                MainManager.Instance.GameManager.PlayerController.Rotation = position.Rotation;
            }
            else
            {
                MainManager.Instance.GameManager.PlayerController.Translation = GameSaveManager.CurrentGameSave.PlayerPosition;
                MainManager.Instance.GameManager.PlayerController.Rotation = GameSaveManager.CurrentGameSave.PlayerRotation;
            }

            LoadLevelData(levelId);
            CallDeferred(nameof(EndLoadingData), levelId);
            return true;
        }

        void EndLoadingData(string levelId)
        {
            if (!(bool)_levelThread.WaitToFinish())
            {
                MainManager.Instance.ExitGame(ExitCode.PositionNotFound);
                return;
            }
            
            _levelThread = null;

            MainManager.Instance.GameManager.ChangeLevelFinished(levelId);
        }

        /// <summary>
        /// Converts NodePath to path relative to <b>current</b> level
        /// </summary>
        /// <param name="path">Node's absolute path</param>
        /// <returns>Node's path relative to current level</returns>
        public static string ConvertNodePath(NodePath path)
        {
            string result = "";
            int count = 0;
            while (path.GetName(count) != GameSaveManager.CurrentGameSave.CurrentMapId)
            {
                count++;
            }
            count++;

            for (int i = count; i < path.GetNameCount(); i++)
            {
                result = result + "/" + path.GetName(i);
            }

            return result.Remove(0, 1);
        }

        public void SaveLevelData()
        {
            Logger.Log("Saving level data...", LogLevel.Info);
            var entities = GetTree().GetNodesInGroup("entity");

            if (!GameSaveManager.CurrentGameSave.ContainsLevelData(GameSaveManager.CurrentGameSave.CurrentMapId))
            {
                GameSaveManager.CurrentGameSave.AddLevelData(GameSaveManager.CurrentGameSave.CurrentMapId, new Dictionary<string, Dictionary<string, object>>());
            }
            var levelData = GameSaveManager.CurrentGameSave.GetLevelData(GameSaveManager.CurrentGameSave.CurrentMapId);

            foreach (Node entity in entities)
            {
                if (entity is IEntity ie)
                {
                    SaveEntityData(levelData, ConvertNodePath(entity.GetPath()), ie);
                }
                else
                {
                    Logger.Log(entity.ToString() + " node is not valid IEntity object.", LogLevel.Error);
                }
            }

            Logger.Log("Level data saved.", LogLevel.Info);
        }

        void SaveEntityData(Dictionary<string, Dictionary<string, object>> levelData, string name, IEntity entity)
        {
            Logger.Log("Saving data of: " + name);
            
            if (levelData.ContainsKey(name))
            {
                levelData[name] = entity.SaveToDictionary();
            }
            else
            {
                levelData.Add(name, entity.SaveToDictionary());
            }  
        }

        public void LoadLevelData(string levelId)
        {
            Logger.Log("Loading level data...", LogLevel.Info);

            if (GameSaveManager.CurrentGameSave.ContainsLevelData(levelId))
            {
                foreach (var kvpair in GameSaveManager.CurrentGameSave.GetLevelData(levelId))
                {
                    LoadEntityData(kvpair.Key, kvpair.Value);
                }
            }

            Logger.Log("Level data loaded.");
        }

        void LoadEntityData(string name, Dictionary<string, object> data)
        {
            Logger.Log("Loading data of: " + name);

            var node = CurrentLevel.GetNode(name);
            if (node is IEntity ie)
            {
                ie.LoadFromDictionary(data);
            }
            else
            {
                Logger.Log(node.ToString() + " is not valid IEntity object.", LogLevel.Error);
            }
        }
    }
}