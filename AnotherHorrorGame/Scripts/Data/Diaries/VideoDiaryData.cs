using Godot;
using Godot.Collections;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Data.Diaries
{
    public class VideoDiaryData : DiaryData
    {
		public const string VIDEO_DIARY_EXT = "ogv";

		public TranscriptData Transcript { get; private set; }

		public string VideoPath { get; private set; }

        public VideoDiaryData(string diaryId, Dictionary<string, object> data) : base(diaryId, data)
        {
			VideoPath = DIARY_DATA_DIR + "/" + diaryId + "/" + diaryId + "." + VIDEO_DIARY_EXT;
			File file = new File();
			if (!file.FileExists(VideoPath))
			{
				Logger.Log("Video file does not exists.", LogLevel.Error);
				MainManager.Instance.ExitGame(ExitCode.FileNotFound);
				return;
			}

			Transcript = new TranscriptData(DIARY_DATA_DIR + "/" + diaryId + "/" + diaryId + "." + TranscriptData.TRANSCRIPT_EXT);

			Logger.Log("Created: " + this.ToString());
        }

		public override string ToString()
        {
            return $"VideoDiaryData(Date: {Date}, Author: {Author}, Title: {Title})";
        }
    }
}