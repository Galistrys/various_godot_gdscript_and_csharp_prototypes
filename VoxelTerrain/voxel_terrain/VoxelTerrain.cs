using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

using ChunkCoords = Godot.Vector3I;
using WorldCoords = Godot.Vector3;

public partial class VoxelTerrain : Node3D
{
    public static Vector3I worldDimensions = new Vector3I(10, 10, 10);
    public VoxelChunk[] _chunksArray;

    public VoxelTerrain()
    {
        VoxelChunk.chunkMaterial = BlocksDatabase.Instance.voxelTerrainMaterial;
    }

    public override void _Ready()
    {
        _chunksArray = new VoxelChunk[worldDimensions.X * worldDimensions.Y * worldDimensions.Z];
        BuildWorld();

        // for (int x = 0; x < worldDimensions.X; x++)
        // {
        //     for (int z = 0; z < worldDimensions.Z; z++)
        //     {
        //         for (int y = 0; y < worldDimensions.Y; y++)
        //         {
        //             VoxelChunk chunk = new VoxelChunk(new WorldCoords(x, y, z) * VoxelChunk.CHUNK_SIZE);

        //             chunk.CreateInstance(GetWorld3D().Scenario);
        //             chunk.GenerateData();
        //             chunk.RegenerateMesh();

        //             _chunksArray[x + worldDimensions.X * (z + y * worldDimensions.Z)] = chunk;
        //         }
        //     }
        // }
    }

    public async void BuildWorld()
    {
        for (int x = 0; x < worldDimensions.X; x++)
        {
            for (int z = 0; z < worldDimensions.Z; z++)
            {
                for (int y = 0; y < worldDimensions.Y; y++)
                {
                    VoxelChunk chunk = new VoxelChunk(new WorldCoords(x, y, z) * VoxelChunk.CHUNK_SIZE);

                    chunk.CreateInstance(GetWorld3D().Scenario);
                    chunk.CreateBody(GetWorld3D().Space);
                    chunk.GenerateData();
                    chunk.RegenerateMesh();

                    _chunksArray[x + worldDimensions.X * (z + y * worldDimensions.Z)] = chunk;

                    await ToSignal(GetTree(), "process_frame");
                }
            }
        }
    }

    public override void _ExitTree()
    {
        for (int i = 0; i < _chunksArray.Length; i++)
        {
            _chunksArray[i].Destroy();
            _chunksArray[i] = null;
        }
        _chunksArray = null;
    }
}
