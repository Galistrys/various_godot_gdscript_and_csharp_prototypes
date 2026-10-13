using Godot;
using Godot.Collections;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Data.Diaries
{
    public abstract class DiaryData
    {
        public const string DIARY_DATA_DIR = "res://Resources/Data/Diaries/Data";
        public const string DIARY_AUTHOR_DIR = "res://Resources/Data/Diaries/Authors";

        public const string DIARY_AUTHOR_ICON_EXT = "tex";
        
        public const string DIARY_DATE_KEY = "date";
        public const string DIARY_TYPE_KEY = "type";
        public const string DIARY_AUTHOR_KEY = "author";
        public const string DIARY_TITLE_KEY = "title";

        public string Date { get; private set; }
        public string Author { get; private set; }
        public string Title { get; private set; }

        public DiaryData(string diaryId, Dictionary<string, object> data)
        {
            Date = (string)data[DIARY_DATE_KEY];

            Author = (string)data[DIARY_AUTHOR_KEY];
            Title = (string)data[DIARY_TITLE_KEY];
        }

        public static ImageTexture GetAuthorIcon(string author)
        {
            return ResourceLoader.Load<ImageTexture>(DIARY_AUTHOR_DIR + "/" + author + "." + DIARY_AUTHOR_ICON_EXT);
        } 
    }
}