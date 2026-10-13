using Godot;
using HorrorGame.Managers;
using System;
using System.Collections.Generic;

namespace HorrorGame.GUI.HUD.Inventory
{
    public class InventoryController : Control
    {
        [Signal]
        public delegate void BackgroundVisible();
        [Signal]
        public delegate void ContentHidden();
        [Signal]
        public delegate void AnimationFinished();

        Panel Background { get { return GetNode<Panel>("Background"); } }
        AnimationPlayer AnimationPlayer { get { return GetNode<AnimationPlayer>("AnimationPlayer"); } }

        public Equipment Equipment { get { return GetNode<Equipment>("Equipment"); } }

        public bool Busy { get { return AnimationPlayer.IsPlaying(); } }

        public InventoryController()
        {
            
        }

        public override void _Ready()
        {
            
        }

        public override void _ExitTree()
        {
            
        }

        public void OnAnimationFinished(string name)
        {
            EmitSignal(nameof(AnimationFinished));
        }

        public async void ShowInventory(View view, object[] data)
        {
            if (AnimationPlayer.IsPlaying())
                await ToSignal(AnimationPlayer, "animation_finished");

            Visible = true;

            AnimationPlayer.Play("FadeIn");
            await ToSignal(this, nameof(BackgroundVisible));
            Logger.Log("Background visible");
            Input.SetMouseMode(Input.MouseMode.Visible);

            Equipment.OpenView(view, data);
        }

        public async void HideInventory()
        {
            if (AnimationPlayer.IsPlaying())
                await ToSignal(AnimationPlayer, "animation_finished");

            Input.SetMouseMode(Input.MouseMode.Captured);
            
            AnimationPlayer.Play("FadeOut");
            await ToSignal(this, nameof(ContentHidden));
            Equipment.CloseView();
            await ToSignal(this, nameof(AnimationFinished));
            Visible = false;
        }

        public override void _Process(float delta)
        {

        }
    }
}