using Godot;
using HorrorGame.Data;
using HorrorGame.Data.Diaries;
using HorrorGame.GUI.HUD.Inventory.Diaries;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Views
{
    public class DiariesView : Control, IInventoryView
    {
        [Export] NodePath DiariesListNodePath { get; set; }
        [Export] NodePath ReadersControllerNodePath { get; set; }

        ItemList _diariesList;
        Texture _textListTexture = ResourceLoader.Load<StreamTexture>("res://Assets/text.png");
        Texture _audioListTexture = ResourceLoader.Load<StreamTexture>("res://Assets/audio.png");
        Texture _videoListTexture = ResourceLoader.Load<StreamTexture>("res://Assets/video.png");

        int _diarySelected = -1;

        VBoxContainer _diariesCont;
        VBoxContainer _readersCont;

        Godot.Collections.Array<string> _ids = new Godot.Collections.Array<string>();

        ReadersController _readersController;

        bool _isReading = false;

        public override void _Ready()
        {
            _diariesList = GetNode<ItemList>(DiariesListNodePath);

            _diariesCont = GetNode<VBoxContainer>("DiariesList");
            _readersCont = GetNode<VBoxContainer>("DiaryReader");
            
            _readersController = GetNode<ReadersController>(ReadersControllerNodePath);

            _diariesList.Clear();
        }

        public override void _ExitTree()
        {
            _diariesList = null;
            _diariesCont = null;
            _readersCont = null;
            _readersController = null;
        }

        public void Init(object[] data)
        {
            var playersDiaries = GameSaveManager.CurrentGameSave.GetDiaries();
            var gameDiaries = Database.Instance.GetDiariesIds();

            foreach (var gameDiar in gameDiaries)
            {
                if (playersDiaries.Contains(gameDiar))
                {
                    _ids.Add(gameDiar);
                    DiaryData diaryData = Database.Instance.GetDiary(gameDiar);
                    string itemName = diaryData.Date + ": " + TranslationServer.Translate(diaryData.Title); 

                    if (diaryData is TextDiaryData)
                        _diariesList.AddItem(itemName, _textListTexture);
                    if (diaryData is AudioDiaryData)
                        _diariesList.AddItem(itemName, _audioListTexture);
                    if (diaryData is VideoDiaryData)
                        _diariesList.AddItem(itemName, _videoListTexture);
                }
            }

            // if (data == null)
            // {
            //     _diariesCont.Visible = true;
            // }
            // else
            // {
            //     _diariesCont.Visible = false;
            //     // OpenNote(data[0] as string);
            // }

            this.Visible = true;
        }

        public void Exit()
        {
            this.Visible = false;

            _diarySelected = -1;
            _diariesList.Clear();
            _ids.Clear();
        }

        void OnOpenDiaryButtonPressed()
        {
            if (_diarySelected != -1)
                OpenDiary(_ids[_diarySelected]);
        }

        public void Process(float delta)
        {
            
        }

        public void UiCancel()
        {
            if (_isReading)
            {
                CloseDiary();
            }
            else
            {
                MainManager.Instance.GameManager.CloseInventory();
            }
        }

        void OnDiaryListItemSelected(int idx)
        {
            Logger.Log($"Diary selected: {idx}");
            _diarySelected = idx;
        }

        void OnDiaryListItemActivated(int idx)
        {
            Logger.Log($"Diary activated: {idx}");
            _diarySelected = idx;
            
            OpenDiary(_ids[idx]);
        }

        void OnReturnButtonPressed()
        {
            CloseDiary();
        }

        void OpenDiary(string diaryId)
        {
            _diarySelected = -1;
            _diariesList.UnselectAll();

            Logger.Log($"Opening diary with id: {diaryId}");
            _readersController.OpenDiary(diaryId);

            _diariesCont.Visible = false;
            _readersCont.Visible = true;
            _isReading = true;
        }

        void CloseDiary()
        {
            _readersCont.Visible = false;
            _readersController.CloseDiary();
            _diariesCont.Visible = true;

            _isReading = false;
        }
    }
}