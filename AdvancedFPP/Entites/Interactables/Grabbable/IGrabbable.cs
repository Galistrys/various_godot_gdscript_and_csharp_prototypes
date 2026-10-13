using Godot;
using System;

namespace SpaceAdventure.Entites.Interactables.Grabbable
{
    public interface IGrabbable : IInteractable
    {
        Vector3 GetGlobalPosition();
    }
}