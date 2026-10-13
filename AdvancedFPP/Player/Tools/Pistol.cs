using Godot;
using System;

namespace SpaceAdventure.Player.Tools
{
    public class Pistol : Spatial
    {
        [Signal] public delegate void AnimationFinished();

        AnimationPlayer _animPlayer;
        AudioStreamPlayer _soundPlayer;
        Random _random;

        public Pistol()
        {
            _random = new Random();
        }

        public override void _Ready()
        {
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _soundPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        }

        public void Shoot()
        {
            if (_animPlayer.IsPlaying())
            {

            }
            else
            {
                _soundPlayer.PitchScale = Mathf.RangeLerp(GD.Randf(), 0, 1, 0.7f, 0.9f);
                _animPlayer.Play("Kick");
            }
        }

        public void ShowPistol()
        {
            _animPlayer.Play("Show");
            Visible = true;

        }

        public void HidePistol()
        {
            _animPlayer.Play("Hide");
        }

        void AnimFinished(string animName)
        {
            EmitSignal(nameof(AnimationFinished));
        }
    }
}