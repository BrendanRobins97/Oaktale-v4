// File: ChunkManager.cs
// Author: Brendan Robinson
// Date Created: 01/13/2018
// Date Last Modified: 07/18/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    // Global chunk size
    public static int visualChunkSize = 8;
    public static int colliderChunkSize = 2;
    public static int lightChunkSize = 16;

    private static int pixelsPerScreenPixel = 2;
    private static int pixelsPerTile = 12;

    public Dictionary<Int2, ColliderChunk> colliderChunkDictionary = new Dictionary<Int2, ColliderChunk>();
    public Dictionary<Int2, byte> colliderDependencies = new Dictionary<Int2, byte>();
    public Dictionary<Int2, PolygonCollider2D> colliderObjs = new Dictionary<Int2, PolygonCollider2D>();

    [SerializeField] private GameObject visualChunkPrefab;

    [SerializeField] private GameObject colliderChunkPrefab;

    [SerializeField] private GameObject lightChunkPrefab;

    private Transform visualChunkContainer;

    private Transform colliderChunkContainer;

    private Transform lightChunkContainer;

    [SerializeField] private Camera cam;

    [SerializeField] private int poolWidthBorder = 5;

    [SerializeField] private int poolHeightBorder = 4;

    private ChunkPool<VisualChunk> visualChunkPool;
    private ChunkPool<ColliderChunk> colliderChunkPool;
    private ChunkPool<LightChunk> lightChunkPool;

    private World currentWorld;

    private void Awake()
    {
        GameManager.Set(this);
        pixelsPerScreenPixel = (int)(45f / Camera.main.orthographicSize);
        visualChunkContainer = GameObject.FindGameObjectWithTag("Container").transform;
        colliderChunkContainer = GameObject.FindGameObjectWithTag("Container").transform;
        lightChunkContainer = GameObject.FindGameObjectWithTag("Container").transform;
        SimplePool.Preload(colliderChunkPrefab, colliderChunkContainer, 2000);

    }

    private void Start()
    {
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
        Initialize();
    }
    public void UpdateCollider(Int2 pos)
    {
        PolygonCollider2D collider;
        colliderObjs.TryGetValue(pos, out collider);
        //currentWorld = GameManager.Get<WorldManager>().currentWorld;
        if (collider != null)
        {
            if (!currentWorld.HasTile(0, pos.x, pos.y))
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = true;
                if (currentWorld.GetTile(0, pos.x, pos.y).ID == ItemID.WoodPlatform)
                {
                    collider.gameObject.layer = 18;
                    Vector2[] currentPath = new Vector2[4];
                    currentPath[0] = new Vector2(1, 0.98f);
                    currentPath[1] = new Vector2(0, 0.98f);
                    currentPath[2] = new Vector2(0, 1);
                    currentPath[3] = new Vector2(1, 1);
                    collider.SetPath(0, currentPath);
                }
                else
                {
                    collider.gameObject.layer = 20;

                    switch (currentWorld.GetTile(0, pos.x, pos.y).Info)
                    {
                        // Full
                        case 0:
                            Vector2[] currentPath0 = new Vector2[4];

                            currentPath0[0] = new Vector2(1, 0);
                            currentPath0[1] = new Vector2(0, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 2) &&
                                !currentWorld.HasTile(0, pos.x - 1, pos.y))
                                currentPath0[2] = new Vector2(0.02f, 1);
                            else
                                currentPath0[2] = new Vector2(0, 1);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 1) &&
                                !currentWorld.HasTile(0, pos.x + 1, pos.y))
                                currentPath0[3] = new Vector2(0.98f, 1);
                            else
                                currentPath0[3] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath0);
                            break;
                        case 1: // Bottom Left
                            Vector2[] currentPath1 = new Vector2[3];

                            currentPath1[0] = new Vector2(1, 0);
                            currentPath1[1] = new Vector2(0, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 2) &&
                                !currentWorld.HasTile(0, pos.x - 1, pos.y))
                                currentPath1[2] = new Vector2(0.02f, 1);
                            else
                                currentPath1[2] = new Vector2(0, 1);
                            collider.SetPath(0, currentPath1);
                            break;
                        case 2: // Bottom Right
                            Vector2[] currentPath2 = new Vector2[3];

                            currentPath2[0] = new Vector2(1, 0);
                            currentPath2[1] = new Vector2(0, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 1) &&
                                !currentWorld.HasTile(0, pos.x + 1, pos.y))
                                currentPath2[2] = new Vector2(0.98f, 1);
                            else
                                currentPath2[2] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath2);
                            break;
                        case 3: // Top Right
                            Vector2[] currentPath3 = new Vector2[3];

                            currentPath3[0] = new Vector2(1, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 1) &&
                                !currentWorld.HasTile(0, pos.x + 1, pos.y))
                                currentPath3[1] = new Vector2(0.98f, 1);
                            else
                                currentPath3[1] = new Vector2(1, 1);
                            currentPath3[2] = new Vector2(0, 1);
                            collider.SetPath(0, currentPath3);
                            break;
                        case 4: // Top Left
                            Vector2[] currentPath4 = new Vector2[3];

                            currentPath4[0] = new Vector2(0, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 2) &&
                                !currentWorld.HasTile(0, pos.x - 1, pos.y))
                                currentPath4[1] = new Vector2(0.02f, 1);
                            else
                                currentPath4[2] = new Vector2(0, 1);
                            currentPath4[2] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath4);
                            break;
                        case 5: // Bottom
                            Vector2[] currentPath5 = new Vector2[4];

                            currentPath5[0] = new Vector2(1, 0);
                            currentPath5[1] = new Vector2(0, 0);
                            currentPath5[2] = new Vector2(0, 0.5f);
                            currentPath5[3] = new Vector2(1, 0.5f);
                            collider.SetPath(0, currentPath5);
                            break;
                        case 6: // Right
                            Vector2[] currentPath6 = new Vector2[4];

                            currentPath6[0] = new Vector2(1, 0);
                            currentPath6[1] = new Vector2(0.5f, 0);
                            currentPath6[2] = new Vector2(0.5f, 1);
                            currentPath6[3] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath6);
                            break;
                        case 7: // Top
                            Vector2[] currentPath7 = new Vector2[4];

                            currentPath7[0] = new Vector2(1, 0.5f);
                            currentPath7[1] = new Vector2(0, 0.5f);
                            currentPath7[2] = new Vector2(0, 1);
                            currentPath7[3] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath7);
                            break;
                        case 8: // Left
                            Vector2[] currentPath8 = new Vector2[4];

                            currentPath8[0] = new Vector2(0.5f, 0);
                            currentPath8[1] = new Vector2(0, 0);
                            currentPath8[2] = new Vector2(0, 1);
                            currentPath8[3] = new Vector2(0.5f, 1);
                            collider.SetPath(0, currentPath8);
                            break;
                        default:
                            Vector2[] currentPath9 = new Vector2[4];

                            currentPath9[0] = new Vector2(1, 0);
                            currentPath9[1] = new Vector2(0, 0);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 2) &&
                                !currentWorld.HasTile(0, pos.x - 1, pos.y))
                                currentPath9[2] = new Vector2(0.02f, 1);
                            else
                                currentPath9[2] = new Vector2(0, 1);

                            if ((!currentWorld.HasTile(0, pos.x, pos.y + 1) ||
                                 currentWorld.GetTile(0, pos.x, pos.y + 1).Info == 1) &&
                                !currentWorld.HasTile(0, pos.x + 1, pos.y))
                                currentPath9[3] = new Vector2(0.98f, 1);
                            else
                                currentPath9[3] = new Vector2(1, 1);
                            collider.SetPath(0, currentPath9);
                            break;
                    }
                }
            }

            collider.transform.position = pos.ToVector2();
        }
    }

    public void DeleteCollider(Int2 pos)
    {
        if (colliderDependencies.ContainsKey(pos))
        {
            colliderDependencies[pos] -= 1;
            if (colliderDependencies[pos] <= 0)
            {
                SimplePool.Despawn(colliderObjs[pos].gameObject);
                colliderObjs.Remove(pos);
                colliderDependencies.Remove(pos);
            }
        }
    }

    public void AddCollider(Int2 pos)
    {
        if (colliderDependencies.ContainsKey(pos))
        {
            colliderDependencies[pos] += 1;
        }
        else
        {
            colliderObjs.Add(pos,
                SimplePool.Spawn(colliderChunkPrefab, pos.ToVector2(), Quaternion.identity)
                    .GetComponent<PolygonCollider2D>());
            UpdateCollider(pos);
            colliderDependencies.Add(pos, 1);
        }
    }

    public void Initialize()
    {
        int poolWidth, poolHeight;

        poolWidth = Screen.width / pixelsPerScreenPixel / pixelsPerTile / visualChunkSize + 2 * poolWidthBorder;
        poolHeight = Screen.height / pixelsPerScreenPixel / pixelsPerTile / visualChunkSize + 2 * poolHeightBorder;

        visualChunkPool =
            new ChunkPool<VisualChunk>(poolWidth, poolHeight, visualChunkPrefab, visualChunkContainer,
                visualChunkSize)
            { prevCamPosition = RoundToChunkPos(new Int2(cam.transform.position), visualChunkSize) };


        poolWidth = Screen.width / pixelsPerScreenPixel / pixelsPerTile / lightChunkSize + 2 * poolWidthBorder + 2;
        poolHeight = Screen.height / pixelsPerScreenPixel / pixelsPerTile / lightChunkSize + 2 * poolHeightBorder + 2;

        lightChunkPool =
            new ChunkPool<LightChunk>(poolWidth, poolHeight, lightChunkPrefab, lightChunkContainer, lightChunkSize)
            {
                prevCamPosition = RoundToChunkPos(new Int2(cam.transform.position), lightChunkSize)
            };

        GenerateChunks(ref visualChunkPool);
        GenerateChunks(ref lightChunkPool);
    }

    public void ResetPools()
    {
        foreach (VisualChunk chunk in visualChunkPool.chunkArray) chunk.UpdateWorld();
        foreach (LightChunk chunk in lightChunkPool.chunkArray) chunk.UpdateWorld();

        visualChunkPool.prevCamPosition = RoundToChunkPos(new Int2(cam.transform.position), visualChunkSize);
        lightChunkPool.prevCamPosition = RoundToChunkPos(new Int2(cam.transform.position), lightChunkSize);

        GenerateChunks(ref visualChunkPool);
        GenerateChunks(ref lightChunkPool);
    }

    private void Update()
    {
        Int2 camPosition = RoundToChunkPos(new Int2(cam.transform.position), visualChunkSize);

        if (visualChunkPool.prevCamPosition.x != camPosition.x || visualChunkPool.prevCamPosition.y != camPosition.y)
        {
            MoveCamera((camPosition - visualChunkPool.prevCamPosition) / visualChunkSize, ref visualChunkPool);
            visualChunkPool.prevCamPosition = camPosition;
        }

        camPosition = RoundToChunkPos(new Int2(cam.transform.position), lightChunkSize);

        if (lightChunkPool.prevCamPosition.x != camPosition.x || lightChunkPool.prevCamPosition.y != camPosition.y)
        {
            MoveCamera((camPosition - lightChunkPool.prevCamPosition) / lightChunkSize, ref lightChunkPool);
            lightChunkPool.prevCamPosition = camPosition;
        }
    }

    private void GenerateChunks<T>(ref ChunkPool<T> chunkPool) where T : Chunk
    {
        chunkPool.ResetRows();
        chunkPool.dictionary.Clear();
        for (int x = 0; x < chunkPool.poolWidth; x++)
            for (int y = 0; y < chunkPool.poolHeight; y++)
            {
                Int2 camPosition = RoundToChunkPos(new Int2(cam.transform.position), chunkPool.chunkSize);
                Int2 pos = new Int2((x - chunkPool.poolWidth / 2) * chunkPool.chunkSize,
                    (y - chunkPool.poolHeight / 2) * chunkPool.chunkSize);
                pos += camPosition;
                chunkPool.dictionary.Add(pos, chunkPool.chunkArray[x, y]);
                chunkPool.chunkArray[x, y].transform.position = new Vector2(pos.x, pos.y);
                chunkPool.chunkArray[x, y].Position = pos;
            }
    }

    public void UpdateChunk(Int2 pos)
    {
        UpdateChunk(pos.x, pos.y);
    }

    public void UpdateChunk(int x, int y)
    {
        UpdateChunk(x, y, visualChunkPool);
        if (x % visualChunkSize == 0) UpdateChunk(x - 1, y, visualChunkPool);
        if (x % visualChunkSize == visualChunkSize - 1) UpdateChunk(x + 1, y, visualChunkPool);
        if (y % visualChunkSize == 0) UpdateChunk(x, y - 1, visualChunkPool);
        if (y % visualChunkSize == visualChunkSize - 1) UpdateChunk(x, y + 1, visualChunkPool);
        UpdateChunk(x, y, lightChunkPool);

        UpdateCollider(new Int2(x, y));
    }

    public void UpdateColliderChunk(int x, int y)
    {
        ColliderChunk chunk;
        Int2 pos = RoundToChunkPos(new Int2(x, y), colliderChunkPool.chunkSize);
        colliderChunkDictionary.TryGetValue(pos, out chunk);
        if (chunk) chunk.UpdateChunk();
        else
            Debug.Log("not here");
    }

    private void UpdateChunk<T>(int x, int y, ChunkPool<T> chunkPool) where T : Chunk
    {
        T chunk;
        // Find the chunk that the position is located in and update it if it exists
        Int2 pos = RoundToChunkPos(new Int2(x, y), chunkPool.chunkSize);
        chunkPool.dictionary.TryGetValue(pos, out chunk);
        if (chunk) chunk.UpdateChunk();
        else
            Debug.Log("not here");
    }

    private Int2 RoundToChunkPos(Int2 pos, int chunkSize)
    {
        return new Int2(pos.x / chunkSize * chunkSize, pos.y / chunkSize * chunkSize);
    }

    private struct ChunkPool<T> where T : Chunk
    {
        public T[,] chunkArray;
        public Dictionary<Int2, T> dictionary;

        public int poolWidth;
        public int poolHeight;
        public int leftCol;
        public int rightCol;
        public int topRow;
        public int botRow;

        public GameObject prefab;
        public Transform container;
        public int chunkSize;

        public Int2 prevCamPosition;

        public ChunkPool(int poolWidth, int poolHeight, GameObject prefab, Transform container, int chunkSize = 8)
        {
            this.poolWidth = poolWidth;
            this.poolHeight = poolHeight;
            leftCol = 0;
            rightCol = poolWidth - 1;
            topRow = poolHeight - 1;
            botRow = 0;

            this.prefab = prefab;
            this.container = container;
            this.chunkSize = chunkSize;

            prevCamPosition = Int2.zero;
            chunkArray = new T[poolWidth, poolHeight];

            for (int x = 0; x < poolWidth; x++)
                for (int y = 0; y < poolHeight; y++)
                    chunkArray[x, y] = Instantiate(prefab, Vector2.zero, Quaternion.identity, container).GetComponent<T>();
            dictionary = new Dictionary<Int2, T>();
        }

        public void ResetRows()
        {
            leftCol = 0;
            rightCol = poolWidth - 1;
            topRow = poolHeight - 1;
            botRow = 0;
        }
    }

    #region CameraMovement

    private void MoveColliderCamera()
    {
        Int2 camPosition = RoundToChunkPos(new Int2(cam.transform.position), colliderChunkPool.chunkSize);
        ColliderChunk chunk;
        Int2 pos;

        for (int x = 0; x < colliderChunkPool.poolWidth; x++)
            for (int y = 0; y < colliderChunkPool.poolHeight; y++)
            {
                pos = new Int2((x - colliderChunkPool.poolWidth / 2) * colliderChunkPool.chunkSize,
                    (y - colliderChunkPool.poolHeight / 2) * colliderChunkPool.chunkSize);
                pos += camPosition;
                if (!colliderChunkDictionary.ContainsKey(pos))
                {
                    chunk = SimplePool.Spawn(colliderChunkPrefab, pos.ToVector2(), Quaternion.identity)
                        .GetComponent<ColliderChunk>();
                    colliderChunkDictionary.Add(pos, chunk);
                    chunk.transform.position = new Vector2(pos.x, pos.y);
                    chunk.Position = pos;
                }
            }
    }

    private void MoveCamera<T>(Int2 dist, ref ChunkPool<T> chunkPool) where T : Chunk
    {
        if (dist.x != 0)
        {
            T chunk;
            int col;

            int moveDirection = (int)Mathf.Sign(dist.x);

            for (int x = 0; x < Mathf.Abs(dist.x); x++)
            {
                if (moveDirection == 1)
                    col = chunkPool.leftCol;
                else
                    col = chunkPool.rightCol;

                for (int y = 0; y < chunkPool.poolHeight; y++)
                {
                    chunk = chunkPool.chunkArray[col, y];
                    chunkPool.dictionary.Remove(chunk.Position);
                    chunk.transform.Translate(new Vector3(moveDirection * chunkPool.poolWidth * chunkPool.chunkSize, 0,
                        0));

                    chunk.Position = new Int2(chunk.transform.position);
                    chunkPool.dictionary.Add(chunk.Position, chunk);
                }

                chunkPool.leftCol = Utilities.Mod(moveDirection + chunkPool.leftCol, chunkPool.poolWidth);
                chunkPool.rightCol = Utilities.Mod(moveDirection + chunkPool.rightCol, chunkPool.poolWidth);
            }
        }

        if (dist.y != 0)
        {
            T chunk;
            int row;

            int moveDirection = (int)Mathf.Sign(dist.y);

            for (int y = 0; y < Mathf.Abs(dist.y); y++)
            {
                if (moveDirection == 1)
                    row = chunkPool.botRow;
                else
                    row = chunkPool.topRow;

                for (int x = 0; x < chunkPool.poolWidth; x++)
                {
                    chunk = chunkPool.chunkArray[x, row];
                    chunkPool.dictionary.Remove(chunk.Position);
                    chunk.transform.Translate(new Vector3(0, moveDirection * chunkPool.poolHeight * chunkPool.chunkSize,
                        0));

                    chunk.Position = new Int2(chunk.transform.position);
                    chunkPool.dictionary.Add(chunk.Position, chunk);
                }

                chunkPool.topRow = Utilities.Mod(moveDirection + chunkPool.topRow, chunkPool.poolHeight);
                chunkPool.botRow = Utilities.Mod(moveDirection + chunkPool.botRow, chunkPool.poolHeight);
            }
        }
    }

    #endregion
}