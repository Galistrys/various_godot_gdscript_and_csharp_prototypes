using Godot;
using Godot.Collections;
using System;

public class GUI : Control
{
    public const string BACKGROUND_FADE_IN_ANIM_NAME = "BackgroundFadeIn";
    public const string BACKGROUND_FADE_OUT_ANIM_NAME = "BackgroundFadeOut";

    Control _control;
    Panel _background;
    AnimationPlayer _animPlayer;

    Dictionary<int, float> _scales = new Dictionary<int, float>()
    {
        {
            0,
            0.5f
        },
        {
            660,
            0.70f
        },
        {
            1010,
            0.75f
        },
        {
            1515,
            1.0f
        },
        {
            2020,
            1.5f
        },
        {
            2525,
            2f
        }
    };

    float _customScale = 0;
    
    public override void _Ready()
    {
        OS.WindowMaximized = true;

        GetTree().Root.Connect("size_changed", this, nameof(UpdateScale));
        _control = GetNode<Control>("GUIManager");
        _background = GetNode<Panel>("Background");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        UpdateScale();
    }

    public void OnGameReady()
    {
        GetNode<GUIManager>("GUIManager").LateReady();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey iek)
        {
            if (iek.Scancode == (int)KeyList.Q && iek.Pressed)
            {
                if (iek.GetScancodeWithModifiers() == 301989969)
                {
                    Logger.Log("Control+Shift+Escape pressed! Exiting!", LogLevel.Warning);
                    GetTree().Quit(0);
                }
            }
        }
    }

    public void UpdateScale()
    {
        _background.RectSize = OS.WindowSize;
        if (_customScale == 0)
        {
            Vector2 windowSize = OS.WindowSize;

            bool found = false;
            foreach (var pair in _scales)
            {
                if (windowSize.y < pair.Key && !found)
                {
                    _control.RectScale = Vector2.One * pair.Value;
                    found = true;
                }
            }

            if (!found)
            {
                _control.RectScale = Vector2.One * _scales[0];
            }
        }
        else
        {
            _control.RectScale = Vector2.One * _customScale;
        }
        
        _control.RectPosition = OS.WindowSize / 2.0f;

        Logger.Log("Current scale: " + GD.Str(_control.RectScale), LogLevel.Info);
    }

    public void BackgroundFadeIn()
    {
        var anim = _animPlayer.GetAnimation(BACKGROUND_FADE_IN_ANIM_NAME);
        anim.TrackSetKeyValue(0, 0, _background.Modulate);
        _animPlayer.Play("BackgroundFadeIn", customSpeed: 4);
    }

    public void BackgroundFadeOut()
    {
        var anim = _animPlayer.GetAnimation(BACKGROUND_FADE_OUT_ANIM_NAME);
        anim.TrackSetKeyValue(0, 0, _background.Modulate);
        _animPlayer.Play("BackgroundFadeOut", customSpeed: 4);
    }
}
