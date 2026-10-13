using Godot;
using HorrorGame.Data.Diaries;
using HorrorGame.Managers;
using System;

namespace HorrorGame.GUI.HUD.Inventory.Diaries
{
    public class AudioDiaryReader : Control, IDiaryReader
    {
        const string ANIM_NAME = "subtitles";

        [Export] NodePath TimeSliderNodePath { get; set; }
        [Export] NodePath TimeLabelNodePath { get; set; }

        [Export] NodePath AudioStreamPlayerNodePath { get; set; }
        [Export] NodePath TranscriptLabelNodePath { get; set; }
        [Export] NodePath TranscriptPlayerNodePath { get; set; }

        Button _ffButton;
        Button _reButton;
        HSlider _timeSlider;
        Label _timeLabel;
        AudioStreamPlayer _audioPlayer;
        Label _transcriptLabel;
        AnimationPlayer _transcriptPlayer;

        System.Collections.Generic.List<TranscriptData.Entry> _entries;

        string _maxTime;
        float _currentTime = 0;

        public override void _Ready()
        {
            _timeSlider = GetNode<HSlider>(TimeSliderNodePath);
            _timeLabel = GetNode<Label>(TimeLabelNodePath);

            _audioPlayer = GetNode<AudioStreamPlayer>(AudioStreamPlayerNodePath);
            _transcriptLabel = GetNode<Label>(TranscriptLabelNodePath);
            _transcriptPlayer = GetNode<AnimationPlayer>(TranscriptPlayerNodePath);
        }

        public override void _ExitTree()
        {
            _ffButton = null;
            _reButton = null;

            _timeSlider = null;
            _timeLabel = null;

            _audioPlayer = null;
            _transcriptLabel = null;
            _transcriptPlayer = null;
        }

        public override void _Input(InputEvent @event)
        {
            if (_audioPlayer.Stream != null)
            {
                if (@event is InputEventMouseButton iemb)
                {
                    if (iemb.ButtonIndex == (int)ButtonList.Left)
                    {
                        Vector2 pos = _timeSlider.GetLocalMousePosition();
                        if (new Rect2(Vector2.Zero, _timeSlider.RectSize).HasPoint(pos))
                        {
                            ChangeTime(pos.x / _timeSlider.RectSize.x * _audioPlayer.Stream.GetLength());
                        }
                    }
                }
            }
        }

        public void OpenDiary(string diaryId, DiaryData data)
        {
            if (data is AudioDiaryData add)
            {
                _audioPlayer.Stream = add.Audio;

                Animation animation = new Animation();
                animation.AddTrack(Animation.TrackType.Method);
                animation.TrackSetPath(0,this.GetPath());
                animation.Length = add.Audio.GetLength();

                _entries = add.Transcript.Entries;

                int count = 0;
                foreach (TranscriptData.Entry entry in _entries)
                {
                    animation.TrackInsertKey(
                        0,
                        entry.StartTime,
                        new Godot.Collections.Dictionary() {
                            {
                                "method",
                                nameof(ShowEntry)
                            },
                            {
                                "args",
                                new Godot.Collections.Array() { count++ }
                            }
                        }
                    );

                    animation.TrackInsertKey(
                        0,
                        entry.EndTime,
                        new Godot.Collections.Dictionary() {
                            {
                                "method",
                                nameof(HideEntry)
                            },
                            {
                                "args",
                                new Godot.Collections.Array()
                            }
                        }
                    );

                    _transcriptPlayer.AddAnimation(ANIM_NAME, animation);

                    float maxTime = _audioPlayer.Stream.GetLength();
                    _timeSlider.MaxValue = maxTime;

                    int hours = Mathf.FloorToInt(maxTime / 3600.0f);
                    int minutes = Mathf.FloorToInt((maxTime - hours * 3600) / 60.0f);
                    int seconds = Mathf.FloorToInt(maxTime - hours * 3600 - minutes * 60);

                    _maxTime = $"{(hours > 9 ? GD.Str(hours) : GD.Str("0", hours))}:{(minutes > 9 ? GD.Str(minutes) : GD.Str("0", minutes))}:{(seconds > 9 ? GD.Str(seconds) : GD.Str("0", seconds))}";
                    _currentTime = 0;
                }
            }
            else
            {
                Logger.Log($"Wrong type of DiaryData: was {data.GetType().Name}, expected {typeof(TextDiaryData).Name}", LogLevel.Error);
                MainManager.Instance.ExitGame(ExitCode.WrongDataType);
            }
        }

        public void CloseDiary()
        {
            Stop();
            _audioPlayer.Stream = null;
            _entries = null;
            
            _transcriptPlayer.RemoveAnimation(ANIM_NAME);
        }

        void ShowEntry(int id)
        {
            _transcriptLabel.Text = _entries[id].Text;
            Logger.Log($"Show entry: {id}");
        }

        void HideEntry()
        {
            _transcriptLabel.Text = "";
            Logger.Log($"Hide entry");
        }

        public override void _Process(float delta)
        {
            _currentTime = _audioPlayer.GetPlaybackPosition();

            _timeSlider.Value = _currentTime;

            int hours = Mathf.FloorToInt(_currentTime / 3600.0f);
            int minutes = Mathf.FloorToInt((_currentTime - hours * 3600) / 60.0f);
            int seconds = Mathf.FloorToInt(_currentTime - hours * 3600 - minutes * 60);
            
            _timeLabel.Text = $"{(hours > 9 ? GD.Str(hours) : GD.Str("0", hours))}:{(minutes > 9 ? GD.Str(minutes) : GD.Str("0", minutes))}:{(seconds > 9 ? GD.Str(seconds) : GD.Str("0", seconds))} / {_maxTime}";;
        }

        void ChangeTime(float val)
        {
            _audioPlayer.Seek(val);
        }

        void OnPlayPauseButtonPressed()
        {
            // float cur = _audioPlayer.GetPlaybackPosition();

            // int count = 0;
            // foreach (var entry in _entries)
            // {
            //     if (cur >= entry.StartTime || cur < entry.EndTime)
            //     {
            //         ShowEntry(count);
            //         break;
            //     }
            //     count++;
            // }

            Play();
        }

        void OnStopButtonPressed()
        {
            Stop();
        }

        void OnAudioStreamPlayerFinished()
        {
            Stop();
        }

        void Play()
        {
            _audioPlayer.Play(_currentTime);
            _transcriptPlayer.Play(ANIM_NAME);
            _transcriptPlayer.Advance(_currentTime);

            SetProcess(true);
        }

        void Stop()
        {
            SetProcess(false);

            _audioPlayer.Stop();

            _currentTime = 0;
            
            _transcriptPlayer.Stop(true);
            _transcriptLabel.Text = "";
        }
    }
}