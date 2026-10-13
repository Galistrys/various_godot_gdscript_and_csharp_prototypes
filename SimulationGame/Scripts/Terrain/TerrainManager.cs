using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TerrainManager : Spatial
{
    public delegate void WorldArrayGenerated();

    public FreeCamera player;

    public const int WORLD_SIZE_X = 64;
    public const int WORLD_SIZE_Y = 8;
    public const int WORLD_SIZE_Z = 64;

    public const int CHUNKS_COUNT = WORLD_SIZE_X * WORLD_SIZE_Y * WORLD_SIZE_Z;

    int _chunksBuilded = 0;
    int _chunksDrawed = 0;

    public const int CHUNK_SIZE = 16;

    bool _allChunksGenerated = false;

    int radius = 5;

    public Chunk[] chunks = new Chunk[WORLD_SIZE_X * WORLD_SIZE_Y * WORLD_SIZE_Z];

    enum ChunkState
    {
        ToDraw,
        Drawn,
        ToRemove
    }
    ConcurrentDictionary<Chunk, ChunkState> visibleChunks = new ConcurrentDictionary<Chunk, ChunkState>();

    public override void _Ready()
    {
        SINGLETONS.MainManager.TerrainManager = this;

        player = GetNode<FreeCamera>("../FreeCamera");
        _lastPosition = GetPlayerPosInChunks();

        Chunk.Scenario = GetWorld().Scenario;
    }

    Vector3 _lastPosition = new Vector3(0, 0, 0);

    public override void _Process(float delta)
    {
        Vector3 currentPosition = GetPlayerPosInChunks();
        if (_lastPosition != currentPosition)
        {
            GD.Print("Changed Chunk: ", _lastPosition, " -> ", currentPosition);

            StartDrawingProcess((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z);
            
            _lastPosition = currentPosition;
        }
    }

    public Vector3 GetPlayerPosInChunks()
    {
        Vector3 playerPos = player.Translation / CHUNK_SIZE;

        int x = Mathf.FloorToInt(playerPos.x);
        int y = Mathf.FloorToInt(playerPos.y);
        int z = Mathf.FloorToInt(playerPos.z);

        return new Vector3(x, y, z);
    }

    public void DestroyWorld()
    {
        foreach(Chunk chunk in chunks)
        {
            chunk.ShowChunk();
            chunk.DestroyChunk();
        }

        GetTree().CallDeferred("quit");
    }

    public async void GenerateWorld()
    {
        SINGLETONS.MainManager.GameManager.progressBar.MaxValue = CHUNKS_COUNT * 3;
        GetNode<Label>("../../LoadCanvas/CentCont/VBox/LoadLabel").Text = "Generating World";

        System.Threading.Thread generateThread = new System.Threading.Thread(() => GenerateChunksInThread());

        generateThread.Start();
        while(!_allChunksGenerated)
        {
            await ToSignal(GetTree(), "idle_frame");
        }
        generateThread.Join();
        generateThread = null;

        GetNode<Label>("../../LoadCanvas/CentCont/VBox/LoadLabel").Text = "Building World";

        System.Threading.Thread buildChunks = new System.Threading.Thread(() => BuildChunksInThread());
        buildChunks.Start();
        
        while (_chunksBuilded != CHUNKS_COUNT)
        {
            await ToSignal(GetTree(), "idle_frame");
        }

        buildChunks.Join();
        buildChunks = null;

        GetNode<Label>("../../LoadCanvas/CentCont/VBox/LoadLabel").Text = "Drawing World";

        System.Threading.Thread drawChunks = new System.Threading.Thread(() => DrawChunksInThread());
        drawChunks.Start();

        while (_chunksDrawed != CHUNKS_COUNT)
        {
            await ToSignal(GetTree(), "idle_frame");
        }

        drawChunks.Join();
        drawChunks = null;

        ShowChunk(0, 0, 0);
        StartDrawingProcess(0, 0, 0);

        GetNode<CanvasLayer>("../../LoadCanvas").QueueFree();
    }

    public void GenerateChunksInThread()
    {
        for (int x = 0; x < WORLD_SIZE_X; x++)
        {
            SINGLETONS.MainManager.GameManager.progressBar.Value += WORLD_SIZE_Z * WORLD_SIZE_Y;

            for (int z = 0; z < WORLD_SIZE_Z; z++)
            {
                for (int y = 0; y < WORLD_SIZE_Y; y++)
                {
                    chunks[x * WORLD_SIZE_Y * WORLD_SIZE_Z + y * WORLD_SIZE_Z + z] = null;
                    Chunk chunk;

                    chunk = new Chunk(new Vector3Int(x * CHUNK_SIZE, y * CHUNK_SIZE, z * CHUNK_SIZE));
                    chunk.GenerateChunk();
                    chunks[x * WORLD_SIZE_Y * WORLD_SIZE_Z + y * WORLD_SIZE_Z + z] = chunk;
                }
            }
        }

        _allChunksGenerated = true;
    }

    public void BuildChunksInThread()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.BuildChunk(true);
            _chunksBuilded++;
            SINGLETONS.MainManager.GameManager.progressBar.Value++;
        }
    }

    public void DrawChunksInThread()
    {
        foreach (Chunk chunk in chunks)
        {
            chunk.DrawChunk();
            _chunksDrawed++;
            SINGLETONS.MainManager.GameManager.progressBar.Value++;

            chunk.HideChunk();
        }
    }

    public void ShowChunk(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x > WORLD_SIZE_X - 1 || y > WORLD_SIZE_Y - 1 || z > WORLD_SIZE_Z - 1)
            return;
        chunks[x * WORLD_SIZE_Y * WORLD_SIZE_Z + y * WORLD_SIZE_Z + z].ShowChunk();
    }

    public void HideChunk(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x > WORLD_SIZE_X - 1 || y > WORLD_SIZE_Y - 1 || z > WORLD_SIZE_Z - 1)
            return;
        chunks[x * WORLD_SIZE_Y * WORLD_SIZE_Z + y * WORLD_SIZE_Z + z].HideChunk();
    }

    System.Threading.Thread _drawingThread;

    public void StartDrawingProcess(int x, int y, int z)
    {
        if (_drawingThread == null)
            _drawingThread = new System.Threading.Thread(() => DrawProcess(x, y, z, radius));
        else
        {
            _requestedDrawProcessToStop = true;
            _drawingThread.Join();
            _drawingThread = new System.Threading.Thread(() => DrawProcess(x, y, z, radius));
            _requestedDrawProcessToStop = false;
        }
        
        _drawingThread.Start();
    }

    bool _requestedDrawProcessToStop = false;
    public void DrawProcess(int x, int y, int z, int rad)
    {
        rad--;
        if (rad < 0)
            return;

        if (_requestedDrawProcessToStop)
            return;
        
        ShowChunk(x, y - 1, z);
        ShowChunk(x, y + 1, z);
        ShowChunk(x - 1, y, z);
        ShowChunk(x + 1, y, z);
        ShowChunk(x, y, z - 1);
        ShowChunk(x, y, z + 1);
        DrawProcess(x, y - 1, z, rad);
        DrawProcess(x, y + 1, z, rad);
        DrawProcess(x - 1, y, z, rad);
        DrawProcess(x + 1, y, z, rad);
        DrawProcess(x, y, z - 1, rad);
        DrawProcess(x, y, z + 1, rad);
    }
}