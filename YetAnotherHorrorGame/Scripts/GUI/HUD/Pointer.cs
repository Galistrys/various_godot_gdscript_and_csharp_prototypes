using Godot;
using HorrorGame.Data.Items;
using System;

namespace HorrorGame.GUI.HUD
{
    public enum PointerType
    {
        Default,
        Pick,
        Grab,
        Push,
        Door
    }

    public class Pointer : CenterContainer
    {
        public const string POINTERS_DIR = "res://Assets/Pointers";
        public const string POINTERS_EXT = "Pointer.png";

        TextureRect _pointerTextureRect;
        TextureRect _itemTextureRect;
        Label _messageLabel;

        AnimationPlayer _animPlayer;

        public override void _Ready()
        {
            _pointerTextureRect = GetNode<TextureRect>("PointerTexture");
            _itemTextureRect = GetNode<TextureRect>("ItemTexture");
            _messageLabel = GetNode<Label>("MessageLabel");
            _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            ShowPointer(PointerType.Default);
        }

        public override void _ExitTree()
        {
            _pointerTextureRect = null;
            _messageLabel = null;
            _animPlayer = null;
        }

        public void ShowPointer(PointerType type)
        {
            string path = POINTERS_DIR + "/" + type.ToString() + POINTERS_EXT;
            
            if (ResourceLoader.Exists(path))
            {
                Logger.Log($"Changing pointer to {type.ToString()}");
                _pointerTextureRect.Texture = ResourceLoader.Load<Texture>(path);
            }
            else
            {
                Logger.Log($"Pointer of type {type.ToString()} not found: {path}", LogLevel.Error);
            }
        }

        public void ShowItem(string itemId)
        {
            if (itemId == null)
            {
                _itemTextureRect.Texture = null;
                _pointerTextureRect.Visible = true;
            }
            else
            {
                _pointerTextureRect.Visible = false;
                _itemTextureRect.Texture = ItemData.GetItemTexture(itemId);
            }
        }

        public void ShowMessage(string message)
        {
            if (_animPlayer.PlaybackActive)
                _animPlayer.Stop();
            _messageLabel.Modulate = new Color(1, 1, 1, 0);

            _messageLabel.Text = message;
            _animPlayer.Play("ShowMessage");
        }
    }
}