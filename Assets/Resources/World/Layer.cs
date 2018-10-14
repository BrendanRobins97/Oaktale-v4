using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Layer { 

    public int Width { get; private set; }
    public int Height { get; private set; }

    [HideInInspector][SerializeField] private Tile[] tiles;

    public Layer(int width, int height)
    {
        Width = width;
        Height = height;

        tiles = new Tile[width * height];
        for (int i = 0; i < width * height; i++)
        {
            tiles[i] = Tile.empty;
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (InRange(x, y))
        {
            return tiles[x + y * Width];
        }
        return Tile.empty;
    }
    public Tile GetTile(Int2 pos)
    {
        return GetTile(pos.x, pos.y);
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (InRange(x, y))
        {
            tiles[x + y * Width] = tile;
        }
    }
    public void SetTile(Int2 pos, Tile tile)
    {
        SetTile(pos.x, pos.y, tile);
    }

    public bool HasTile(int x, int y)
    {
        return InRange(x, y) && tiles[x + y * Width].ID != 0;
    }
    public bool HasTile(Int2 pos)
    {
        return HasTile(pos.x, pos.y);
    }

    public void DeleteTile(int x, int y)
    {
        SetTile(x, y, Tile.empty);
    }
    public void DeleteTile(Int2 pos)
    {
        DeleteTile(pos.x, pos.y);
    }

    private bool InRange(int x, int y)
    {
        if (x >= 0 && 
            x < Width && 
            y >= 0 && 
            y < Height)
        {
            return true;
        }
        return false;
    }
}
