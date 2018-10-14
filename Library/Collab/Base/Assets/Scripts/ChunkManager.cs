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

    private Chunk[,] chunkArray;

    private Camera cam;
    private Int2 prevCamPosition;

    private int poolWidth;
    private int poolHeight;

    [SerializeField]
    private int poolWidthBorder = 3;
    [SerializeField]
    private int poolHeightBorder = 2;

    private int leftCol;
    private int rightCol;
    private int topRow;
    private int botRow;

    private Int2 mapOffset;

    private World currentWorld;

    private Dictionary<Int2, VisualChunk> visualChunks = new Dictionary<Int2, VisualChunk>();
    private int pixelsPerScreenPixel = 2;
    private int pixelsPerTile = 12;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        GetWorld();
        GenerateChunks<VisualChunk>(visualChunkSize, visualChunkPrefab, visualChunkContainer, ref visualChunks);
    }

    private void GetWorld()
    {
        currentWorld = WorldManager.Instance.currentWorld;
    }

    private void GenerateChunks<T>(int chunkSize, GameObject prefab, Transform container, ref Dictionary<Int2, T> dictionary) where T : Chunk
    {
        for (int x = 0; x < currentWorld.width / chunkSize; x++)
        {
            for (int y = 0; y < currentWorld.height / chunkSize; y++)
            {
                CreateChunk(new Int2(x * chunkSize, y * chunkSize), ref dictionary);
            }
        }
    }

    public Chunk CreateChunk<T>(Int2 pos, ref Dictionary<Int2, T> dictionary) where T : Chunk
    {

        T newChunk = Instantiate(visualChunkPrefab, new Vector2(pos.x, pos.y), Quaternion.identity).GetComponent<T>();
        dictionary.Add(pos, newChunk);
        
        newChunk.Position = pos;

        return newChunk;
    }
}
