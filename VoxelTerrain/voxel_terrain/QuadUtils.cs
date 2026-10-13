using System;
using System.Collections.Generic;
using Godot;

public static class QuadUtils
{
    public const int UV00_IDX = 1;
    public const int UV10_IDX = 0;
    public const int UV01_IDX = 2;
    public const int UV11_IDX = 3;

    static readonly Vector3 p0 = new Vector3(0.0f, 0.0f, 0.0f);
    static readonly Vector3 p1 = new Vector3(1.0f, 0.0f, 0.0f);
    static readonly Vector3 p2 = new Vector3(1.0f, 0.0f, 1.0f);
    static readonly Vector3 p3 = new Vector3(0.0f, 0.0f, 1.0f);
    static readonly Vector3 p4 = new Vector3(0.0f, 1.0f, 0.0f);
    static readonly Vector3 p5 = new Vector3(1.0f, 1.0f, 0.0f);
    static readonly Vector3 p6 = new Vector3(1.0f, 1.0f, 1.0f);
    static readonly Vector3 p7 = new Vector3(0.0f, 1.0f, 1.0f);

    public static void AddQuadData_Threaded(
        Vector3[] verticies, Vector3[] normals,
        Vector2[] uvs, int[] triangles,
        int verticesOffset, int trianglesOffset,
        Vector3 positionOffset,
        MeshUtils.BlockSide side, BlockType blockType)
    {
        triangles[trianglesOffset + 0] = verticesOffset + 3;
        triangles[trianglesOffset + 1] = verticesOffset + 1;
        triangles[trianglesOffset + 2] = verticesOffset + 0;
        triangles[trianglesOffset + 3] = verticesOffset + 3;
        triangles[trianglesOffset + 4] = verticesOffset + 2;
        triangles[trianglesOffset + 5] = verticesOffset + 1;

        var uvs_data = blockType.GetUVCoords(side);
        Vector2 uv00 = uvs_data[UV00_IDX];
        Vector2 uv10 = uvs_data[UV10_IDX];
        Vector2 uv01 = uvs_data[UV01_IDX];
        Vector2 uv11 = uvs_data[UV11_IDX];

        uvs[verticesOffset + 0] = uv10;
        uvs[verticesOffset + 1] = uv00;
        uvs[verticesOffset + 2] = uv01;
        uvs[verticesOffset + 3] = uv11;

        switch (side)
        {
            case MeshUtils.BlockSide.Bottom:
                verticies[verticesOffset + 0] = p0 + positionOffset;
                verticies[verticesOffset + 1] = p1 + positionOffset;
                verticies[verticesOffset + 2] = p2 + positionOffset;
                verticies[verticesOffset + 3] = p3 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Down;
                normals[verticesOffset + 1] = Vector3.Down;
                normals[verticesOffset + 2] = Vector3.Down;
                normals[verticesOffset + 3] = Vector3.Down;
                
                break;
            case MeshUtils.BlockSide.Top:
                verticies[verticesOffset + 0] = p7 + positionOffset;
                verticies[verticesOffset + 1] = p6 + positionOffset;
                verticies[verticesOffset + 2] = p5 + positionOffset;
                verticies[verticesOffset + 3] = p4 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Up;
                normals[verticesOffset + 1] = Vector3.Up;
                normals[verticesOffset + 2] = Vector3.Up;
                normals[verticesOffset + 3] = Vector3.Up;

                break;
            case MeshUtils.BlockSide.Left:
                verticies[verticesOffset + 0] = p7 + positionOffset;
                verticies[verticesOffset + 1] = p4 + positionOffset;
                verticies[verticesOffset + 2] = p0 + positionOffset;
                verticies[verticesOffset + 3] = p3 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Left;
                normals[verticesOffset + 1] = Vector3.Left;
                normals[verticesOffset + 2] = Vector3.Left;
                normals[verticesOffset + 3] = Vector3.Left;

                break;
            case MeshUtils.BlockSide.Right:
                verticies[verticesOffset + 0] = p5 + positionOffset;
                verticies[verticesOffset + 1] = p6 + positionOffset;
                verticies[verticesOffset + 2] = p2 + positionOffset;
                verticies[verticesOffset + 3] = p1 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Right;
                normals[verticesOffset + 1] = Vector3.Right;
                normals[verticesOffset + 2] = Vector3.Right;
                normals[verticesOffset + 3] = Vector3.Right;

                break;
            case MeshUtils.BlockSide.Front:
                verticies[verticesOffset + 0] = p4 + positionOffset;
                verticies[verticesOffset + 1] = p5 + positionOffset;
                verticies[verticesOffset + 2] = p1 + positionOffset;
                verticies[verticesOffset + 3] = p0 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Forward;
                normals[verticesOffset + 1] = Vector3.Forward;
                normals[verticesOffset + 2] = Vector3.Forward;
                normals[verticesOffset + 3] = Vector3.Forward;

                break;
            case MeshUtils.BlockSide.Back:
                verticies[verticesOffset + 0] = p6 + positionOffset;
                verticies[verticesOffset + 1] = p7 + positionOffset;
                verticies[verticesOffset + 2] = p3 + positionOffset;
                verticies[verticesOffset + 3] = p2 + positionOffset;

                normals[verticesOffset + 0] = Vector3.Back;
                normals[verticesOffset + 1] = Vector3.Back;
                normals[verticesOffset + 2] = Vector3.Back;
                normals[verticesOffset + 3] = Vector3.Back;
                
                break;
        }
    }

    public static void AddQuadData(
        List<Vector3> verticies, List<Vector3> normals,
        List<Vector2> uvs, List<int> triangles,
        Vector3 positionOffset, int indexOffset,
        MeshUtils.BlockSide side, BlockType blockType)
    {
        triangles.Add(indexOffset + 3);
        triangles.Add(indexOffset + 1);
        triangles.Add(indexOffset + 0);
        triangles.Add(indexOffset + 3);
        triangles.Add(indexOffset + 2);
        triangles.Add(indexOffset + 1);

        var uvs_data = blockType.GetUVCoords(side);
        Vector2 uv00 = uvs_data[UV00_IDX];
        Vector2 uv10 = uvs_data[UV10_IDX];
        Vector2 uv01 = uvs_data[UV01_IDX];
        Vector2 uv11 = uvs_data[UV11_IDX];

        uvs.Add(uv10);
        uvs.Add(uv00);
        uvs.Add(uv01);
        uvs.Add(uv11);

        switch (side)
        {
            case MeshUtils.BlockSide.Bottom:
                verticies.Add(p0 + positionOffset);
                verticies.Add(p1 + positionOffset);
                verticies.Add(p2 + positionOffset);
                verticies.Add(p3 + positionOffset);

                normals.Add(Vector3.Down);
                normals.Add(Vector3.Down);
                normals.Add(Vector3.Down);
                normals.Add(Vector3.Down);
                
                break;
            case MeshUtils.BlockSide.Top:
                verticies.Add(p7 + positionOffset);
                verticies.Add(p6 + positionOffset);
                verticies.Add(p5 + positionOffset);
                verticies.Add(p4 + positionOffset);

                normals.Add(Vector3.Up);
                normals.Add(Vector3.Up);
                normals.Add(Vector3.Up);
                normals.Add(Vector3.Up);

                break;
            case MeshUtils.BlockSide.Left:
                verticies.Add(p7 + positionOffset);
                verticies.Add(p4 + positionOffset);
                verticies.Add(p0 + positionOffset);
                verticies.Add(p3 + positionOffset);

                normals.Add(Vector3.Left);
                normals.Add(Vector3.Left);
                normals.Add(Vector3.Left);
                normals.Add(Vector3.Left);

                break;
            case MeshUtils.BlockSide.Right:
                verticies.Add(p5 + positionOffset);
                verticies.Add(p6 + positionOffset);
                verticies.Add(p2 + positionOffset);
                verticies.Add(p1 + positionOffset);

                normals.Add(Vector3.Right);
                normals.Add(Vector3.Right);
                normals.Add(Vector3.Right);
                normals.Add(Vector3.Right);

                break;
            case MeshUtils.BlockSide.Front:
                verticies.Add(p4 + positionOffset);
                verticies.Add(p5 + positionOffset);
                verticies.Add(p1 + positionOffset);
                verticies.Add(p0 + positionOffset);

                normals.Add(Vector3.Forward);
                normals.Add(Vector3.Forward);
                normals.Add(Vector3.Forward);
                normals.Add(Vector3.Forward);

                break;
            case MeshUtils.BlockSide.Back:
                verticies.Add(p6 + positionOffset);
                verticies.Add(p7 + positionOffset);
                verticies.Add(p3 + positionOffset);
                verticies.Add(p2 + positionOffset);

                normals.Add(Vector3.Back);
                normals.Add(Vector3.Back);
                normals.Add(Vector3.Back);
                normals.Add(Vector3.Back);
                
                break;
        }
    }
}