using Godot;
using HorrorGame.Data;
using HorrorGame.Data.Diaries;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Diaries
{
    public class TextDiaryReader : Control, IDiaryReader
    {
		[Export] NodePath RichTextLabelNodePath { get; set; }

        public override void _Ready()
        {
            
        }

        public override void _ExitTree()
        {
            
        }

        public void OpenDiary(string diaryId, DiaryData data)
        {
			if (data is TextDiaryData tdd)
			{
				GetNode<RichTextLabel>(RichTextLabelNodePath).BbcodeText = TranslationServer.Translate(tdd.Content).Replace("<<new_line>>", "\n");
			}
			else
			{
				Logger.Log($"Wrong type of DiaryData: was {data.GetType().Name}, expected {typeof(TextDiaryData).Name}", LogLevel.Error);
				MainManager.Instance.ExitGame(ExitCode.WrongDataType);
			}
        }

		public void CloseDiary()
        {
            
        }
    }
}