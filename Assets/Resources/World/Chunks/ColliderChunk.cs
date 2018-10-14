using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class ColliderChunk : Chunk
{
    private static float maxDistance = 100f;

    private Stack<Vector2[]> colPaths = new Stack<Vector2[]>();
    private List<Vector2> currentPath = new List<Vector2>();
    private PolygonCollider2D coll;
    private Transform player;

    // Use this for initialization
    void Awake()
    {
        coll = GetComponent<PolygonCollider2D>();
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
        player = GameManager.Get<PlayerInfo>().transform;
    }

    private void FixedUpdate()
    {
        /*
        if ((player.position - transform.position).magnitude >= maxDistance)
        {
            GameManager.Get<ChunkManager>().colliderChunkDictionary.Remove(Position);
            SimplePool.Despawn(gameObject);
        }
        */
    }

    public override void UpdateChunk()
    {

        for (int x = 0; x < ChunkManager.colliderChunkSize; x++)
        {
            for (int y = 0; y < ChunkManager.colliderChunkSize; y++)
            {

                if (currentWorld.HasTile(0, x + pos.x, y + pos.y))
                {
                    AddColliderData(ref currentWorld, x, y);
                }

            }
        }

        ConstructCollider();
    }

    private void AddColliderData(ref World world, int x, int y)
    {
        switch (currentWorld.GetTile(0, x + pos.x, y + pos.y).Info)
        {
            // Full
            case 0:
                currentPath.Add(new Vector2(x + 1, y));
                currentPath.Add(new Vector2(x, y));

                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 2) &&
                !currentWorld.HasTile(0, x + pos.x - 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.02f, y + 1));
                }
                else
                {
                    currentPath.Add(new Vector2(x, y + 1));
                }

                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 1) &&
                !currentWorld.HasTile(0, x + pos.x + 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.98f, y + 1));
                } else
                {
                    currentPath.Add(new Vector2(x + 1, y + 1));
                }
                break;
            case 1: // Bottom Left
                currentPath.Add(new Vector2(x + 1, y));
                currentPath.Add(new Vector2(x, y));
                
                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 2) &&
                !currentWorld.HasTile(0, x + pos.x - 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.02f, y + 1));
                } else
                {
                    currentPath.Add(new Vector2(x, y + 1));
                }
                break;
            case 2: // Bottom Right
                currentPath.Add(new Vector2(x + 1, y));
                currentPath.Add(new Vector2(x, y));

                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 1) &&
                !currentWorld.HasTile(0, x + pos.x + 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.98f, y + 1));
                } else
                {
                    currentPath.Add(new Vector2(x + 1, y + 1));
                }
                break;
            case 3: // Top Right
                currentPath.Add(new Vector2(x + 1, y));
                
                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 1) &&
                !currentWorld.HasTile(0, x + pos.x + 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.98f, y + 1));
                } else
                {
                    currentPath.Add(new Vector2(x + 1, y + 1));
                }
                currentPath.Add(new Vector2(x, y + 1));
                break;
            case 4: // Top Left
                currentPath.Add(new Vector2(x, y));

                if ((!currentWorld.HasTile(0, x + pos.x, y + pos.y + 1) ||
                currentWorld.GetTile(0, x + pos.x, y + pos.y + 1).Info == 2) &&
                !currentWorld.HasTile(0, x + pos.x - 1, y + pos.y))
                {
                    currentPath.Add(new Vector2(x + 0.02f, y + 1));
                } else
                {
                    currentPath.Add(new Vector2(x, y + 1));
                }
                currentPath.Add(new Vector2(x + 1, y + 1));
                break;
            case 5: // Bottom
                currentPath.Add(new Vector2(x + 1, y));
                currentPath.Add(new Vector2(x, y));
                currentPath.Add(new Vector2(x, y + 0.5f));
                currentPath.Add(new Vector2(x + 1, y + 0.5f));
                break;
            case 6: // Right
                currentPath.Add(new Vector2(x + 1, y));
                currentPath.Add(new Vector2(x + 0.5f, y));
                currentPath.Add(new Vector2(x + 0.5f, y + 1));
                currentPath.Add(new Vector2(x + 1, y + 1));
                break;
            case 7: // Top
                currentPath.Add(new Vector2(x + 1, y + 0.5f));
                currentPath.Add(new Vector2(x, y + 0.5f));
                currentPath.Add(new Vector2(x, y + 1));
                currentPath.Add(new Vector2(x + 1, y + 1));
                break;
            case 8: // Left
                currentPath.Add(new Vector2(x + 0.5f, y));
                currentPath.Add(new Vector2(x, y));
                currentPath.Add(new Vector2(x, y + 1));
                currentPath.Add(new Vector2(x + 0.5f, y + 1));
                break;
            default:
                currentPath.Add(new Vector2(x + 0.98f, y));
                currentPath.Add(new Vector2(x + 0.02f, y));
                currentPath.Add(new Vector2(x + 0.02f, y + 1));
                currentPath.Add(new Vector2(x + 0.98f, y + 1));
                break;
        }
        colPaths.Push(currentPath.ToArray());
        currentPath.Clear();
    }

    private bool InsideBlock(Vector2 position, int direction, int blockInfo, out Vector2 hitPoint)
    {
        hitPoint = position;
        switch (blockInfo)
        {
            case 1:
                if (position.x + position.y > 1)
                {
                    return false;
                }
                switch (direction)
                {
                    case 0:
                    default:
                        break;
                }
                break;
            case 2:
                if (position.y > position.x)
                {
                    return false;
                }
                break;
            default:
                break;
        }
        return true;
    }
    private void ConstructCollider()
    {
        int capacity = colPaths.Count;
        coll.pathCount = capacity;
        for (int i = 0; i < capacity; i++)
        {
            coll.SetPath(capacity - i - 1, colPaths.Pop());
        }
    }
    
}
