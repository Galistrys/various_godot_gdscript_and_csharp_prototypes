using Godot;
using System;
using System.Collections.Generic;

public partial class BlocksDatabase : Node
{ 
    public static BlocksDatabase Instance { get; private set; }


    public const string BLOCK_DEFS_DIR = "res://voxel_terrain/block_types/";
    public const string TEXTURES_DIR = "res://assets/voxel_tiles/";

    public const string BLOCK_DEF_EXT = ".res";

    public const int TILE_SIZE = 128;

    public Dictionary<string, BlockType> _blockTypesDict = new Dictionary<string, BlockType>();

    public StandardMaterial3D voxelTerrainMaterial = ResourceLoader.Load<StandardMaterial3D>("res://voxel_terrain/voxel_terrain.material");
    

    public BlocksDatabase()
    {
        Instance = this;
    }


    public void LoadData()
    {
        var directory = DirAccess.Open(BLOCK_DEFS_DIR);
        var err = DirAccess.GetOpenError();
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to open BLOCK_DEFS_DIR at (\"{BLOCK_DEFS_DIR}\"). Reason code: {err}");
            return;
        }

        err = directory.ListDirBegin();
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to iterate through BLOCK_DEFS_DIR at (\"{BLOCK_DEFS_DIR}\"). Reason code: {err}");
            return;
        }

        Dictionary<string, BlockDef> block_definitions = new Dictionary<string, BlockDef>();
        Dictionary<string, Texture2D> texture_mapping = new Dictionary<string, Texture2D>();
        
        string file_name;
        while ((file_name = directory.GetNext()) != String.Empty)
        {
            if (file_name.EndsWith(BLOCK_DEF_EXT))
            {
                BlockDef block_def = ResourceLoader.Load<BlockDef>(BLOCK_DEFS_DIR + file_name);

                block_definitions.Add(file_name.Replace(BLOCK_DEF_EXT, ""), block_def);

                foreach (var texture in block_def.GetTextures())
                {
                    if (texture != null)
                        AddTextureToDict(texture, texture_mapping);
                    else
                        GD.PrintErr(file_name, " -> Null texture detected.");
                }
            }
        }

        directory.ListDirEnd();

        int tiles_in_row = Mathf.CeilToInt(Mathf.Pow(2, Mathf.Ceil(Mathf.Log(Mathf.Sqrt(texture_mapping.Count))/Mathf.Log(2))));
        Image atlas_image = Image.Create(tiles_in_row * TILE_SIZE, tiles_in_row * TILE_SIZE, false, Image.Format.Rgba8);

        Dictionary<string, Vector2[]> texture_uvs = new Dictionary<string, Vector2[]>();
        float uv_unit = 1.0f / tiles_in_row;

        Vector2I current_origin = new Vector2I(0, 0);
        foreach (var texture_id in texture_mapping.Keys)
        {
            var texture_image = texture_mapping[texture_id].GetImage();
            AddImageToImage(atlas_image, texture_image, current_origin);

            // uv10, uv00, uv01, uv11
            Vector2 origin = current_origin / new Vector2(tiles_in_row * TILE_SIZE, tiles_in_row * TILE_SIZE);
            texture_uvs.Add(texture_id, new Vector2[] {
                origin + Vector2.Right * uv_unit,
                origin,
                origin + Vector2.Down * uv_unit,
                origin + Vector2.One * uv_unit,
            });

            current_origin += Vector2I.Right * TILE_SIZE;
            if (current_origin.X == TILE_SIZE * tiles_in_row)
                current_origin = Vector2I.Down * (current_origin.Y + TILE_SIZE);
        }

        voxelTerrainMaterial.AlbedoTexture = ImageTexture.CreateFromImage(atlas_image);

        foreach (var block_id in block_definitions.Keys)
        {
            BlockTexturingMethod block_exturing_method = null;

            var block_def = block_definitions[block_id];

            if (block_def.texturingMode == BlockDef.TexturingMode.AllSame)
                block_exturing_method = new AllSameTexturing(texture_uvs[block_def.allSameTexture.ResourcePath]);
            else if (block_def.texturingMode == BlockDef.TexturingMode.SameSides)
                block_exturing_method = new SameSidesTexturing(
                    texture_uvs[block_def.sameSidesSideTexture.ResourcePath],
                    texture_uvs[block_def.sameSidesTopAndBottomTexture.ResourcePath]
                );
            else if (block_def.texturingMode == BlockDef.TexturingMode.SameSidesDiffTB)
                block_exturing_method = new SameSidesDiffTBTexturing(
                    texture_uvs[block_def.sameSidesDiffTBSideTexture.ResourcePath],
                    texture_uvs[block_def.sameSidesDiffTBTopTexture.ResourcePath],
                    texture_uvs[block_def.sameSidesDiffTBBottomTexture.ResourcePath]
                );
            else if (block_def.texturingMode == BlockDef.TexturingMode.AllDifferent)
                block_exturing_method = new AllDifferentTexturing(
                    texture_uvs[block_def.diffTopTexture.ResourcePath],
                    texture_uvs[block_def.diffBottomTexture.ResourcePath],
                    texture_uvs[block_def.diffLeftTexture.ResourcePath],
                    texture_uvs[block_def.diffRightTexture.ResourcePath],
                    texture_uvs[block_def.diffFrontTexture.ResourcePath],
                    texture_uvs[block_def.diffBackTexture.ResourcePath]
                );
            else
                GD.PushError("Unsupported Texturing Method");

            _blockTypesDict.Add(
                block_id,
                new BlockType(
                    block_id,
                    block_def.transparent,
                    block_exturing_method
                )
            );
        }
    }

    void AddImageToImage(Image baseImage, Image addImage, Vector2I origin)
    {
        for (int x = 0; x < addImage.GetWidth(); x++)
        {
            for (int y = 0; y < addImage.GetWidth(); y++)
            {
                Vector2I current = new Vector2I(x, y);
                baseImage.SetPixelv(
                    origin + current,
                    addImage.GetPixelv(current)
                );
            }
        }
    }

    void AddTextureToDict(Texture2D texture, Dictionary<string, Texture2D> dict)
    {
        if (texture != null)
        {
            dict.TryAdd(texture.ResourcePath, texture);
        }
    }

    public BlockType GetBlockType(string blockId)
    {
        BlockType result = null;
        if (_blockTypesDict.TryGetValue(blockId, out result))
            return result;
        else
            GD.PushError($"Block type \"{blockId}\" not found");
            return null;
    }
}
