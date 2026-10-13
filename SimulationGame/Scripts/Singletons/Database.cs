using Godot;
using System.Collections.Generic;

public class Database : Node
{
    Dictionary<BlockId, BlockData> _blocksData;

    public override void _Ready()
    {
        SINGLETONS.Database = this;

        LoadBlocksData();
    }

    void LoadBlocksData()
    {
        _blocksData = new Dictionary<BlockId, BlockData>()
        {
            {
                BlockId.Air,
                null
            },
            {
                BlockId.Grass,
                new BlockData(
                    false,
                    BlockData.TextureMode.EveryDifferent,
                    Colors.Green
                )
            },
            {
                BlockId.Dirt,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Brown
                )
            },
            {
                BlockId.Stone,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Gray
                )
            },
            {
                BlockId.Sand,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Yellow
                )
            },
            {
                BlockId.WoodLog,
                new BlockData(
                    false,
                    BlockData.TextureMode.EveryDifferent,
                    Colors.RosyBrown
                )
            },
            {
                BlockId.Leaves,
                new BlockData(
                    true,
                    BlockData.TextureMode.OneForAll,
                    Colors.DarkGreen
                )
            },
            {
                BlockId.CoalOre,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Black
                )
            },
            {
                BlockId.IronOre,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Salmon
                )
            },
            {
                BlockId.GoldOre,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Gold
                )
            },
            {
                BlockId.DiamondOre,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.Aqua
                )
            },
            {
                BlockId.Bedrock,
                new BlockData(
                    false,
                    BlockData.TextureMode.OneForAll,
                    Colors.DarkGray
                )
            }
        };
    }

    public BlockData GetBlockData(BlockId id)
    {
        return _blocksData[id];
    }
}
