using Godot;
using Godot.Collections;
using HorrorGame.Data.Diaries;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Data
{
    public class GameSave
    {
        public const string PLAYER_POS_KEY = "player_position";
        public const string PLAYER_ROT_KEY = "player_rotation";
        public const string CURRENT_MAP_ID_KEY = "current_map_id";
        public const string ITEMS_KEY = "items";
        public const string NOTES_KEY = "notes";
        public const string DIARIES_KEY = "diaries";
        public const string QUESTS_KEY = "quests";
        public const string COMPLETED_QUESTS_KEY = "completed";
        public const string IN_PROGRESS_QUESTS_KEY = "in_progress";
        public const string EVENTS_KEY = "events";
        public const string LEVELS_DATA_KEY = "levels_data";

        public string FileName { get; private set; }

        public Vector3 PlayerPosition { get; set; } = Vector3.Zero;
        public Vector3 PlayerRotation { get; set; } = Vector3.Zero;
        public string CurrentMapId { get; set; } = "TestLevel1";

        Array<string> Items { get; set; } = new Array<string>();
        Array<string> Notes { get; set; } = new Array<string>();
        Array<string> Diaries { get; set; } = new Array<string>();
        Array<string> CompletedQuests { get; set; } = new Array<string>();
        Dictionary<string, int> InProgressQuests { get; set; } = new Dictionary<string, int>();
        Array<string> Events { get; set; } = new Array<string>();
        Dictionary<string, Dictionary<string, Dictionary<string, object>>> LevelsData { get; set; } = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

        public GameSave(string saveName)
        {
            FileName = saveName;
        }

        /// <summary>
        /// Saves data stored in GameSave object to file
        /// </summary>
        /// <returns>Returns <b>true</b> when saved successfully</returns>
        public bool SaveData()
        {
            Logger.Log($"Saving game data: {FileName}", LogLevel.Info);

            Dictionary<string, object> data = new Dictionary<string, object>();
            
            if (MainManager.Instance.GameManager != null)
            {
                PlayerPosition = MainManager.Instance.GameManager.PlayerEntity.Translation;
                PlayerRotation = MainManager.Instance.GameManager.PlayerEntity.Rotation;
            }
            else
            {
                PlayerPosition = Vector3.Zero;
                PlayerRotation = Vector3.Zero;
            }

            data.Add(PLAYER_POS_KEY, GD.Var2Str(PlayerPosition));
            data.Add(PLAYER_ROT_KEY, GD.Var2Str(PlayerRotation));
            data.Add(CURRENT_MAP_ID_KEY, CurrentMapId);

            data.Add(ITEMS_KEY, Items);
            data.Add(NOTES_KEY, Notes);
            data.Add(DIARIES_KEY, Diaries);
            data.Add(QUESTS_KEY, new Dictionary<string, object>() {
                {
                    COMPLETED_QUESTS_KEY,
                    CompletedQuests
                },
                {
                    IN_PROGRESS_QUESTS_KEY,
                    InProgressQuests
                }
            });
            data.Add(EVENTS_KEY, Events);
            data.Add(LEVELS_DATA_KEY, LevelsData);

            File file = new File();
            
            Logger.Log("Saving game save file.");
            Error error;
            #if DEBUG
                error = file.Open(GameSaveManager.SAVE_DIR + "/" + FileName + GameSaveManager.SAVE_FILE_EXT, File.ModeFlags.Write);
            #else
                error = file.OpenEncryptedWithPass(GameSaveManager.SAVE_DIR + "/" + FileName + SAVE_FILE_EXT, File.ModeFlags.Write, "You4r34Fuck1ingH4ck3r");
            #endif

            if (error != Error.Ok)
            {
                Logger.Log("Failed to open file \"" + FileName + "\": " + error.ToString(), LogLevel.Error);
                return false;
            }

            file.StoreLine(JSON.Print(data, "\t"));
            file.Close();

            Logger.Log("Game save file saved.", LogLevel.Info);

            return true;
        }

        /// <summary>
        /// Loads data stored in file to GameSave object
        /// </summary>
        /// <returns>Returns <b>true</b> when loaded successfully</returns>
        public bool LoadData()
        {
            Logger.Log($"Loading game data: {FileName}", LogLevel.Info);

            File file = new File();
            
            Error error;
            #if DEBUG
                error = file.Open(GameSaveManager.SAVE_DIR + "/" + FileName + GameSaveManager.SAVE_FILE_EXT, File.ModeFlags.Read);
            #else
                error = file.OpenEncryptedWithPass(GameSaveManager.SAVE_DIR + "/" + FileName + SAVE_FILE_EXT, File.ModeFlags.Read, "You4r34Fuck1ingH4ck3r");
            #endif

            if (error != Error.Ok)
            {
                Logger.Log("Failed to open file \"" + FileName + "\": " + error.ToString(), LogLevel.Error);
                return false;
            }

            var result = JSON.Parse(file.GetAsText());
            if (result.Error != Error.Ok)
            {
                Logger.Log("Failed to parse save data.", LogLevel.Error);
                return false;
            }

            Dictionary<string, object> data = new Dictionary<string, object>((Dictionary)result.Result);
            file.Close();

            try
            {
                PlayerPosition = (Vector3)GD.Str2Var(data[PLAYER_POS_KEY] as string);
                PlayerRotation = (Vector3)GD.Str2Var(data[PLAYER_ROT_KEY] as string);
                CurrentMapId = (string)data[CURRENT_MAP_ID_KEY];

                Items = new Array<string>(data[ITEMS_KEY] as Godot.Collections.Array);
                Notes = new Array<string>(data[NOTES_KEY] as Godot.Collections.Array);
                Diaries = new Array<string>(data[DIARIES_KEY] as Godot.Collections.Array);

                Dictionary<string, object> quests = new Dictionary<string, object>(data[QUESTS_KEY] as Dictionary);
                CompletedQuests = new Array<string>(quests[COMPLETED_QUESTS_KEY] as Godot.Collections.Array);
                InProgressQuests = new Dictionary<string, int>(quests[IN_PROGRESS_QUESTS_KEY] as Dictionary);

                Events = new Array<string>(data[EVENTS_KEY] as Godot.Collections.Array);
                LevelsData = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>(data[LEVELS_DATA_KEY] as Dictionary);
            }
            catch (Exception e)
            {
                Logger.Log("An error occured during save loading: " + e.Message);
                return false;
            }

            return true;
        }

        #region LEVELS_DATA

        public bool ContainsLevelData(string levelId)
        {
            return LevelsData.ContainsKey(levelId);
        }

        public Dictionary<string, Dictionary<string, object>> GetLevelData(string levelId)
        {
            Logger.Log($"Getting \"{levelId}\" level's data...");
            Dictionary<string, Dictionary<string, object>> result;
            if (!LevelsData.TryGetValue(levelId, out result))
            {
                Logger.Log($"Level with \"{levelId}\" id not found.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.LevelDoNotExists);
                return null;
            }

            return result;
        }

        public void AddLevelData(string levelId, Dictionary<string, Dictionary<string, object>> data)
        {
            Logger.Log($"Adding data to \"{levelId}\" level.");
            LevelsData.Add(levelId, data);
        }

        #endregion

        #region ITEMS

        public Array<string> GetItems()
        {
            return Items;
        }

        public string GetItem(int idx)
        {
            Logger.Log($"Getting item from index: {idx}");
            if (idx > -1 && idx < Items.Count)
                return Items[idx];
            else
            {
                Logger.Log("Index out of bounds.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.IndexOutOfBounds);
                return null;
            }
        }

        public void RemoveItem(int idx)
        {
            Logger.Log($"Removing item from index: {idx}");
            if (idx > -1 && idx < Items.Count)
                Items.RemoveAt(idx);
            else
            {
                Logger.Log("Index out of bounds.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.IndexOutOfBounds);
            }
        }

        public void RemoveItem(string itemId)
        {
            Logger.Log($"Removing item with id: {itemId}");
            if (Items.Contains(itemId))
                Items.Remove(itemId);
            else
            {
                Logger.Log($"Item with id \"{itemId}\" is not present in Items.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.ItemIdNotFound);
            }
        }

        public void AddItem(string itemId)
        {
            Logger.Log($"Adding item: {itemId}");
            Items.Add(itemId);
        }

        #endregion
    
        #region NOTES

        public Array<string> GetNotes()
        {
            return Notes;
        }

        public string GetNote(int idx)
        {
            Logger.Log($"Getting note from index: {idx}");
            if (idx > -1 && idx < Notes.Count)
                return Notes[idx];
            else
            {
                Logger.Log("Index out of bounds.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.IndexOutOfBounds);
                return null;
            }
        }

        public void AddNote(string noteId)
        {
            Notes.Add(noteId);
        }

        #endregion
    
        #region DIARIES

        public void AddDiary(string diaryId)
        {
            Diaries.Add(diaryId);
        }

        public Array<string> GetDiaries()
        {
            return Diaries;
        }

        #endregion
    }
}