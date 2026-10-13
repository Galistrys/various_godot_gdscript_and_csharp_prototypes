using Godot;
using System;

public enum PhysicsLayer : uint
{
    None = 0b_0000,
    Terrain = 0b_0001,
    Entity = 0b_0010,
    Item = 0b_0100,
    Interactable = 0b_1000,
    Player = 0b_0001_0000,
    NPC = 0b_0010_0000,
    Enemy = 0b_0100_0000,
    Water = 0b_1000_0000
}

public static class PhysicsLayerExtensions
{
    public static uint Value(this PhysicsLayer layer)
    {
        return (uint)layer;
    }
}