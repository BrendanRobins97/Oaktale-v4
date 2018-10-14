using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpawnManager : MonoBehaviour
{

    public Dictionary<Int2, TileBreakManager> tileHealths = new Dictionary<Int2, TileBreakManager>();
    public GameObject tileBreakPrefab;
    public GameObject tileHighlightPrefab;

    private SpriteRenderer tileHighlight;
    private WorldManager WorldManager;
    void Awake()
    {
        GameManager.Set(this);
    }

    void Start()
    {
        WorldManager = GameManager.Get<WorldManager>();
    }
    private void Update()
    {
        List<Int2> positions = new List<Int2>(tileHealths.Keys);
        foreach (Int2 pos in positions)
        {
            if (tileHealths[pos].currentHealth > tileHealths[pos].maxHealth)
            {
                SimplePool.Despawn(tileHealths[pos].gameObject);
                tileHealths.Remove(pos);
            }
        }
    }

    public void MineBlock(Int2 pos, float power)
    {
        Tile block = WorldManager.currentWorld.GetTile(0, pos.x, pos.y);

        if (block.ID == 0) return;
        if (!tileHealths.ContainsKey(pos))
        {
            TileBreakManager tileBreak = SimplePool.Spawn(tileBreakPrefab, new Vector3(pos.x, pos.y, -30), Quaternion.identity).GetComponent<TileBreakManager>();
            tileBreak.maxHealth = ItemDatabase.GetBlock(block).health;
            tileBreak.currentHealth = ItemDatabase.GetBlock(block).health;
            tileBreak.position = pos;
            tileBreak.tileInfo = WorldManager.currentWorld.GetTile(0, pos).Info;
            //tileBreak.offset = block.Info;

            tileHealths.Add(pos, tileBreak);
        }
        tileHealths[pos].currentHealth -= power;
        if (tileHealths[pos].currentHealth <= 0)
        {
            GameManager.Get<WorldManager>().DeleteBlock(pos);
            SimplePool.Despawn(tileHealths[pos].gameObject);
            tileHealths.Remove(pos);
        }
    }
    public void MineBlock(int x, int y, float power)
    {
        MineBlock(new Int2(x, y), power);
    }

}

