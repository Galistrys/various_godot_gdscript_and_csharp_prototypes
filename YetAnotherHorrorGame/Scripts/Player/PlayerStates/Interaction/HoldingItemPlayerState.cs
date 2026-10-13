using Godot;
using HorrorGame.Entities.Interactables;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Player.PlayerStates.Interaction
{
    public class HoldingItemPlayerState : IdlePlayerState
    {
        string _itemId;

        public HoldingItemPlayerState(PlayerEntity player, string itemId) : base(player)
        {
            _itemId = itemId;
        }

        public override void OnEnter()
        {
            MainManager.Instance.GameManager.HUDController.Pointer.ShowItem(_itemId);
        }

        public override void OnExit()
        {
            MainManager.Instance.GameManager.HUDController.Pointer.ShowItem(null);
        }

        public override void UpdateProcess(float delta)
        {
            base.UpdateProcess(delta);

            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Logger.Log($"Item {_itemId} use aborted.");
                Hide();
            }
        }

        protected override void Interact(IInteractable interactable)
        {
            if (interactable.UseItem(_itemId))
            {
                GameSaveManager.CurrentGameSave.RemoveItem(_itemId);
                Logger.Log($"Item {_itemId} used successfully.");
            }
            else
            {
                Logger.Log($"Item {_itemId} used unsuccessfully.");
            }

            Hide();
        }

        void Hide()
        {
            _player.ChangeInteractionState(new IdlePlayerState(_player));
        }
    }
}