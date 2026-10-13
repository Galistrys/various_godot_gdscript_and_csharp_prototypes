using Godot;
using Godot.Collections;
using System;

namespace HorrorGame.Data.Diaries
{
    public class TextDiaryData : DiaryData
    {
		public const string DIARY_CONTENT_KEY = "content";

		public string Content { get; private set; }
		
        public TextDiaryData(string diaryId, Dictionary<string, object> data) : base(diaryId, data)
        {
			Content = (string)data[DIARY_CONTENT_KEY];

			Logger.Log("Created: " + this.ToString());
        }

        public override string ToString()
        {
            return $"TextDiaryData(Date: {Date}, Author: {Author}, Title: {Title}, Content: {Content})";
        }
    }
}