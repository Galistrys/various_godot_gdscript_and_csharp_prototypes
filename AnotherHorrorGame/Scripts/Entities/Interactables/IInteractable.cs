using Godot;
using HorrorGame.GUI.HUD;
using HorrorGame.Player;
using HorrorGame.Player.PlayerStates;
using System;

namespace HorrorGame.Entities.Interactables
{
    public interface IInteractable : IEntity
    {
        string InteractableId { get; }
        PointerType OnRaycastDetectionPointer { get; }

        void OnInteraction(PlayerController player);
        bool UseItem(string itemId);
    }
}