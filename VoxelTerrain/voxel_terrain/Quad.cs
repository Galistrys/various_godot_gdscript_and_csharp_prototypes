using Godot;
using System.Collections;
using System.Collections.Generic;

public class Quad
{
    public const int TEXTURE_SIZE = 2048;
    public const int TILES_IN_ROW = 16;
    public const int TILE_SIZE = TEXTURE_SIZE / TILES_IN_ROW; 
    public const float TILE_UNIT = TILE_SIZE / (float)TEXTURE_SIZE;

    public const int UV00_IDX = 1;
    public const int UV10_IDX = 0;
    public const int UV01_IDX = 2;
    public const int UV11_IDX = 3;

    public ArrayMesh mesh;

    public Quad(MeshUtils.BlockSide side, Vector3 offset, BlockType blockType)
    {
        mesh = new ArrayMesh();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        var uvs_data = blockType.GetUVCoords(side);
        Vector2 uv00 = uvs_data[UV00_IDX];
        Vector2 uv10 = uvs_data[UV10_IDX];
        Vector2 uv01 = uvs_data[UV01_IDX];
        Vector2 uv11 = uvs_data[UV11_IDX];

        Vector3 p0 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 p1 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 p2 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 p3 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
        Vector3 p4 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;
        Vector3 p5 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 p6 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 p7 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;

        switch (side)
        {
            case MeshUtils.BlockSide.Bottom:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {
                    Vector3.Down, Vector3.Down, Vector3.Down, Vector3.Down
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
            case MeshUtils.BlockSide.Top:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {
                    Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
            case MeshUtils.BlockSide.Left:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {
                    Vector3.Left, Vector3.Left, Vector3.Left, Vector3.Left
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
            case MeshUtils.BlockSide.Right:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {
                    Vector3.Right, Vector3.Right, Vector3.Right, Vector3.Right
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
            case MeshUtils.BlockSide.Front:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {
                    Vector3.Forward, Vector3.Forward, Vector3.Forward, Vector3.Forward
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
            case MeshUtils.BlockSide.Back:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {
                    Vector3.Back, Vector3.Back, Vector3.Back, Vector3.Back
                };
                uvs = new Vector2[] { uv10, uv00, uv01, uv11 };

                break;
        }

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.Normal] = normals;
        arrays[(int)Mesh.ArrayType.TexUV] = uvs;
        arrays[(int)Mesh.ArrayType.Index] = triangles;
        
        mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    }
}
