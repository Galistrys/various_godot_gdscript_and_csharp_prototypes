using Godot;
using System;
using System.Collections.Generic;

public class BlockData
{
    public enum TextureMode
    {
        OneForAll,
        EveryDifferent
    }

    public readonly bool transparent;
    public readonly TextureMode textureMode;
    public readonly Color color;

    public BlockData(bool transparent, TextureMode mode, Color color)
    {
        this.transparent = transparent;
        this.textureMode = mode;
        this.color = color;
    }
}
