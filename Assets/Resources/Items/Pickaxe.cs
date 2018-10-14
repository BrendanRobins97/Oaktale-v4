// File: Pickaxe.cs
// Author: Brendan Robinson
// Date Created: 01/06/2018
// Date Last Modified: 07/14/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : Tool
{
    public GameObject tileHighlightPrefab;
    public GameObject[] tileHighlights;
    public Sprite tileHighlightIcon;

    private float breakSpeed = 50f;
    private float directionalMineMultiplier = 1.25f;
    private int miningRange = 5;
    private Int2 currentMousePosition = Int2.zero;
    private Int2 oldMousePosition = Int2.zero;
    private List<Int2> blocksToMine = new List<Int2>();
    private int numHighlights = 5;

    protected override void Start()
    {
        base.Start();
        tileHighlights = new GameObject[numHighlights];
        for (int i = 0; i < numHighlights; i++)
            tileHighlights[i] = SimplePool.Spawn(tileHighlightPrefab, Vector3.zero, Quaternion.identity);
    }

    private void OnDisable()
    {
        for (int i = 0; i < numHighlights; i++) SimplePool.Despawn(tileHighlights[i]);
    }

    public override void PrimaryUse()
    {
        base.PrimaryUse();
        if (blocksToMine.Count > 1)
            foreach (Int2 tpos in blocksToMine)
                GameManager.Get<SpriteSpawnManager>().MineBlock(tpos,
                    breakSpeed * Time.deltaTime / blocksToMine.Count * directionalMineMultiplier);
        else if (blocksToMine.Count == 1)
            GameManager.Get<SpriteSpawnManager>()
                .MineBlock(blocksToMine[0], breakSpeed * Time.deltaTime / blocksToMine.Count);
    }

    protected override void Update()
    {
        blocksToMine.Clear();

        if (GameManager.Get<PlayerInfo>().directionalMining)
        {
            bool hitBlock = false;
            float directionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            float directionY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
            Int2 direction;
            Int2 currentBlockPos;
            int count = 0;
            Int2 blockToMine;
            if (Mathf.Abs(directionX) > Mathf.Abs(directionY))
            {
                direction = directionX > 0 ? new Int2(1, 0) : new Int2(-1, 0);
                while (!hitBlock && count <= miningRange)
                {
                    currentBlockPos = new Int2(transform.position) + direction * count;
                    for (int y = -1; y <= 1; y++)
                    {
                        blockToMine = new Int2(currentBlockPos.x, currentBlockPos.y + y);
                        if (GameManager.Get<WorldManager>().currentWorld.HasTile(0, blockToMine))
                        {
                            blocksToMine.Add(blockToMine);
                            hitBlock = true;
                        }
                    }

                    count++;
                }
            }
            else
            {
                direction = directionY > 0 ? new Int2(0, 1) : new Int2(0, -1);
                while (!hitBlock && count <= miningRange)
                {
                    currentBlockPos = new Int2((int)(transform.position.x - 0.5f), (int)transform.position.y) +
                                      direction * count;
                    for (int x = 0; x <= 1; x++)
                    {
                        blockToMine = new Int2(currentBlockPos.x + x, currentBlockPos.y);

                        if (GameManager.Get<WorldManager>().currentWorld
                            .HasTile(0, currentBlockPos.x + x, currentBlockPos.y))
                        {
                            blocksToMine.Add(blockToMine);
                            hitBlock = true;
                        }
                    }

                    count++;
                }
            }
        }
        else
        {
            int directionX = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x);
            int directionY = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y);

            if (Mathf.Abs(directionX) <= miningRange && Mathf.Abs(directionY) <= miningRange &&
                GameManager.Get<WorldManager>().currentWorld.GetTile(0, Utilities.GetMousePosition()).ID != 0)
                blocksToMine.Add(Utilities.GetMousePosition());
        }

        for (int i = 0; i < blocksToMine.Count; i++)
        {
            tileHighlights[i].GetComponent<SpriteRenderer>().sprite = tileHighlightIcon;
            tileHighlights[i].transform.position = new Vector3(blocksToMine[i].x + 0.5f, blocksToMine[i].y + 0.5f, -50);
        }

        for (int i = blocksToMine.Count; i < numHighlights; i++)
            tileHighlights[i].GetComponent<SpriteRenderer>().sprite = null;
    }
}