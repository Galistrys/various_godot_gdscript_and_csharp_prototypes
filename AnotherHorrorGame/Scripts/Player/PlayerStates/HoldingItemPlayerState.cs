using Godot;
using HorrorGame.Entities.Interactables;
using HorrorGame.Managers;
using System;

namespace HorrorGame.Player.PlayerStates
{
	public class HoldingItemPlayerState : WalkingPlayerState
	{
		string _itemId;

		public HoldingItemPlayerState(string itemId)
		{
			_itemId = itemId;
		}

		public override void OnEnter()
        {
            base.OnEnter();

            PlayerController.ColShape.Disabled = false;
            PlayerController.CrouchColShape.Disabled = true;

			PlayerController.GameManager.HUDController.Pointer.ShowItem(_itemId);
        }

        public override void OnExit()
        {
            base.OnExit();

			PlayerController.GameManager.HUDController.Pointer.ShowItem(null);
        }

		protected override void InteractWithInteractable(IInteractable interactable)
		{
			if (interactable.UseItem(_itemId))
			{
				GameSaveManager.CurrentGameSave.RemoveItem(_itemId);
				PlayerController.PopState();
				Logger.Log($"Item {_itemId} used successfully.");
			}
			else
			{
				PlayerController.PopState();
				Logger.Log($"Item {_itemId} used unsuccessfully.");
			}
		}
	}
}