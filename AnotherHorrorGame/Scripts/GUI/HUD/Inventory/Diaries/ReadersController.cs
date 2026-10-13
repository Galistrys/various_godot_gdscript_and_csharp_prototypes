using Godot;
using HorrorGame.Data;
using HorrorGame.Data.Diaries;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Diaries
{
	public class ReadersController : MarginContainer
	{
		[Export] NodePath DiaryTitleLabelNodePath { get; set; }
		[Export] NodePath DiaryAuthorIconNodePath { get; set; }
		[Export] NodePath DiaryAuthorNameAndDateNodePath { get; set; }

		[Export] NodePath TextDiaryReaderNodePath { get; set; }
		[Export] NodePath AudioDiaryReaderNodePath { get; set; }

		IDiaryReader _current = null;

        public override void _Ready()
        {
            
        }

        public override void _ExitTree()
        {
            
        }

		public void OpenDiary(string diaryId)
		{
			DiaryData data = Database.Instance.GetDiary(diaryId);
			if (data == null)
				return;
			
			if (data is TextDiaryData tdd)
			{
				_current = GetNode<TextDiaryReader>(TextDiaryReaderNodePath);
			}
			else if (data is AudioDiaryData add)
			{
				_current = GetNode<AudioDiaryReader>(AudioDiaryReaderNodePath);
			}

			GetNode<Label>(DiaryTitleLabelNodePath).Text = data.Title;
			GetNode<TextureRect>(DiaryAuthorIconNodePath).Texture = DiaryData.GetAuthorIcon(data.Author);
			GetNode<Label>(DiaryAuthorNameAndDateNodePath).Text = TranslationServer.Translate(data.Author) + "\n" + data.Date;

			_current.OpenDiary(diaryId, data);
			(_current as Control).Visible = true;
		}

		public void CloseDiary()
		{
			(_current as Control).Visible = false;
			_current.CloseDiary();
			_current = null;
		}
	}
}