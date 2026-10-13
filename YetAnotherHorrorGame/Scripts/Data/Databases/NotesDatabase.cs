using Godot;
using Godot.Collections;
using System;

namespace HorrorGame.Data.Databases
{
    public class NotesDatabase : GameDataDatabase
    {
        public const string NOTES_DATA_FILE_PATH = "res://Resources/Data/Notes.json";
        Dictionary<string, string> _notes;

        public NotesDatabase()
        {
            _notes = new Dictionary<string, string>();
        }

        public override bool LoadData()
        {
            return LoadData(NOTES_DATA_FILE_PATH);
        }

        protected override bool ConvertData(Dictionary dictionary)
        {
            foreach (var noteId in dictionary.Keys)
            {
                Logger.Log($"Note \"{noteId}\" added");
                _notes.Add(noteId as string, dictionary[noteId] as string);
            }

            return true;
        }

        public bool GetNote(string noteId, out string note)
        {
            return _notes.TryGetValue(noteId, out note);
        }
    }
}