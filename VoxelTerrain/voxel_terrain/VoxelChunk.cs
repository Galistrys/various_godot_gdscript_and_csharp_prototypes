using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class VoxelChunk
{
    public const int CHUNK_SIZE = 16;

    public const int WIDTH = CHUNK_SIZE;  // X
    public const int HEIGHT = CHUNK_SIZE; // Y
    public const int DEPTH = CHUNK_SIZE;  // Z

    public const int MAX_VERTICES_COUNT = 4 * 6 * WIDTH * HEIGHT * DEPTH;
    public const int MAX_INDICES_COUNT = 6 * 6 * WIDTH * HEIGHT * DEPTH;
    public const int MAX_BLOCKS_COUNT = WIDTH * HEIGHT * DEPTH;

    public static Material chunkMaterial;

    Block[] _blocks;
    Transform3D _worldTransform;

    Rid _instance = new Rid();
    Rid _body = new Rid();
    
    ArrayMesh _model;
    Shape3D _shape;

    bool _hasShape = false;
    

    public VoxelChunk(Vector3 worldPosition)
    {
        _worldTransform = new Transform3D(Basis.Identity, worldPosition);

        _blocks = new Block[MAX_BLOCKS_COUNT];
    }

    public void CreateInstance(Rid scenario)
    {
        _instance = RenderingServer.InstanceCreate();
        RenderingServer.InstanceSetScenario(_instance, scenario);
        RenderingServer.InstanceSetTransform(_instance, _worldTransform);
    }

    public void CreateBody(Rid space)
    {
        _body = PhysicsServer3D.BodyCreate();
        PhysicsServer3D.BodySetSpace(_body, space);
        PhysicsServer3D.BodySetMode(_body, PhysicsServer3D.BodyMode.Static);
    }

    public void RegenerateMesh()
    {
        GenerateMesh();
        
        if (_model == null)
        {
            RenderingServer.InstanceSetBase(_instance, new Rid());
            PhysicsServer3D.BodyClearShapes(_body);
        }
        else
        {
            RenderingServer.InstanceSetBase(_instance, _model.GetRid());
            GenerateShape();
            PhysicsServer3D.BodyAddShape(_body, _shape.GetRid(), _worldTransform);
        }
    }

    public void Destroy()
    {
        RenderingServer.FreeRid(_instance);
        PhysicsServer3D.FreeRid(_body);

        _instance = new Rid();
        _body = new Rid();

        _model = null;
        _shape = null;

        _blocks = null;
    }

    public void GenerateData()
    {
        BlockType grass_block_type = BlocksDatabase.Instance.GetBlockType("grass");
        BlockType dirt_block_type = BlocksDatabase.Instance.GetBlockType("dirt");

        for (int x = 0; x < WIDTH; x++)
        {
            for (int z = 0; z < DEPTH; z++)
            {
                int surface_height = Mathf.RoundToInt(MeshUtils.GetNoise(x + _worldTransform.Origin.X, z + _worldTransform.Origin.Z, 8, 0.001f, 8, 32) - _worldTransform.Origin.Y);

                for (int y = 0; y < HEIGHT; y++)
                {
                    int index = y * WIDTH * DEPTH + z * WIDTH + x;
                    if (surface_height == y)
                        _blocks[index] = new Block(new Vector3(x, y, z), grass_block_type);
                    else if (y < surface_height)
                        _blocks[index] = new Block(new Vector3(x, y, z), dirt_block_type);
                    else
                        _blocks[index] = null;
                }
            }
        }
    }

    void GenerateMesh()
    {
        Vector3[] vertices_array = new Vector3[MAX_VERTICES_COUNT];
        Vector3[] normals_array = new Vector3[MAX_VERTICES_COUNT];
        Vector2[] uvs_array = new Vector2[MAX_VERTICES_COUNT];
        int[] triangles_array = new int[MAX_INDICES_COUNT];
        int[] neighbors_array = new int[MAX_BLOCKS_COUNT];

        for (int i = 0; i < WIDTH * HEIGHT * DEPTH; i++)
        {
            var block = _blocks[i];
            if (block == null)
                continue;
            
            int x = i % WIDTH;
            int y = i / (WIDTH * DEPTH);
            int z = (i / WIDTH) % DEPTH;

            int neighbors_data = 0;

            if (IsBlockTransparent(x, y - 1, z))
                neighbors_data |= Block.BOTTOM_SIDE;
            if (IsBlockTransparent(x, y + 1, z))
                neighbors_data |= Block.TOP_SIDE;
            if (IsBlockTransparent(x - 1, y, z))
                neighbors_data |= Block.LEFT_SIDE;
            if (IsBlockTransparent(x + 1, y, z))
                neighbors_data |= Block.RIGHT_SIDE;
            if (IsBlockTransparent(x, y, z - 1))
                neighbors_data |= Block.FRONT_SIDE;
            if (IsBlockTransparent(x, y, z + 1))
                neighbors_data |= Block.BACK_SIDE;

            neighbors_array[i] = neighbors_data;
        }

        var result = Parallel.For(0, MAX_BLOCKS_COUNT, (i) => GenerateMeshThread(i, vertices_array, normals_array, uvs_array, triangles_array, neighbors_array[i]));

        int quads_count = 0;
        for (int i = 0; i < MAX_BLOCKS_COUNT * 6; i++)
        {
            if (triangles_array[i * 6] != -1)
                quads_count += 1;
        }

        if (quads_count == 0)
        {
            _model = null;
            return;
        }

        Vector3[] n_ver_array = new Vector3[quads_count * 4];
        Vector3[] n_nor_array = new Vector3[quads_count * 4];
        Vector2[] n_uvs_array = new Vector2[quads_count * 4];
        int[] n_tri_array = new int[quads_count * 6];

        int repair_offset = 0;
        int n_vert_array_offset = 0;
        int n_tri_array_offset = 0;
        for (int i = 0; i < MAX_BLOCKS_COUNT * 6; i++)
        {
            if (triangles_array[i * 6] != -1)
            {
                n_ver_array[n_vert_array_offset + 0] = vertices_array[i * 4 + 0];
                n_ver_array[n_vert_array_offset + 1] = vertices_array[i * 4 + 1];
                n_ver_array[n_vert_array_offset + 2] = vertices_array[i * 4 + 2];
                n_ver_array[n_vert_array_offset + 3] = vertices_array[i * 4 + 3];

                n_nor_array[n_vert_array_offset + 0] = normals_array[i * 4 + 0];
                n_nor_array[n_vert_array_offset + 1] = normals_array[i * 4 + 1];
                n_nor_array[n_vert_array_offset + 2] = normals_array[i * 4 + 2];
                n_nor_array[n_vert_array_offset + 3] = normals_array[i * 4 + 3];

                n_uvs_array[n_vert_array_offset + 0] = uvs_array[i * 4 + 0];
                n_uvs_array[n_vert_array_offset + 1] = uvs_array[i * 4 + 1];
                n_uvs_array[n_vert_array_offset + 2] = uvs_array[i * 4 + 2];
                n_uvs_array[n_vert_array_offset + 3] = uvs_array[i * 4 + 3];

                n_tri_array[n_tri_array_offset + 0] = triangles_array[i * 6 + 0] - repair_offset;
                n_tri_array[n_tri_array_offset + 1] = triangles_array[i * 6 + 1] - repair_offset;
                n_tri_array[n_tri_array_offset + 2] = triangles_array[i * 6 + 2] - repair_offset;
                n_tri_array[n_tri_array_offset + 3] = triangles_array[i * 6 + 3] - repair_offset;
                n_tri_array[n_tri_array_offset + 4] = triangles_array[i * 6 + 4] - repair_offset;
                n_tri_array[n_tri_array_offset + 5] = triangles_array[i * 6 + 5] - repair_offset;

                n_vert_array_offset += 4;
                n_tri_array_offset += 6;
            }
            else
            {
                repair_offset += 4;
            }
        }

        _model = new ArrayMesh();

        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)Mesh.ArrayType.Max);

        arrays[(int)Mesh.ArrayType.Vertex] = n_ver_array;
        arrays[(int)Mesh.ArrayType.Normal] = n_nor_array;
        arrays[(int)Mesh.ArrayType.TexUV] = n_uvs_array;
        arrays[(int)Mesh.ArrayType.Index] = n_tri_array;

        _model.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        _model.SurfaceSetMaterial(0, chunkMaterial);
    }

    void GenerateMeshThread(
        int index,
        Vector3[] verticesArray,
        Vector3[] normalsArray,
        Vector2[] uvsArray,
        int[] trianglesArray,
        int neighborsData)
    {
        trianglesArray[index * 6 * 6] = -1;
        trianglesArray[index * 6 * 6 + 6] = -1;
        trianglesArray[index * 6 * 6 + 12] = -1;
        trianglesArray[index * 6 * 6 + 18] = -1;
        trianglesArray[index * 6 * 6 + 24] = -1;
        trianglesArray[index * 6 * 6 + 30] = -1;

        if (_blocks[index] == null)
            return;

        _blocks[index].AddMeshData_Thread(
            verticesArray, normalsArray, uvsArray, trianglesArray,
            index * 6 * 4, index * 6 * 6, neighborsData
        );
    }

    void GenerateShape()
    {
        _shape = _model.CreateTrimeshShape();
    }

    bool IsBlockTransparent(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x > WIDTH - 1 || y > HEIGHT - 1 || z > DEPTH - 1)
            return true;
        
        var block = _blocks[y * WIDTH * DEPTH + z * WIDTH + x];
        if (block == null)
            return true;

        return block.blockType.transparent;
    }
}
