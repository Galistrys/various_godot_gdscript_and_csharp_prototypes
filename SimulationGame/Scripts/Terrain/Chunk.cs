using Godot;
using System;
using System.Collections.Generic;

public class Chunk
{
    public static RID Scenario;
    public static readonly SpatialMaterial material = ResourceLoader.Load<SpatialMaterial>("res://Resources/Materials/Voxels.material");

    Vector3Int position;

    RID _instanceRid = null;
    RID _renderMeshRid = null;

    ArrayMesh _renderMesh;

    public BlockId[] blocksIdArray = new BlockId[TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE];

    List<Vector3> _renderVertices;
    List<Vector3> _renderNormals;
    List<Color> _renderColors;
    List<int> _renderIndices;

    public Chunk(Vector3Int position)
    {
        this.position = position;

        // _model = new MeshInstance();
        // AddChild(_model);
        // _col = new CollisionShape();
        // AddChild(_col);
    }

    public void DestroyChunk()
    {
        if (_instanceRid == null || _renderMeshRid == null)
            return;

        VisualServer.FreeRid(_instanceRid);
        VisualServer.FreeRid(_renderMeshRid);
    }

    public void GenerateChunk()
    {
        for (int x = 0; x < TerrainManager.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < TerrainManager.CHUNK_SIZE; z++)
            {
                int worldX = position.x + x;
                int worldZ = position.z + z;

                int height = NoiseUtils.Instance.GenerateHeight(worldX, worldZ);
                for (int y = 0; y < TerrainManager.CHUNK_SIZE; y++)
                {
                    int worldY = position.y + y;

                    BlockId blockId = BlockId.Air;

                    if (worldY < 3)
                        blockId = BlockId.Bedrock;
                    else if (worldY < height - 3)
                        blockId = BlockId.Stone;
                    else if (worldY < height)
                        blockId = BlockId.Dirt;
                    else if (worldY == height)
                        blockId = BlockId.Grass;

                    blocksIdArray[x * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE + y * TerrainManager.CHUNK_SIZE + z] = blockId;
                }
            }
        }
    }

    public BlockId GetBlock(Vector3Int vec)
    {
        return blocksIdArray[vec.x * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE + vec.y * TerrainManager.CHUNK_SIZE + vec.z];
    }

    public bool IsBlockFaceVisible(Vector3Int pos, int a, bool back)
    {
        Vector3Int temp = pos;
        temp[a] += back ? -1 : 1;
        return CheckIfVoxelCauseRender(temp.x, temp.y, temp.z, pos.x, pos.y, pos.z);
    }

    public bool CompareStep(Vector3Int a, Vector3Int b, int d, bool back)
    {
        BlockId blockA = GetBlock(a);
        BlockId blockB = GetBlock(b);

        return blockA == blockB && blockB != BlockId.Air && IsBlockFaceVisible(b, d, back);
    }

    public Vector3 GetNormal(int d, bool back)
    {
        if (back)
        {
            if (d == 0)
            {
                return Vector3.Left;
            }
            else if (d == 1)
            {
                return Vector3.Down;
            }
            else
            {
                return Vector3.Forward;
            }
        }
        else
        {
            if (d == 0)
            {
                return Vector3.Right;
            }
            else if (d == 1)
            {
                return Vector3.Up;
            }
            else
            {
                return Vector3.Back;
            }
        }
    }

    public void BuildChunk(bool oldMethod)
    {
        if (oldMethod)
        {
            _renderVertices = new List<Vector3>();
            _renderNormals = new List<Vector3>();
            _renderColors = new List<Color>();
            _renderIndices = new List<int>();

            for (int x = 0; x < TerrainManager.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < TerrainManager.CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < TerrainManager.CHUNK_SIZE; z++)
                    {
                        MakeVoxel(x, y, z);
                    }
                }
            }
        }
        else
        {
            BuildChunk();
        }
    }

    public void BuildChunk()
    {
        _renderVertices = new List<Vector3>();
        _renderNormals = new List<Vector3>();
        _renderColors = new List<Color>();
        _renderIndices = new List<int>();

        bool[,] merged;

        Vector3Int a, b, q, m, n, o;
        BlockId startBlockId;
        int d, i, j;

        for (int s = 0; s < 6; s++)
        {
            bool back = s > 2;
            d = s % 3;
            i = (d + 1) % 3;
            j = (d + 2) % 3;

            a = new Vector3Int();
            b = new Vector3Int();

            for (a[d] = 0; a[d] < TerrainManager.CHUNK_SIZE; a[d]++)
            {
                merged = new bool[TerrainManager.CHUNK_SIZE, TerrainManager.CHUNK_SIZE];

                for (a[i] = 0; a[i] < TerrainManager.CHUNK_SIZE; a[i]++)
                {
                    for (a[j] = 0; a[j] < TerrainManager.CHUNK_SIZE; a[j]++)
                    {
                        startBlockId = GetBlock(a);

                        if (merged[a[i], a[j]] || startBlockId == BlockId.Air || !IsBlockFaceVisible(a, d, back))
                        {
                            continue;
                        }

                        q = new Vector3Int();

                        for (b = a, b[j]++; b[j] < TerrainManager.CHUNK_SIZE && CompareStep(a, b, d, back) && !merged[b[i], b[j]]; b[j]++)
                        {

                        }

                        q[j] = b[j] - a[j];

                        for (b = a, b[i]++; b[i] < TerrainManager.CHUNK_SIZE && CompareStep(a, b, d, back) && !merged[b[i], b[j]]; b[i]++) {
                            for (b[j] = a[j]; b[j] < TerrainManager.CHUNK_SIZE && CompareStep(a, b, d, back) && !merged[b[i], b[j]]; b[j]++) { }

                            if (b[j] - a[j] < q[j])
                                break;
                            else
                                b[j] = a[j];
                        }

                        q[i] = b[i] - a[i];


                        m = new Vector3Int();
                        m[i] = q[i];

                        n = new Vector3Int();
                        n[j] = q[j];

                        o = a;
                        o[d] += back ? 0 : 1;

                        _renderVertices.Add(o);
                        _renderVertices.Add(o + n);
                        _renderVertices.Add(o + m + n);
                        _renderVertices.Add(o + m);

                        int count = _renderVertices.Count;
                        if (back)
                        {
                            _renderIndices.Add(count - 1);
                            _renderIndices.Add(count - 2);
                            _renderIndices.Add(count - 3);
                            _renderIndices.Add(count - 1);
                            _renderIndices.Add(count - 3);
                            _renderIndices.Add(count - 4);
                        }
                        else
                        {
                            _renderIndices.Add(count - 4);
                            _renderIndices.Add(count - 3);
                            _renderIndices.Add(count - 1);
                            _renderIndices.Add(count - 3);
                            _renderIndices.Add(count - 2);
                            _renderIndices.Add(count - 1);
                        }

                        

                        BlockData data = SINGLETONS.Database.GetBlockData(startBlockId);
                        Color color = data.color;

                        _renderColors.Add(color);
                        _renderColors.Add(color);
                        _renderColors.Add(color);
                        _renderColors.Add(color);

                        Vector3 nor = GetNormal(d, back);
                        _renderNormals.Add(nor);
                        _renderNormals.Add(nor);
                        _renderNormals.Add(nor);
                        _renderNormals.Add(nor);

                        for (int f = 0; f < q[i]; f++)
                        {
                            for (int g = 0; g < q[j]; g++)
                            {
                                merged[a[i] + f, a[j] + g] = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void DrawChunk()
    {
        if (_renderVertices.Count == 0)
        {
            // _renderMesh = null;
            _renderMeshRid = null;
            _instanceRid = null;
            return;
        }

        Godot.Collections.Array renderArrays = new Godot.Collections.Array();
        renderArrays.Resize((int)Mesh.ArrayType.Max);

        renderArrays[(int)Mesh.ArrayType.Vertex] = _renderVertices.ToArray();
        renderArrays[(int)Mesh.ArrayType.Normal] = _renderNormals.ToArray();
        renderArrays[(int)Mesh.ArrayType.Color] = _renderColors.ToArray();
        renderArrays[(int)Mesh.ArrayType.Index] = _renderIndices.ToArray();

        _renderMeshRid = VisualServer.MeshCreate();
        VisualServer.MeshAddSurfaceFromArrays(_renderMeshRid, VisualServer.PrimitiveType.Triangles, renderArrays);
        VisualServer.MeshSurfaceSetMaterial(_renderMeshRid, 0, material.GetRid());

        _instanceRid = VisualServer.InstanceCreate2(_renderMeshRid, Scenario);
        Transform glt = Transform.Identity;
        glt.origin = position;
        VisualServer.InstanceSetTransform(_instanceRid, glt);
    }

    void MakeVoxel(int x, int y, int z)
    {
        BlockId currentId = blocksIdArray[x * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE + y * TerrainManager.CHUNK_SIZE + z];
        if (currentId == BlockId.Air)
            return;
        
        if (CheckIfVoxelCauseRender(x, y + 1, z, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Top, currentId);

        if (CheckIfVoxelCauseRender(x, y - 1, z, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Bottom, currentId);

        if (CheckIfVoxelCauseRender(x + 1, y, z, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Right, currentId);
        
        if (CheckIfVoxelCauseRender(x - 1, y, z, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Left, currentId);

        if (CheckIfVoxelCauseRender(x, y, z + 1, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Back, currentId);

        if (CheckIfVoxelCauseRender(x, y, z - 1, x, y, z))
            MakeVoxelFace(x, y, z, BlockFace.Front, currentId);
    }

    void MakeVoxelFace(int x, int y, int z, BlockFace face, BlockId blockId)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        BlockData data = SINGLETONS.Database.GetBlockData(blockId);

        Color color = data.color;

        switch (face)
        {
            case BlockFace.Top:
                MakeVoxelFaceTop(x, y, z);
                break;
            case BlockFace.Bottom:
                MakeVoxelFaceBottom(x, y, z);
                break;
            case BlockFace.Right:
                MakeVoxelFaceRight(x, y, z);
                break;
            case BlockFace.Left:
                MakeVoxelFaceLeft(x, y, z);
                break;
            case BlockFace.Back:
                MakeVoxelFaceBack(x, y, z);
                break;
            case BlockFace.Front:
                MakeVoxelFaceFront(x, y, z);
                break;
        }

        _renderColors.Add(color);
        _renderColors.Add(color);
        _renderColors.Add(color);
        _renderColors.Add(color);

        int count = _renderVertices.Count;
        _renderIndices.Add(count - 4);
        _renderIndices.Add(count - 3);
        _renderIndices.Add(count - 1);
        _renderIndices.Add(count - 3);
        _renderIndices.Add(count - 2);
        _renderIndices.Add(count - 1);
    }

    void MakeVoxelFaceTop(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x, y + 1, z));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _renderVertices.Add(new Vector3(x, y + 1, z + 1));

        _renderNormals.Add(Vector3.Up);
        _renderNormals.Add(Vector3.Up);
        _renderNormals.Add(Vector3.Up);
        _renderNormals.Add(Vector3.Up);
    }

    void MakeVoxelFaceBottom(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x, y, z + 1));
        _renderVertices.Add(new Vector3(x + 1, y, z + 1));
        _renderVertices.Add(new Vector3(x + 1, y, z));
        _renderVertices.Add(new Vector3(x, y, z));

        _renderNormals.Add(Vector3.Down);
        _renderNormals.Add(Vector3.Down);
        _renderNormals.Add(Vector3.Down);
        _renderNormals.Add(Vector3.Down);
    }

    void MakeVoxelFaceRight(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x + 1, y, z));
        _renderVertices.Add(new Vector3(x + 1, y, z + 1));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z + 1));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z));

        _renderNormals.Add(Vector3.Right);
        _renderNormals.Add(Vector3.Right);
        _renderNormals.Add(Vector3.Right);
        _renderNormals.Add(Vector3.Right);
    }

    void MakeVoxelFaceLeft(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x, y, z + 1));
        _renderVertices.Add(new Vector3(x, y, z));
        _renderVertices.Add(new Vector3(x, y + 1, z));
        _renderVertices.Add(new Vector3(x, y + 1, z + 1));

        _renderNormals.Add(Vector3.Left);
        _renderNormals.Add(Vector3.Left);
        _renderNormals.Add(Vector3.Left);
        _renderNormals.Add(Vector3.Left);
    }

    void MakeVoxelFaceBack(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x + 1, y, z + 1));
        _renderVertices.Add(new Vector3(x, y, z + 1));
        _renderVertices.Add(new Vector3(x, y + 1, z + 1));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z + 1));

        _renderNormals.Add(Vector3.Back);
        _renderNormals.Add(Vector3.Back);
        _renderNormals.Add(Vector3.Back);
        _renderNormals.Add(Vector3.Back);
    }

    void MakeVoxelFaceFront(int x, int y, int z)
    {
        _renderVertices.Add(new Vector3(x, y, z));
        _renderVertices.Add(new Vector3(x + 1, y, z));
        _renderVertices.Add(new Vector3(x + 1, y + 1, z));
        _renderVertices.Add(new Vector3(x, y + 1, z));

        _renderNormals.Add(Vector3.Forward);
        _renderNormals.Add(Vector3.Forward);
        _renderNormals.Add(Vector3.Forward);
        _renderNormals.Add(Vector3.Forward);
    }

    bool IsVoxelInThisChunkBounds(int x, int y, int z)
    {
        return x > -1 && x < TerrainManager.CHUNK_SIZE && y > -1 && y < TerrainManager.CHUNK_SIZE && z > -1 && z < TerrainManager.CHUNK_SIZE;
    }

    int ConvertBlockIndexToLocal(int i)
    {
        if (i == -1)
            i = TerrainManager.CHUNK_SIZE - 1;
        else if (i == TerrainManager.CHUNK_SIZE)
            i = 0;
        return i;
    }

    bool CheckIfVoxelCauseRender(int x, int y, int z, int x_this, int y_this, int z_this)
    {
        if (IsVoxelInThisChunkBounds(x, y, z))
        {
            return BlockId.Air == blocksIdArray[x * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE + y * TerrainManager.CHUNK_SIZE + z];
        }
        else
        {
            Vector3Int neighbourChunkPos = position + (new Vector3Int(x, y, z) - new Vector3Int(x_this, y_this, z_this)) * TerrainManager.CHUNK_SIZE;

            if (neighbourChunkPos.x < 0 || neighbourChunkPos.x > TerrainManager.WORLD_SIZE_X * TerrainManager.CHUNK_SIZE - 1 ||
                neighbourChunkPos.y < 0 || neighbourChunkPos.y > TerrainManager.WORLD_SIZE_Y * TerrainManager.CHUNK_SIZE - 1 ||
                neighbourChunkPos.z < 0 || neighbourChunkPos.z > TerrainManager.WORLD_SIZE_Z * TerrainManager.CHUNK_SIZE - 1)
            {
                return true;
            }

            Chunk chunk = SINGLETONS.MainManager.TerrainManager.chunks[neighbourChunkPos.x / TerrainManager.CHUNK_SIZE * TerrainManager.WORLD_SIZE_Y * TerrainManager.WORLD_SIZE_Z + neighbourChunkPos.y / TerrainManager.CHUNK_SIZE * TerrainManager.WORLD_SIZE_Z + neighbourChunkPos.z / TerrainManager.CHUNK_SIZE];

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            return BlockId.Air == chunk.blocksIdArray[x * TerrainManager.CHUNK_SIZE * TerrainManager.CHUNK_SIZE + y * TerrainManager.CHUNK_SIZE + z];
        }
    }

    public bool visible = true;

    public void HideChunk()
    {
        visible = false;
        if (_instanceRid != null)
        {
            VisualServer.InstanceSetVisible(_instanceRid, false);
        }
    }

    public void ShowChunk()
    {
        visible = true;
        if (_instanceRid != null)
        {
            VisualServer.InstanceSetVisible(_instanceRid, true);
        }
    }
}