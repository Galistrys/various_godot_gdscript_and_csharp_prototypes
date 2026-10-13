using Godot;
using HorrorGame.GUI.HUD;
using HorrorGame.Player;
using System;

namespace HorrorGame.Entities.Interactables
{
    public interface IInteractable : IEntity
    {
        string InteractableId { get; }
        PointerType OnRaycastDetectionPointer { get; }

        void OnInteraction(PlayerEntity player);
        bool UseItem(string itemId);
    }
}