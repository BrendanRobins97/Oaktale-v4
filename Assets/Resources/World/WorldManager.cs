// File: WorldManager.cs
// Author: Brendan Robinson
// Date Created: 01/01/2018
// Date Last Modified: 08/02/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public World currentWorld;
    public List<World> worlds = new List<World>();

    public PlayerInfo playerInfo;

    public bool spriteImage;
    public bool generateTerrain;

    public GameObject pickup;
    public Transform pickupContainer;
    public Int2 playerSpawn;
    public Player player;
    public Transform decorationContainer;
    public GameObject tileBreakPrefab;

    public Dictionary<Int2, TileBreakManager> tileHealths = new Dictionary<Int2, TileBreakManager>();

    private Dictionary<Int2, DecorationController> decorations = new Dictionary<Int2, DecorationController>(128);
    private TerrainGenerator TerrainGenerator;

    private void Awake()
    {
        GameManager.Set(this);
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 2000;
        pickup = Resources.Load<GameObject>("Prefabs/Pickup");

        ItemDatabase.ConstructDatabase();

    }

    private void Start()
    {
        TerrainGenerator = GameManager.Get<TerrainGenerator>();
        pickupContainer = gameObject.transform;
        SimplePool.Preload(pickup, pickupContainer, 200);

        if (generateTerrain)
        {
            worlds.Add(new World("FirstWorld", 2000, 2000));
            worlds.Add(new World("SecondWorld", 256, 256));

            TerrainGenerator.SetWorld(worlds[0]);
            TerrainGenerator.GenerateNormalWorld();

            TerrainGenerator.SetWorld(worlds[1]);
            TerrainGenerator.GenerateMaplestoryShop();
        }

        currentWorld = worlds[0];
        //currentWorld.SetBlock(60, 540, new Tile(8001, 0, 127));
        //currentWorld.SetBlock(61, 540, new Tile(8001, 0, 127));
        //currentWorld.SetBlock(60, 541, new Tile(8001, 0, 127));
        //currentWorld.SetBlock(61, 541, new Tile(8001, 0, 127));
        if (spriteImage) currentWorld.DisplaySpriteMap();

        InstantiateDecorations();
    }
    private void Update()
    {
        List<Int2> positions = new List<Int2>(tileHealths.Keys);
        foreach (Int2 pos in positions)
            if (tileHealths[pos].currentHealth > tileHealths[pos].maxHealth)
            {
                SimplePool.Despawn(tileHealths[pos].gameObject);
                tileHealths.Remove(pos);
            }
    }

    public void SwitchWorld(int worldIndex, Int2 playerSpawn)
    {
        currentWorld = worlds[worldIndex];
        InstantiateDecorations();
        GameManager.Get<Player>().Teleport(playerSpawn);
        GameManager.Get<ChunkManager>().ResetPools();
    }

    #region BlockManagement

    public bool SetBlock(int x, int y, Tile tile)
    {
        if (currentWorld.HasTile(2, x, y)) return false;
        if (currentWorld.GetTile(0, x, y).ID == tile.ID && currentWorld.GetTile(0, x, y).Info != tile.Info)
        {
            currentWorld.SetTile(0, x, y, tile);
            UpdateChunk(x, y);
            return false;
        }

        if (currentWorld.HasTile(0, x, y)) return false;

        currentWorld.DeleteTile(3, x, y);

        Tile block;
        switch (tile.Info)
        {
            case 0:

                block = currentWorld.GetTile(3, x + 1, y + 1);
                block.Info = 0;
                currentWorld.SetTile(3, x + 1, y + 1, block);
                block = currentWorld.GetTile(3, x - 1, y + 1);
                block.Info = 0;
                currentWorld.SetTile(3, x - 1, y + 1, block);
                block = currentWorld.GetTile(3, x, y + 2);
                block.Info = 0;
                currentWorld.SetTile(3, x, y + 2, block);
                block = currentWorld.GetTile(3, x, y);
                block.Info = 0;
                currentWorld.SetTile(3, x, y, block);
                break;
            default:
                break;
        }

        currentWorld.SetTile(0, x, y, tile);
        UpdateChunk(x, y);
        return true;
    }

    public bool SetBlock(Int2 pos, Tile tile)
    {
        return SetBlock(pos.x, pos.y, tile);
    }

    public bool SetBlock(int x, int y, int id)
    {
        return SetBlock(x, y, new Tile(id));
    }

    public bool SetBlock(Int2 pos, int id)
    {
        return SetBlock(pos.x, pos.y, new Tile(id));
    }

    public bool DeleteBlock(int x, int y)
    {
        if (currentWorld.HasTile(2, x, y + 1)) return false;
        ushort blockId = currentWorld.GetTile(0, x, y).ID;
        if (blockId == 0) return false;

        ItemDatabase.GetItemData<BlockData>(blockId).Delete(x, y);

        currentWorld.DeleteTile(3, x, y + 1);
        currentWorld.DeleteTile(0, x, y);
        UpdateChunk(x, y);
        return true;
    }

    public bool DeleteBlock(Int2 pos)
    {
        return DeleteBlock(pos.x, pos.y);
    }

    #endregion

    #region DecorationManagement

    public bool SetDecoration(int x, int y, Tile tile)
    {
        DecorationData decoration = ItemDatabase.GetDecoration(tile.ID);
        switch (decoration.DecorationType)
        {
            case DecorationType.Grounded:
                for (int i = x; i < x + decoration.Width; i++)
                {
                    if (!currentWorld.HasTile(0, i, y - 1)) return false;
                    for (int j = y; j < y + decoration.Height; j++)
                        if (currentWorld.HasTile(2, i, j) || currentWorld.HasTile(0, i, j))
                            return false; // Decoration spot occupied
                }

                break;
            case DecorationType.Mounted:
                for (int i = x; i < x + decoration.Width; i++)
                    for (int j = y; j < y + decoration.Height; j++)
                        if (currentWorld.HasTile(2, i, j) || currentWorld.HasTile(0, i, j))
                            return false; // Decoration spot occupied
                bool blocksUnder = true;
                for (int i = x; i < x + decoration.Width; i++)
                    if (!currentWorld.HasTile(0, i, y - 1))
                        blocksUnder = false;
                if (!blocksUnder)
                    for (int i = x; i < x + decoration.Width; i++)
                        for (int j = y; j < y + decoration.Height; j++)
                            if (!currentWorld.HasTile(1, i, j))
                                return false; // Decoration spot occupied
                break;
            default:
                for (int i = x; i < x + decoration.Width; i++)
                    for (int j = y; j < y + decoration.Height; j++)
                        if (currentWorld.HasTile(2, i, j) || currentWorld.HasTile(0, i, j))
                            return false; // Decoration spot occupied
                break;
        }

        for (int i = x; i < x + decoration.Width; i++)
            for (int j = y; j < y + decoration.Height; j++)
                currentWorld.SetTile(2, i, j, ushort.MaxValue);
        currentWorld.SetTile(2, x, y, tile.ID);

        return true;
    }

    public bool SetDecoration(Int2 pos, Tile tile)
    {
        return SetDecoration(pos.x, pos.y, tile);
    }

    public bool SetDecoration(int x, int y, int id)
    {
        return SetDecoration(x, y, new Tile(id));
    }

    public bool SetDecoration(Int2 pos, int id)
    {
        return SetDecoration(pos.x, pos.y, new Tile(id));
    }

    public void DeleteDecoration(Int2 pos)
    {
        Tile tile = currentWorld.GetTile(2, pos);
        DecorationData decoration = ItemDatabase.GetDecoration(tile.ID);
        if (!decoration) return;
        Destroy(decorations[pos].gameObject);
        decorations.Remove(pos);

        decoration.Delete(pos.x, pos.y);

        for (int x = pos.x; x < pos.x + decoration.Width; x++)
            for (int y = pos.y; y < pos.y + decoration.Height; y++)
                currentWorld.SetTile(2, x, y, 0);
    }

    private void InstantiateDecorations()
    {
        int id;
        for (int x = 0; x < currentWorld.Width; x++)
            for (int y = 0; y < currentWorld.Height; y++)
            {
                id = currentWorld.GetTile(2, x, y).ID;
                if (id != 0 && id != ushort.MaxValue)
                    ItemDatabase.GetItemData<DecorationData>(id).InstantiateDecoration(x, y);
            }
    }

    public void AddDecoration(int x, int y, DecorationController decoration)
    {
        decorations.Add(new Int2(x, y), decoration);
    }

    #endregion

    #region WallManagement

    public bool SetWall(int x, int y, Tile tile)
    {
        if (currentWorld.GetTile(1, x, y).ID == tile.ID && currentWorld.GetTile(1, x, y).Info != tile.Info)
        {
            currentWorld.SetTile(1, x, y, tile);
            UpdateChunk(x, y);
            return false;
        }

        if (currentWorld.HasTile(1, x, y)) return false;
        currentWorld.SetTile(1, x, y, tile);
        UpdateChunk(x, y);
        return true;
    }

    public bool SetWall(Int2 pos, Tile tile)
    {
        return SetWall(pos.x, pos.y, tile);
    }

    public bool SetWall(int x, int y, int id)
    {
        return SetWall(x, y, new Tile(id));
    }

    public bool SetWall(Int2 pos, int id)
    {
        return SetWall(pos.x, pos.y, new Tile(id));
    }

    public bool DeleteWall(int x, int y)
    {
        if (currentWorld.HasTile(0, x, y))
            return false;

        ushort blockId = currentWorld.GetTile(1, x, y).ID;
        if (blockId != 0)
            SimplePool.Spawn(pickup, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity).GetComponent<Pickup>()
                .Initialize(new Item(blockId), 1);

        currentWorld.DeleteTile(1, x, y);
        UpdateChunk(x, y);
        return true;
    }

    #endregion

    #region ForegroundManagement

    public bool SetForeground(int x, int y, Tile tile)
    {
        if (!currentWorld.HasTile(0, x, y) || currentWorld.HasTile(0, x, y + 1)) return false;

        currentWorld.SetTile(3, x, y, tile);
        UpdateChunk(x, y);
        return true;
    }

    public bool SetForeground(int x, int y, int id)
    {
        return SetForeground(x, y, new Tile(id));
    }

    public bool SetForeground(Int2 pos, Tile tile)
    {
        return SetForeground(pos.x, pos.y, tile);
    }

    public bool SetForeground(Int2 pos, int id)
    {
        return SetForeground(pos.x, pos.y, new Tile(id));
    }

    public bool DeleteForeGround(int x, int y)
    {
        ushort blockId = currentWorld.GetTile(3, x, y).ID;
        if (blockId != 0)
            SimplePool.Spawn(pickup, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity).GetComponent<Pickup>()
                .Initialize(new Item(blockId), 1);

        currentWorld.DeleteTile(0, x, y);
        UpdateChunk(x, y);
        return true;
    }

    public bool DeleteForeGround(Int2 pos)
    {
        return DeleteForeGround(pos.x, pos.y);
    }

    #endregion

    #region ChunkManagement

    public void UpdateChunk(int x, int y)
    {
        GameManager.Get<ChunkManager>().UpdateChunk(x, y);
    }

    public void UpdateChunk(Int2 pos)
    {
        UpdateChunk(pos.x, pos.y);
    }

    #endregion
}