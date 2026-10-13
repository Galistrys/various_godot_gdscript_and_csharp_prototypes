using Godot;
using System;

public enum PhysicsLayer : uint
{
    None = 0b_0000_0000,
    Terrain = 0b_0000_0001,
    Player = 0b_0000_0010,
    Enemy = 0b_0000_0100,
    ItemEntity = 0b_0000_1000,
    MovableEntity = 0b_0001_0000,
    GrabbableEntity = 0b_0010_0000,
    StateEntity = 0b_0100_0000,
    Door = 0b_1000_0000
}