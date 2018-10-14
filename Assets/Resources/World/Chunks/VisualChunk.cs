using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class VisualChunk : Chunk
{
    [SerializeField]
    private LightObstacleMesh lightMesh;

    [SerializeField]
    private MeshFilter foregroundFilter;
    [SerializeField]
    private MeshFilter frontBlockFilter;
    [SerializeField]
    private MeshFilter backBlockFilter;
    [SerializeField]
    private MeshFilter liquidFilter;
    [SerializeField]
    private MeshFilter wallFilter;

    private MeshInfo foregroundMeshInfo;
    private MeshInfo frontBlockMeshInfo;
    private MeshInfo backBlockMeshInfo;
    private MeshInfo liquidMeshInfo;
    private MeshInfo wallMeshInfo;

    void Awake()
    {
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
        foregroundMeshInfo = new MeshInfo(ref foregroundFilter, MeshData.GetForegroundMeshData);
        frontBlockMeshInfo = new MeshInfo(ref frontBlockFilter, MeshData.GetFrontBlockMeshData);
        backBlockMeshInfo = new MeshInfo(ref backBlockFilter, MeshData.GetBackBlockMeshData);
        liquidMeshInfo = new MeshInfo(ref liquidFilter, MeshData.GetLiquidMeshData);
        wallMeshInfo = new MeshInfo(ref wallFilter, MeshData.GetWallMeshData);
    }
    
    public override void UpdateChunk()
    {
        // Add mesh data for each entity in chunk
        for (int x = 0; x < ChunkManager.visualChunkSize; x++)
        {
            for (int y = 0; y < ChunkManager.visualChunkSize; y++)
            {

                ushort id = currentWorld.GetTile(3, x + pos.x, y + pos.y).ID;
                if (id != 0)
                {
                    foregroundMeshInfo.AddMeshData(ref currentWorld, new Int2(pos.x + x, pos.y + y), new Int2(x, y));
                }

                id = currentWorld.GetTile(0, x + pos.x, y + pos.y).ID;
                if (id != 0)
                {
                    if (id == 8001)
                    {
                        liquidMeshInfo.AddMeshData(ref currentWorld, new Int2(pos.x + x, pos.y + y), new Int2(x, y));
                    }
                    else
                    {
                        frontBlockMeshInfo.AddMeshData(ref currentWorld, new Int2(pos.x + x, pos.y + y), new Int2(x, y));
                        backBlockMeshInfo.AddMeshData(ref currentWorld, new Int2(pos.x + x, pos.y + y), new Int2(x, y));
                    }

                }
                id = currentWorld.GetTile(1, x + pos.x, y + pos.y).ID;
                if (id != 0)
                {
                    
                    wallMeshInfo.AddMeshData(ref currentWorld, new Int2(pos.x + x, pos.y + y), new Int2(x, y));
                }

            }
        }
        foregroundMeshInfo.ConstructMesh();
        frontBlockMeshInfo.ConstructMesh();
        backBlockMeshInfo.ConstructMesh();
        liquidMeshInfo.ConstructMesh();
        wallMeshInfo.ConstructMesh();
        
        lightMesh.Refresh();
        
    }
}

public class MeshInfo
{
    public MeshFilter filter;

    public List<Vector2> uvs;
    public List<Vector3> vertices;
    public List<int> triangles;

    public delegate void GetMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs);
    public GetMeshData getMeshData;

    public MeshInfo(ref MeshFilter meshFilter, GetMeshData getMeshDataFunction)
    {
        uvs = new List<Vector2>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        filter = meshFilter;
        getMeshData = getMeshDataFunction;
    }

    public void AddMeshData(ref World currentWorld, Int2 tilePos, Int2 offset)
    {
        // Populate vertices and uv array with data from entity
        List<Vector3> verts;
        List<Vector2> UVs;

        getMeshData(ref currentWorld, tilePos, offset, out verts, out UVs);

        vertices.AddRange(verts);
        uvs.AddRange(UVs);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }

    public void ConstructMesh()
    {
        GameObject.Destroy(filter.mesh);
        filter.mesh = new Mesh();
        filter.mesh.vertices = vertices.ToArray();
        filter.mesh.triangles = triangles.ToArray();
        filter.mesh.uv = uvs.ToArray();

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
}