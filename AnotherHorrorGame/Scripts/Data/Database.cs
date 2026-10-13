using Godot;
using HorrorGame.Data.Databases;
using HorrorGame.Data.Diaries;
using HorrorGame.Data.Items;
using HorrorGame.Managers;

namespace HorrorGame.Data
{
    public class Database : Node
    {
        public static Database Instance { get; private set; }

        ItemsDatabase _itemsDatabase;
        NotesDatabase _notesDatabase;
        DiariesDatabase _diariesDatabase;

        public Database()
        {
            if (Instance != null)
            {
                MainManager.Instance.ExitGame(ExitCode.AutoloadDuplication);
                return;
            }

            Instance = this;

            _itemsDatabase = new ItemsDatabase();
            _notesDatabase = new NotesDatabase();
            _diariesDatabase = new DiariesDatabase();
        }

        public bool LoadData()
        {
            if (!_itemsDatabase.LoadData())
                return false;
            
            if (!_notesDatabase.LoadData())
                return false;

            if (!_diariesDatabase.LoadData())
                return false;
            
            return true;
        }

        public bool ItemExists(string itemId)
        {
            return _itemsDatabase.ItemDataExists(itemId);
        }

        public ItemData GetItemData(string itemId)
        {
            ItemData result = null;
            if (!_itemsDatabase.GetItemData(itemId, out result))
            {
                Logger.Log($"Item with \"{itemId}\" id not found.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.ItemIdNotFound);
            }
            return result;
        }

        public string GetNote(string noteId)
        {
            string result = "";
            if (!_notesDatabase.GetNote(noteId, out result))
            {
                Logger.Log($"Note with \"{noteId}\" id not found.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.NoteIdNotFound);
            }
            return result;
        }

        public DiaryData GetDiary(string diaryId)
        {
            DiaryData result = null;
            if (!_diariesDatabase.GetDiary(diaryId, out result))
            {
                Logger.Log($"Diary with \"{diaryId}\" id not found.", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.DiaryIdNotFound);
            }
            return result;
        }

        public System.Collections.Generic.Dictionary<string, DiaryData>.KeyCollection GetDiariesIds()
        {
            return _diariesDatabase.GetDiariesIds();
        }
    }
}