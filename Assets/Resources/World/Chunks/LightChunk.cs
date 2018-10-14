using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LightChunk : Chunk
{
    // Lists used for mesh data
    private List<Vector2> uv = new List<Vector2>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter filter;

    public void Awake()
    {
        filter = gameObject.GetComponent<MeshFilter>();
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
    }

    public override void UpdateChunk()
    {
        for (int x = 0; x < ChunkManager.lightChunkSize; x++)
        {
            int startY = 0;
            int endY;
            for (int y = 0; y < ChunkManager.lightChunkSize; y++)
            {
                endY = y + 1;
                if (currentWorld.GetTile(1, x + pos.x, y + pos.y).ID != 0)
                {
                    startY = endY;

                }
                else
                {
                    if (currentWorld.GetTile(1, x + pos.x, y + 1 + pos.y).ID != 0 || endY == ChunkManager.lightChunkSize)
                    {
                        AddBox(x, startY, endY);
                    }
                }

            }
        }
        ConstructMesh();
    }

    private void AddBox(int x, int startY, int endY)
    {
        // Populate vertices and uv array with data from entity
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();

        verts.Add(new Vector3(x, startY));
        verts.Add(new Vector3(x, endY));
        verts.Add(new Vector3(x + 1f, endY));
        verts.Add(new Vector3(x + 1f, startY));

        UVs.Add(new Vector2(0, 0));
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(1, 0));


        vertices.AddRange(verts);
        uv.AddRange(UVs);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
    }

    // Constructs mesh from all submeshes and links materials to each submesh
    private void ConstructMesh()
    {
        // Create a new mesh
        Destroy(filter.mesh);
        filter.mesh = new Mesh();
        filter.mesh.SetVertices(vertices);
        filter.mesh.SetUVs(0, uv);
        filter.mesh.SetTriangles(triangles.ToArray(), 0);

        // Clear lists
        vertices.Clear();
        uv.Clear();
        triangles.Clear();
    }
    
}
