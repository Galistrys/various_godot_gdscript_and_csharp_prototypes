using Godot;
using KayEth.Items;
using KayEth.Player;
using System;

namespace KayEth.Entities.Interactables
{
    public interface IInteractable : IEntity
    {
        void OnInteraction(PlayerController player);
        bool UseItem(Item item);
    }
}
