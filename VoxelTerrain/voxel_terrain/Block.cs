using Godot;
using System;
using System.Collections.Generic;

public class Block
{
    public const string BLOCK_ID = "stone";

    public const int BOTTOM_SIDE = 1;
    public const int TOP_SIDE = 2;
    public const int LEFT_SIDE = 4;
    public const int RIGHT_SIDE = 8;
    public const int FRONT_SIDE = 16;
    public const int BACK_SIDE = 32;

    readonly Vector3 _offset;
    public readonly BlockType blockType;

    public Block(Vector3 offset, BlockType blockType)
    {
        _offset = offset;
        this.blockType = blockType;
    }

    public void AddMeshData_Thread(
        Vector3[] vertices, Vector3[] normals,
        Vector2[] uvs, int[] triangles,
        int verticesOffset, int triangleOffset,
        int neighborsData)
    {
        if ((neighborsData & BOTTOM_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset, triangleOffset, _offset, MeshUtils.BlockSide.Bottom, blockType);
        if ((neighborsData & TOP_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset + 4, triangleOffset + 6, _offset, MeshUtils.BlockSide.Top, blockType);
        if ((neighborsData & LEFT_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset + 8, triangleOffset + 12, _offset, MeshUtils.BlockSide.Left, blockType);
        if ((neighborsData & RIGHT_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset + 12, triangleOffset + 18, _offset, MeshUtils.BlockSide.Right, blockType);
        if ((neighborsData & FRONT_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset + 16, triangleOffset + 24, _offset, MeshUtils.BlockSide.Front, blockType);
        if ((neighborsData & BACK_SIDE) > 0)
            QuadUtils.AddQuadData_Threaded(vertices, normals, uvs, triangles, verticesOffset + 20, triangleOffset + 30, _offset, MeshUtils.BlockSide.Back, blockType);
    }

    public int AddMeshData(
        List<Vector3> verticies, List<Vector3> normals,
        List<Vector2> uvs, List<int> triangles,
        int indexOffset, int neighborsData)
    {
        if ((neighborsData & BOTTOM_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Bottom, blockType);
            indexOffset += 4;
        }
        if ((neighborsData & TOP_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Top, blockType);
            indexOffset += 4;
        }
        if ((neighborsData & LEFT_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Left, blockType);
            indexOffset += 4;
        }
        if ((neighborsData & RIGHT_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Right, blockType);
            indexOffset += 4;
        }
        if ((neighborsData & FRONT_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Front, blockType);
            indexOffset += 4;
        }
        if ((neighborsData & BACK_SIDE) > 0)
        {
            QuadUtils.AddQuadData(verticies, normals, uvs, triangles, _offset, indexOffset, MeshUtils.BlockSide.Back, blockType);
            indexOffset += 4;
        }

        return indexOffset;
    }
}
