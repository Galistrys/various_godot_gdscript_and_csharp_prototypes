using Godot;
using Godot.Collections;
using SpaceAdventure.Player;
using SpaceAdventure.Player.PlayerStates.Interaction;
using System;

namespace SpaceAdventure.Entites.Interactables
{
    public interface IInteractable
    {
        string InteractableName { get; }

        InteractionPlayerState OnInteraction(PlayerController playerController);

        Dictionary<string, object> SaveToDictionary();
        bool LoadFromDictionary(Dictionary<string, object> data);
    }
}