using Godot;
using System;


public class BlockType
{
    public readonly string blockName;
    public readonly bool transparent;

    readonly BlockTexturingMethod _blockTexturingMethod;

    public BlockType(string blockName, bool transparent, BlockTexturingMethod blockTexturingMethod)
    {
        this.blockName = blockName;
        this.transparent = transparent;

        _blockTexturingMethod = blockTexturingMethod;
    }

    public Vector2[] GetUVCoords(MeshUtils.BlockSide side)
    {
        return _blockTexturingMethod.GetUVCoords(side);
    }
}

public abstract class BlockTexturingMethod
{
    public abstract Vector2[] GetUVCoords(MeshUtils.BlockSide side);
}

public class AllSameTexturing : BlockTexturingMethod
{
    Vector2[] _uvs;

    public AllSameTexturing(Vector2[] uvs)
    {
        _uvs = uvs;
    }

    public override Vector2[] GetUVCoords(MeshUtils.BlockSide side)
    {
        return _uvs;
    }
}

public class SameSidesTexturing : BlockTexturingMethod
{
    Vector2[] _sideUvs;
    Vector2[] _tbUvs;

    public SameSidesTexturing(Vector2[] sideUvs, Vector2[] tbUvs)
    {
        _sideUvs = sideUvs;
        _tbUvs = tbUvs;
    }

    public override Vector2[] GetUVCoords(MeshUtils.BlockSide side)
    {
        if (side == MeshUtils.BlockSide.Top || side == MeshUtils.BlockSide.Bottom)
            return _tbUvs;
        else 
            return _sideUvs;
    }
}

public class SameSidesDiffTBTexturing : BlockTexturingMethod
{
    Vector2[] _sideUvs;
    Vector2[] _topUvs;
    Vector2[] _bottomUvs;

    public SameSidesDiffTBTexturing(Vector2[] sideUvs, Vector2[] topUvs, Vector2[] bottomUvs)
    {
        _sideUvs = sideUvs;
        _topUvs = topUvs;
        _bottomUvs = bottomUvs;
    }

    public override Vector2[] GetUVCoords(MeshUtils.BlockSide side)
    {
        if (side == MeshUtils.BlockSide.Top)
            return _topUvs;
        else if (side == MeshUtils.BlockSide.Bottom)
            return _bottomUvs;
        else 
            return _sideUvs;
    }
}

public class AllDifferentTexturing : BlockTexturingMethod
{
    Vector2[] _topUvs;
    Vector2[] _bottomUvs;
    Vector2[] _leftUvs;
    Vector2[] _rightUvs;
    Vector2[] _frontUvs;
    Vector2[] _backUvs;

    public AllDifferentTexturing(
        Vector2[] topUvs, Vector2[] bottomUvs,
        Vector2[] leftUvs, Vector2[] rightUvs,
        Vector2[] frontUvs, Vector2[] backUvs)
    {
        _topUvs = topUvs;
        _bottomUvs = bottomUvs;
        _leftUvs = leftUvs;
        _rightUvs = rightUvs;
        _frontUvs = frontUvs;
        _backUvs = backUvs;
    }

    public override Vector2[] GetUVCoords(MeshUtils.BlockSide side)
    {
        if (side == MeshUtils.BlockSide.Top)
            return _topUvs;
        else if (side == MeshUtils.BlockSide.Bottom)
            return _bottomUvs;
        else if (side == MeshUtils.BlockSide.Left)
            return _leftUvs;
        else if (side == MeshUtils.BlockSide.Right)
            return _rightUvs;
        else if (side == MeshUtils.BlockSide.Front)
            return _frontUvs;
        else 
            return _backUvs;
    }
}