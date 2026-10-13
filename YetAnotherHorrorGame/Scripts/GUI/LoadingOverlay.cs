using Godot;
using System;

namespace HorrorGame.GUI
{
    public class LoadingOverlay : Control
    {
        [Signal]
        public delegate void BackgroundHidden();
        [Signal]
        public delegate void BackgroundShown();

        TextureRect TextImage { get { return GetNode<TextureRect>("Content/VBoxContainer/Image"); } }
        RichTextLabel Text { get { return GetNode<RichTextLabel>("Content/VBoxContainer/Text"); } }
        Label LoadingLabel { get { return GetNode<Label>("Content/VBoxContainer/LoadingLabel"); } }
        Button ProceedButton { get { return GetNode<Button>("Content/VBoxContainer/ProceedButton"); } }

        AnimationPlayer AnimPlayer { get { return GetNode<AnimationPlayer>("AnimationPlayer"); } }

        public bool Active { get; private set; } = true;

        public override void _Ready()
        {
            
        }

        public async void ShowLoading()
        {
            this.Visible = true;
            LoadingLabel.Modulate = Colors.White;
            LoadingLabel.Visible = true;
            ProceedButton.Modulate = new Color(1, 1, 1, 0);
            ProceedButton.Visible = false;
            ProceedButton.Disabled = true;
            ProceedButton.GrabFocus();

            Active = true;
            AnimPlayer.Play("FadeIn");
            await ToSignal(AnimPlayer, "animation_finished");
        }

        void ShowMouse()
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }

        public async void HideLoading()
        {
            Active = false;
            ProceedButton.Disabled = true;
            AnimPlayer.Play("FadeOut");
            Input.SetMouseMode(Input.MouseMode.Captured);
            await ToSignal(AnimPlayer, "animation_finished");
            this.Visible = false;
        }

        public async void OnLoadingComplete()
        {
            if (AnimPlayer.IsPlaying())
                await ToSignal(AnimPlayer, "animation_finished");
            AnimPlayer.Play("ShowProceedButton");
        }

        void OnProceedButtonPressed()
        {
            HideLoading();
        }
    }
}