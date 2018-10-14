using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{

    // Global chunk size
    public static int visualChunkSize = 8;
    public static int colliderChunkSize = 4;
    public static int lightChunkSize = 16;

    // Reference to itself
    public static ChunkManager Instance;

    [SerializeField]
    private GameObject visualChunkPrefab;
    [SerializeField]
    private GameObject colliderChunkPrefab;
    [SerializeField]
    private GameObject lightChunkPrefab;

    [SerializeField]
    private Transform visualChunkContainer;
    [SerializeField]
    private Transform colliderChunkContainer;
    [SerializeField]
    private Transform lightChunkContainer;

    private Camera cam;

    [SerializeField]
    private int poolWidthBorder = 5;
    [SerializeField]
    private int poolHeightBorder = 4;

    private ChunkPool<VisualChunk> visualChunkPool;
    private ChunkPool<ColliderChunk> colliderChunkPool;
    private ChunkPool<LightChunk> lightChunkPool;

    private static int pixelsPerScreenPixel = 2;
    private static int pixelsPerTile = 12;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void Initialize()
    {
        cam = Camera.main;

        int poolWidth = Screen.width / pixelsPerScreenPixel / pixelsPerTile / visualChunkSize + poolWidthBorder;
        int poolHeight = Screen.height / pixelsPerScreenPixel / pixelsPerTile / visualChunkSize + poolHeightBorder;

        visualChunkPool = new ChunkPool<VisualChunk>(poolWidth, poolHeight, 0, poolWidth - 1, poolHeight - 1, 0, visualChunkPrefab, visualChunkContainer, visualChunkSize);
        visualChunkPool.prevCamPosition = RoundToChunkPos(new Int2((int)cam.transform.position.x, (int)cam.transform.position.y), visualChunkSize);

        poolWidth = Screen.width / pixelsPerScreenPixel / pixelsPerTile / colliderChunkSize + poolWidthBorder;
        poolHeight = Screen.height / pixelsPerScreenPixel / pixelsPerTile / colliderChunkSize + poolHeightBorder;

        colliderChunkPool = new ChunkPool<ColliderChunk>(poolWidth, poolHeight, 0, poolWidth - 1, poolHeight - 1, 0, colliderChunkPrefab, colliderChunkContainer, colliderChunkSize);
        colliderChunkPool.prevCamPosition = RoundToChunkPos(new Int2((int)cam.transform.position.x, (int)cam.transform.position.y), colliderChunkSize);


        GenerateChunks<VisualChunk>(ref visualChunkPool);
        GenerateChunks<ColliderChunk>(ref colliderChunkPool);
        
    }

    private void Update()
    {
        
        Int2 camPosition = RoundToChunkPos(new Int2(cam.transform.position), visualChunkSize);

        if (visualChunkPool.prevCamPosition.x != camPosition.x || visualChunkPool.prevCamPosition.y != camPosition.y)
        {
            MoveCamera((camPosition - visualChunkPool.prevCamPosition) / visualChunkSize, ref visualChunkPool);
            visualChunkPool.prevCamPosition = camPosition;
        }
        
        camPosition = RoundToChunkPos(new Int2(cam.transform.position), colliderChunkSize);

        if (colliderChunkPool.prevCamPosition.x != camPosition.x || colliderChunkPool.prevCamPosition.y != camPosition.y)
        {
            MoveCamera((camPosition - colliderChunkPool.prevCamPosition) / colliderChunkSize, ref colliderChunkPool);
            colliderChunkPool.prevCamPosition = camPosition;
        }
        
        
    }


    private void GenerateChunks<T>(ref ChunkPool<T> chunkPool) where T : Chunk
    {
        for (int x = 0; x < chunkPool.poolWidth; x++)
        {
            for (int y = 0; y < chunkPool.poolHeight; y++)
            {
                Int2 pos = new Int2((x - chunkPool.poolWidth / 2) * chunkPool.chunkSize + (int)cam.transform.position.x, (y - chunkPool.poolHeight / 2) * chunkPool.chunkSize + (int)cam.transform.position.y);
                chunkPool.chunkArray[x, y] = CreateChunk(pos, ref chunkPool);
            }
        }
    }

    private T CreateChunk<T>(Int2 pos, ref ChunkPool<T> chunkPool) where T : Chunk
    {

        T newChunk = Instantiate(chunkPool.prefab, new Vector2(pos.x, pos.y), Quaternion.identity, chunkPool.container).GetComponent<T>();
        chunkPool.dictionary.Add(pos, newChunk);
        newChunk.Position = pos;
        return newChunk;
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
                {
                    col = chunkPool.leftCol;
                }
                else
                {
                    col = chunkPool.rightCol;
                }

                for (int y = 0; y < chunkPool.poolHeight; y++)
                {
                    chunk = chunkPool.chunkArray[col, y];
                    chunkPool.dictionary.Remove(chunk.Position);
                    chunk.transform.Translate(new Vector3(moveDirection * chunkPool.poolWidth * chunkPool.chunkSize, 0, 0));

                    chunk.Position = new Int2(chunk.transform.position);
                    chunkPool.dictionary.Add(chunk.Position, chunk);

                }

                chunkPool.leftCol = Utilities.Mod((moveDirection + chunkPool.leftCol), chunkPool.poolWidth);
                chunkPool.rightCol = Utilities.Mod((moveDirection + chunkPool.rightCol), chunkPool.poolWidth);
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
                {
                    row = chunkPool.botRow;
                }
                else
                {
                    row = chunkPool.topRow;
                }

                for (int x = 0; x < chunkPool.poolWidth; x++)
                {
                    chunk = chunkPool.chunkArray[x, row];
                    chunkPool.dictionary.Remove(chunk.Position);
                    chunk.transform.Translate(new Vector3(0, moveDirection * chunkPool.poolHeight * chunkPool.chunkSize, 0));

                    chunk.Position = new Int2(chunk.transform.position);
                    chunkPool.dictionary.Add(chunk.Position, chunk);

                }

                chunkPool.topRow = Utilities.Mod((moveDirection + chunkPool.topRow), chunkPool.poolHeight);
                chunkPool.botRow = Utilities.Mod((moveDirection + chunkPool.botRow), chunkPool.poolHeight);
            }
        }
    }

    public void UpdateChunk(Int2 pos)
    {
        UpdateChunk(pos.x, pos.y);
    }
    public void UpdateChunk(int x, int y)
    {
        UpdateChunk(x, y, visualChunkPool);
        UpdateChunk(x, y, colliderChunkPool);
        //UpdateChunk(x, y, lightChunkPool);
    }

    private void UpdateChunk<T>(int x, int y, ChunkPool<T> chunkPool) where T : Chunk
    {
        T chunk;

        Int2 pos = RoundToChunkPos(new Int2(x, y), chunkPool.chunkSize);
        chunkPool.dictionary.TryGetValue(pos, out chunk);
        if (chunk)
        {
            chunk.UpdateChunk();
        }
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

        public ChunkPool(int poolWidth, int poolHeight, int leftCol, int rightCol, int topRow, int botRow, GameObject prefab, Transform container, int chunkSize = 8)
        {
            this.poolWidth = poolWidth;
            this.poolHeight = poolHeight;
            this.leftCol = leftCol;
            this.rightCol = rightCol;
            this.topRow = topRow;
            this.botRow = botRow;

            
            this.prefab = prefab;
            this.container = container;
            this.chunkSize = chunkSize;

            prevCamPosition = Int2.zero;
            chunkArray = new T[poolWidth, poolHeight];
            dictionary = new Dictionary<Int2, T>();
        }
    }
}
