using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshData {

    public static readonly float border = 2f;
    public static readonly float tileSize = 12f;
    public static readonly float tileTextureHeight = 2048f;
    public static readonly float tileTextureWidth = 2048f;
    public static readonly float foregroundTextureHeight = 2304;
    public static readonly float foregroundTextureWidth = 480f;

    public static void GetFrontBlockMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        Tile tile = world.GetTile(0, pos);
        float layer = ItemDatabase.GetLayer(tile.ID);

        List<Vector3> verts = new List<Vector3>(4);
        List<Vector2> uvs = new List<Vector2>(4);

        int randomOffSetX = world.GetTile(0, pos).Variation;
        randomOffSetX += 4 * world.GetTile(0, pos).Info;

        verts.Add(new Vector3(1f + offset.x, offset.y, -layer));
        verts.Add(new Vector3(offset.x, offset.y, -layer));
        verts.Add(new Vector3(offset.x, 1f + offset.y, -layer));
        verts.Add(new Vector3(1f + offset.x, 1f + offset.y, -layer));


        uvs.Add(new Vector2((border + tileSize + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + tileSize + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + tileSize + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + tileSize + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + 16 * tile.ID)) / tileTextureHeight));



        // return the out variables
        UVs = uvs;
        vertices = verts;

    }
    public static void GetBackBlockMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        Tile tile = world.GetTile(0, pos);
        float layer = ItemDatabase.GetLayer(tile.ID);


        List<Vector3> verts = new List<Vector3>(4);

        List<Vector2> uvs = new List<Vector2>(4);

        int randomOffSetX = world.GetTile(0, pos).Variation;
        randomOffSetX += 4 * world.GetTile(0, pos).Info;

        // Add vertices
        verts.Add(new Vector3(1f + offset.x + border / tileSize, offset.y - border / tileSize, -layer + LayerInfo.BackBlock));
        verts.Add(new Vector3(offset.x - border / tileSize, offset.y - border / tileSize, -layer + LayerInfo.BackBlock));
        verts.Add(new Vector3(offset.x - border / tileSize, 1f + offset.y + border / tileSize, -layer + LayerInfo.BackBlock));
        verts.Add(new Vector3(1f + offset.x + border / tileSize, 1f + offset.y + border / tileSize, -layer + LayerInfo.BackBlock));


        // Add uv's
        uvs.Add(new Vector2(((1 + randomOffSetX) * 16) / tileTextureWidth,
                            (tileTextureHeight - ((1 + tile.ID) * 16)) / tileTextureHeight));
        uvs.Add(new Vector2((randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - ((1 + tile.ID) * 16)) / tileTextureHeight));
        uvs.Add(new Vector2((randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2(((1 + randomOffSetX) * 16) / tileTextureWidth,
                            (tileTextureHeight - (16 * tile.ID)) / tileTextureHeight));


        // return the out variables
        UVs = uvs;
        vertices = verts;

    }

    public static void GetWallMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        Tile tile = world.GetTile(1, pos);
        float layer = ItemDatabase.GetLayer(tile.ID);


        List<Vector3> verts = new List<Vector3>(4);
        List<Vector2> uvs = new List<Vector2>(4);
        int randomOffSetX = world.GetTile(1, pos).Variation + TextureOffset.Wall;
        randomOffSetX += 4 * world.GetTile(1, pos).Info;

        verts.Add(new Vector3(1f + offset.x, offset.y, -layer + LayerInfo.Wall));
        verts.Add(new Vector3(offset.x, offset.y, -layer + LayerInfo.Wall));
        verts.Add(new Vector3(offset.x, 1f + offset.y, -layer + LayerInfo.Wall));
        verts.Add(new Vector3(1f + offset.x, 1f + offset.y, -layer + LayerInfo.Wall));


        uvs.Add(new Vector2((border + tileSize + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + tileSize + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + tileSize + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + 16 * tile.ID)) / tileTextureHeight));
        uvs.Add(new Vector2((border + tileSize + randomOffSetX * 16) / tileTextureWidth,
                            (tileTextureHeight - (border + 16 * tile.ID)) / tileTextureHeight));

        int blockLayer = ItemDatabase.GetLayer(world.GetTile(1, pos.x, pos.y).ID);

        // Trim edge of block if there is a neighboring block
        if (ItemDatabase.GetLayer(world.GetTile(1, pos.x, pos.y + 1).ID) != blockLayer)
        {
            verts[2] += Vector3.up * border / tileSize;
            verts[3] += Vector3.up * border / tileSize;
            uvs[2] += Vector2.up * border / tileTextureHeight;
            uvs[3] += Vector2.up * border / tileTextureHeight;
        }

        if (ItemDatabase.GetLayer(world.GetTile(1, pos.x + 1, pos.y).ID) != blockLayer)
        {
            verts[0] += Vector3.right * border / tileSize;
            verts[3] += Vector3.right * border / tileSize;
            uvs[0] += Vector2.right * border / tileTextureWidth;
            uvs[3] += Vector2.right * border / tileTextureWidth;

        }
        if (ItemDatabase.GetLayer(world.GetTile(1, pos.x, pos.y - 1).ID) != blockLayer)
        {
            verts[0] += Vector3.down * border / tileSize;
            verts[1] += Vector3.down * border / tileSize;
            uvs[0] += Vector2.down * border / tileTextureHeight;
            uvs[1] += Vector2.down * border / tileTextureHeight;

        }
        if (ItemDatabase.GetLayer(world.GetTile(1, pos.x - 1, pos.y).ID) != blockLayer)
        {
            verts[1] += Vector3.left * border / tileSize;
            verts[2] += Vector3.left * border / tileSize;
            uvs[1] += Vector2.left * border / tileTextureWidth;
            uvs[2] += Vector2.left * border / tileTextureWidth;
        }

        // return the out variables
        UVs = uvs;
        vertices = verts;
    }

    public static void GetForegroundMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        Tile tile = world.GetTile(3, pos);
        float layer = ItemDatabase.GetLayer(tile.ID);


        List<Vector3> verts = new List<Vector3>(4);
        List<Vector2> uvs = new List<Vector2>(4);

        int randomOffSetX = world.GetTile(3, pos).Variation;
        randomOffSetX += 4 * world.GetTile(3, pos).Info;
        // Add vertices
        verts.Add(new Vector3(1f + offset.x, offset.y - 24 / tileSize, -layer + LayerInfo.Foreground));
        verts.Add(new Vector3(offset.x, offset.y - 24 / tileSize, -layer + LayerInfo.Foreground));
        verts.Add(new Vector3(offset.x, offset.y + 12 / tileSize, -layer + LayerInfo.Foreground));
        verts.Add(new Vector3(1f + offset.x, offset.y + 12 / tileSize, -layer + LayerInfo.Foreground));

        int index = (tile.ID - 1000) + 0;
        // Add uv's
        uvs.Add(new Vector2(((1 + randomOffSetX) * 12) / foregroundTextureWidth,
                            (foregroundTextureHeight - 36 * index) / foregroundTextureHeight));
        uvs.Add(new Vector2((randomOffSetX * 12) / foregroundTextureWidth,
                            (foregroundTextureHeight - 36 * index) / foregroundTextureHeight));
        uvs.Add(new Vector2((randomOffSetX * 12) / foregroundTextureWidth,
                            (foregroundTextureHeight - 36 * (index - 1)) / foregroundTextureHeight));
        uvs.Add(new Vector2(((1 + randomOffSetX) * 12) / foregroundTextureWidth,
                            (foregroundTextureHeight - 36 * (index - 1)) / foregroundTextureHeight));


        // return the out variables
        UVs = uvs;
        vertices = verts;
    }

    public static void GetLiquidMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        List<Vector3> verts = new List<Vector3>(4);
        List<Vector2> uvs = new List<Vector2>(4);
        Tile tileAbove = world.GetTile(0, pos.x, pos.y + 1);

        float liquidAmount;
        if (tileAbove.ID == 8001 && tileAbove.Info > 0)
        {
            liquidAmount = 1f;

        }
        else
        {
            liquidAmount = (float)world.GetTile(0, pos).Info / byte.MaxValue;

        }

        verts.Add(new Vector3(1f + offset.x, offset.y, LayerInfo.Liquid));
        verts.Add(new Vector3(offset.x, offset.y, LayerInfo.Liquid));
        verts.Add(new Vector3(offset.x, liquidAmount + offset.y, LayerInfo.Liquid));
        verts.Add(new Vector3(1f + offset.x, liquidAmount + offset.y, LayerInfo.Liquid));

        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, liquidAmount));
        uvs.Add(new Vector2(1, liquidAmount));

        // return the out variables
        UVs = uvs;
        vertices = verts;

    }
}
