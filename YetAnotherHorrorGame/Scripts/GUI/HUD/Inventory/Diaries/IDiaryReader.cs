using Godot;
using HorrorGame.Data.Diaries;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Diaries
{
    public interface IDiaryReader
    {
        void OpenDiary(string diaryId, DiaryData data);
        void CloseDiary();
    }
}