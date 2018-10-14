using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World")]
public class WorldSO : ScriptableObject {

    private static readonly int NumLayers = 5;
    private static readonly int SeedRange = 10000;

    public string Name;
    public int Width;
    public int Height;
    public int Seed;

    public bool terrainGenerated = false;

    [HideInInspector] [SerializeField] private Layer[] layers;

    public void SetupWorld()
    {
        layers = new Layer[NumLayers];
        for (int i = 0; i < NumLayers; i++)
        {
            layers[i] = new Layer(Width, Height);
        }
        if (Seed == -1)
        {
            RandomSeed();
        }
        if (!terrainGenerated)
        {
            GenerateTerrain();
            terrainGenerated = true;
        }
    }

    public virtual void GenerateTerrain()
    {
        //TerrainGenerator.GenerateNormalWorld(this);
    }

    public void RandomSeed()
    {
        this.Seed = UnityEngine.Random.Range(0, SeedRange);

    }
    public bool HasTile(int layer, int x, int y)
    {
        return layers[layer].HasTile(x, y);
    }
    public bool HasTile(int layer, Int2 pos)
    {
        return HasTile(layer, pos.x, pos.y);
    }
    public bool HasTile(int layer, Int2 bottomLeft, Int2 topRight)
    {
        for (int x = bottomLeft.x; x < topRight.x; x++)
        {
            for (int y = bottomLeft.y; y < topRight.y; y++)
            {
                if (HasTile(layer, x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Tile GetTile(int layer, int x, int y)
    {
        return layers[layer].GetTile(x, y);

    }
    public Tile GetTile(int layer, Int2 pos)
    {
        return GetTile(layer, pos.x, pos.y);
    }

    public void SetTile(int layer, int x, int y, Tile tile)
    {
        layers[layer].SetTile(x, y, tile);
    }
    public void SetTile(int layer, int x, int y, int id)
    {
        SetTile(layer, x, y, new Tile((ushort)id));
    }
    public void SetTile(int layer, Int2 pos, Tile tile)
    {
        SetTile(layer, pos.x, pos.y, tile);
    }
    public void SetTile(int layer, Int2 pos, int id)
    {
        SetTile(layer, pos.x, pos.y, new Tile((ushort)id));
    }

    public void DeleteTile(int layer, int x, int y)
    {
        layers[layer].DeleteTile(x, y);
    }
    public void DeleteTile(int layer, Int2 pos)
    {
        DeleteTile(layer, pos.x, pos.y);
    }
}
