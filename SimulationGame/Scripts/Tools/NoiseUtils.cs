using Godot;
using System;
using System.Collections.Generic;

public class NoiseUtils
{
    int _maxHeight = 128;
    // static float _smooth = 0.01f;

    // static int _seed = (int)GD.Randi();

    OpenSimplexNoise _noise = new OpenSimplexNoise() {
        Period = 400,
        Octaves = 4,
        Persistence = 0.5f
    };

    OpenSimplexNoise _stoneNoise = new OpenSimplexNoise() {
        Period = 400,
        Octaves = 4 + 1,
        Persistence = 0.5f
    };

    OpenSimplexNoise _caveNoise = new OpenSimplexNoise() {
        Period = 10,
        Octaves = 3,
        Persistence = 0.5f
    };

    OpenSimplexNoise _oreNoise = new OpenSimplexNoise() {
        Period = 5,
        Octaves = 2,
        Persistence = 0.01f
    };

    static NoiseUtils _instance = null;

    public static NoiseUtils Instance {
        get
        {
            if (_instance == null)
                _instance = new NoiseUtils();
            
            return _instance;
        }
    }

    Dictionary<BlockId, int> _oreSeeds;

    public NoiseUtils()
    {
        GD.Randomize();

        int seed = (int)GD.Randi();

        _noise.Seed = seed;
        _stoneNoise.Seed = seed;
        _caveNoise.Seed = seed;
        _oreNoise.Seed = seed;

        _oreSeeds = new Dictionary<BlockId, int>()
        {
            {
                BlockId.CoalOre,
                (int)GD.Randi()
            },
            {
                BlockId.IronOre,
                (int)GD.Randi()
            },
            {
                BlockId.GoldOre,
                (int)GD.Randi()
            },
            {
                BlockId.DiamondOre,
                (int)GD.Randi()
            }
        };
    }

    public int GenerateHeight(float x, float y)
    {
        return (int)Map(0, _maxHeight, 0, 1, (_noise.GetNoise2d(x, y) + 1) / 2f);
    }

    public int GenerateStoneHeight(float x, float y)
    {
        return (int)Map(0, _maxHeight - 16, 0, 1, (_stoneNoise.GetNoise2d(x, y) + 1) / 2f);
    }

    public float GetValueForCaves(float x, float y, float z)
    {
        return (_caveNoise.GetNoise3d(x, y, z) + 1) / 2f;
    }

    public float GetValueForOre(float x, float y, float z, BlockId oreId)
    {
        _oreNoise.Seed = _oreSeeds[oreId];
        return (_oreNoise.GetNoise3d(x, y, z) + 1) / 2f;
    }

    static float Map(float newmin, float newmax, float origmin, float origmax, float value)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(origmin, origmax, value));
    }
}
