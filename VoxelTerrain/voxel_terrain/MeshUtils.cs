using Godot;
using System;
using System.Collections.Generic;
using VertexData = System.Tuple<Godot.Vector3, Godot.Vector3, Godot.Vector2>;

public static class MeshUtils
{
    public enum BlockSide { Bottom, Top, Left, Right, Front, Back }

    public static FastNoiseLite _noise;

    public static float GetNoise(float x, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += _noise.GetNoise2D(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2;
        }
        return total + heightOffset;
    }

    public static T[] MergeArray<T>(T[] array1, T[] array2)
    {
        var original_first_array_length = array1.Length;
        T[] first_temp_array = Array.Empty<T>();

        Array.Resize(ref first_temp_array, array1.Length + array2.Length);
        Array.Copy(array2, 0, first_temp_array, original_first_array_length, array2.Length);

        return first_temp_array;
    }

    // public static ArrayMesh MergeMeshes(ArrayMesh[] meshes)
    // {
    //     Dictionary<VertexData, int> points_order = new Dictionary<VertexData, int>();
    //     HashSet<VertexData> points_hash = new HashSet<VertexData>();
    //     List<int> tris = new List<int>();

    //     int p_index = 0;
    //     for (int i = 0; i < meshes.Length; i++)
    //     {
    //         if (meshes[i] == null) continue;
    //         var surface_arrays = meshes[i].SurfaceGetArrays(0);
    //         Vector3[] vertex_array = (Vector3[])surface_arrays[(int)Mesh.ArrayType.Vertex];
    //         Vector3[] normal_array = (Vector3[])surface_arrays[(int)Mesh.ArrayType.Normal];
    //         Vector2[] uv_array = (Vector2[])surface_arrays[(int)Mesh.ArrayType.TexUV];
    //         int[] index_array = (int[])surface_arrays[(int)Mesh.ArrayType.Index];

    //         for (int j = 0; j < vertex_array.Length; j++)
    //         {
    //             Vector3 v = vertex_array[j];
    //             Vector3 n = normal_array[j];
    //             Vector2 u = uv_array[j];

    //             VertexData p = new VertexData(v, n, u);
    //             if (!points_hash.Contains(p))
    //             {
    //                 points_order.Add(p, p_index);
    //                 points_hash.Add(p);
    //                 p_index++;
    //             }
    //         }

    //         for (int t = 0; t < index_array.Length; t++)
    //         {
    //             int tri_point = index_array[t];

    //             Vector3 v = vertex_array[tri_point];
    //             Vector3 n = normal_array[tri_point];
    //             Vector2 u = uv_array[tri_point];

    //             VertexData p = new VertexData(v, n, u);

    //             int index;
    //             points_order.TryGetValue(p, out index);
    //             tris.Add(index);
    //         }

    //         meshes[i] = null;
    //     }

    //     var mesh = new ArrayMesh();
    //     ExtractArrays(points_order, tris, mesh);
    //     return mesh;
    // }

    // public static void ExtractArrays(Dictionary<VertexData, int> list, List<int> indicies, ArrayMesh mesh)
    // {
    //     List<Vector3> verts = new List<Vector3>();
    //     List<Vector3> norms = new List<Vector3>();
    //     List<Vector2> uvs = new List<Vector2>();

    //     foreach (VertexData v in list.Keys)
    //     {
    //         verts.Add(v.Item1);
    //         norms.Add(v.Item2);
    //         uvs.Add(v.Item3);
    //     }

    //     var arrays = new Godot.Collections.Array();
    //     arrays.Resize((int)Mesh.ArrayType.Max);
    //     arrays[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
    //     arrays[(int)Mesh.ArrayType.Normal] = norms.ToArray();
    //     arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
    //     arrays[(int)Mesh.ArrayType.Index] = indicies.ToArray();

    //     mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
    // }
}