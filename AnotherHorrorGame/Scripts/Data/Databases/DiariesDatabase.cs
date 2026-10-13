using Godot;
using Godot.Collections;
using HorrorGame.Data.Diaries;
using System;

namespace HorrorGame.Data.Databases
{
    public class DiariesDatabase : GameDataDatabase
    {
        public const string DIARIES_DATA_FILE_PATH = "res://Resources/Data/Diaries.json";
        System.Collections.Generic.Dictionary<string, DiaryData> _diariesData;

        public DiariesDatabase()
        {
            _diariesData = new System.Collections.Generic.Dictionary<string, DiaryData>();
            
        }

        public override bool LoadData()
        {
            return LoadData(DIARIES_DATA_FILE_PATH);
        }

        protected override bool ConvertData(Dictionary dictionary)
        {
            System.Collections.Generic.Dictionary<string, DiaryData> tempDict = new System.Collections.Generic.Dictionary<string, DiaryData>();

            foreach (var diaryId in dictionary.Keys)
            {
                // Logger.Log($"Diary \"{diaryDate}\" added");
                // _diariesData.Add(diaryData as string, dictionary[diaryDate] as string);

                DiaryData data = null;

                if ((dictionary[diaryId] as Dictionary)[DiaryData.DIARY_TYPE_KEY] as string == "Text")
                    data = new TextDiaryData(diaryId as string, new Dictionary<string, object>(dictionary[diaryId] as Dictionary));
                else if ((dictionary[diaryId] as Dictionary)[DiaryData.DIARY_TYPE_KEY] as string == "Audio")
                    data = new AudioDiaryData(diaryId as string, new Dictionary<string, object>(dictionary[diaryId] as Dictionary));
                else if ((dictionary[diaryId] as Dictionary)[DiaryData.DIARY_TYPE_KEY] as string == "Video")
                    data = new VideoDiaryData(diaryId as string, new Dictionary<string, object>(dictionary[diaryId] as Dictionary));

                tempDict.Add(diaryId as string, data);
            }

            System.Collections.Generic.List<Tuple<string, DiaryData>> tempList = new System.Collections.Generic.List<Tuple<string, DiaryData>>();

            foreach (var newPair in tempDict)
            {
                int pos = 0;
                foreach (var pair in tempList)
                {
                    int comp = newPair.Value.Date.CompareTo(pair.Item2.Date);
                    if (comp == 0)
                    {
                        comp = newPair.Value.Title.CompareTo(pair.Item2.Title);
                        if (comp < 0)
                        {
                            break;
                        }
                    }
                    else if (comp < 0)
                    {
                        break;
                    }
                    pos++;
                }

                if (pos == tempList.Count)
                    tempList.Add(new Tuple<string, DiaryData>(newPair.Key, newPair.Value));
                else
                    tempList.Insert(pos, new Tuple<string, DiaryData>(newPair.Key, newPair.Value));
            }

            foreach (var tuple in tempList)
            {
                _diariesData.Add(tuple.Item1, tuple.Item2);
            }

            return true;
        }

        public bool GetDiary(string diaryId, out DiaryData diaryData)
        {
            return _diariesData.TryGetValue(diaryId, out diaryData);
        }

        public System.Collections.Generic.Dictionary<string, DiaryData>.KeyCollection GetDiariesIds()
        {
            return _diariesData.Keys;
        }
    }
}
