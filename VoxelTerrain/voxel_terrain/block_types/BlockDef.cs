using Godot;
using System;

public partial class BlockDef : Resource
{
    public enum TexturingMode
    {
        AllSame, SameSides, SameSidesDiffTB, AllDifferent
    }

    [Export] public TexturingMode texturingMode = TexturingMode.AllSame;

    [ExportGroup("Same Textures")]
    [Export] public Texture2D allSameTexture;

    [ExportGroup("Same Sides Textures")]
    [Export] public Texture2D sameSidesSideTexture;
    [Export] public Texture2D sameSidesTopAndBottomTexture;
    
    [ExportGroup("Same Sides And Different T_B Textures")]
    [Export] public Texture2D sameSidesDiffTBSideTexture;
    [Export] public Texture2D sameSidesDiffTBTopTexture;
    [Export] public Texture2D sameSidesDiffTBBottomTexture;

    [ExportGroup("Different Textures")]
    [Export] public Texture2D diffBottomTexture;
    [Export] public Texture2D diffTopTexture;
    [Export] public Texture2D diffLeftTexture;
    [Export] public Texture2D diffRightTexture;
    [Export] public Texture2D diffFrontTexture;
    [Export] public Texture2D diffBackTexture;

    [ExportGroup("Properties")]
    [Export] public bool transparent;

    public BlockDef()
    {
        texturingMode = TexturingMode.AllSame;

        allSameTexture = null;

        sameSidesSideTexture = null;
        sameSidesTopAndBottomTexture = null;

        sameSidesDiffTBSideTexture = null;
        sameSidesDiffTBTopTexture = null;
        sameSidesDiffTBBottomTexture = null;

        diffBottomTexture = null;
        diffTopTexture = null;
        diffLeftTexture = null;
        diffRightTexture = null;
        diffFrontTexture = null;
        diffBackTexture = null;

        transparent = false;
    }

    public BlockDef(
        TexturingMode tm,
        Texture2D ast,
        Texture2D ssst, Texture2D sstbt,
        Texture2D ssdtbst, Texture2D ssdtbtt, Texture2D ssdtbbt,
        Texture2D dbot, Texture2D dtt, Texture2D dlt, Texture2D drt, Texture2D dft, Texture2D dbat,
        bool tr)
    {
        texturingMode = tm;

        allSameTexture = ast;

        sameSidesSideTexture = ssst;
        sameSidesTopAndBottomTexture = sstbt;

        sameSidesDiffTBSideTexture = ssdtbst;
        sameSidesDiffTBTopTexture = ssdtbtt;
        sameSidesDiffTBBottomTexture = ssdtbbt;

        diffBottomTexture = dbot;
        diffTopTexture = dtt;
        diffLeftTexture = dlt;
        diffRightTexture = drt;
        diffFrontTexture = dft;
        diffBackTexture = dbat;

        transparent = tr;
    }

    public Texture2D[] GetTextures()
    {
        if (texturingMode == TexturingMode.AllSame)
            return new Texture2D[] { allSameTexture };
        else if (texturingMode == TexturingMode.SameSides)
            return new Texture2D[] { sameSidesSideTexture, sameSidesTopAndBottomTexture };
        else if (texturingMode == TexturingMode.SameSidesDiffTB)
            return new Texture2D[] { sameSidesDiffTBTopTexture, sameSidesDiffTBBottomTexture, sameSidesDiffTBSideTexture };
        else
            return new Texture2D[] {
                diffTopTexture, diffBottomTexture,
                diffLeftTexture, diffRightTexture,
                diffFrontTexture, diffBackTexture
            };
    }
}
