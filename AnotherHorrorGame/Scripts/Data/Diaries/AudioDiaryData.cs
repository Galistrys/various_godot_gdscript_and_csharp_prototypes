using Godot;
using Godot.Collections;
using System;

namespace HorrorGame.Data.Diaries
{
    public class AudioDiaryData : DiaryData
    {
		public const string AUDIO_DIARY_EXT = "oggstr";

		public AudioStreamOGGVorbis Audio { get; private set; }
		public TranscriptData Transcript { get; private set; }

		public AudioDiaryData(string diaryId, Dictionary<string, object> data) : base(diaryId, data)
        {
			Audio = ResourceLoader.Load<AudioStreamOGGVorbis>(DIARY_DATA_DIR + "/" + diaryId + "/" + diaryId + "." + AUDIO_DIARY_EXT);
			Transcript = new TranscriptData(DIARY_DATA_DIR + "/" + diaryId + "/" + diaryId + "." + TranscriptData.TRANSCRIPT_EXT);

            Logger.Log("Created: " + this.ToString());
        }

		public override string ToString()
        {
            return $"AudioDiaryData(Date: {Date}, Author: {Author}, Title: {Title})";
        }
    }
}