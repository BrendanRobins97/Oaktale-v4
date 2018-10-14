// File: SpawnManager.cs
// Author: Brendan Robinson
// Date Created: 05/21/2018
// Date Last Modified: 07/31/2018
// Description: 

using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    private List<GameObject> spawns = new List<GameObject>();
    private PlayerInfo player;
    private static float maxDistance = 30f;

    private void Awake()
    {
        GameManager.Set(this);

    }

    private void Start()
    {
        player = GameManager.Get<PlayerInfo>();
    }
    private void Update()
    {
        for (int index = 0; index < spawns.Count; index++)
        {
            GameObject spawn = spawns[index];
            if (spawn == null)
            {
                spawns.Remove(spawn);
                continue;
            }
            if ((spawn.transform.position - player.transform.position).magnitude < maxDistance)
            {
                spawn.SetActive(true);
            }
            else
            {
                spawn.SetActive(false);
            }
        }
    }
    public void SpawnEnemy(Int2 pos, GameObject enemy)
    {
        Character character = enemy.GetComponent<Character>();
        int width = Mathf.CeilToInt(character.width);
        int height = Mathf.CeilToInt(character.height);
        //Int2 perimeter = new Int2(width, height);
        Int2 spawn = new Int2(pos.x - width / 2, pos.y - height / 2);
        /*
        while (currentWorld.HasBlock(spawn, spawn + perimeter))
        {
            spawn.y += height;
        }
        */
        spawns.Add(Instantiate(enemy, spawn.ToVector2(), Quaternion.identity));
    }

    public void SpawnPack(Int2 pos, GameObject enemy, int amount, int range)
    {
        Character character = enemy.GetComponent<Character>();
        int width = Mathf.CeilToInt(character.width);
        Int2 spawnLocation = pos;
        for (int i = pos.x; i < pos.x + range; i += range / amount)
        {
            spawnLocation.x = i + Random.Range(-2 - width, 2 + width);
            SpawnEnemy(spawnLocation, enemy);
        }
    }
}