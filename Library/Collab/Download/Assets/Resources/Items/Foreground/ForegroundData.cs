using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Foreground", menuName = "Foreground", order = 1)]
public class ForegroundData : ItemData {

    public static readonly float tileSize = 12f;
    public static readonly float textureHeight = 2304f;
    public static readonly float textureWidth = 480f;

    public int layer;

    public void GetForegroundMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        List<Vector3> verts = new List<Vector3>(4);

        List<Vector2> uvs = new List<Vector2>(4);

        int randomOffSetX = world.GetForeground(pos).variation;
        randomOffSetX += 4 * world.GetForeground(pos).info;
        randomOffSetX += Random.Range(0, 4);
        // Add vertices
        verts.Add(new Vector3(1f + offset.x, offset.y - 24 / tileSize, -layer - 12 + pos.x / world.width));
        verts.Add(new Vector3(offset.x, offset.y - 24 / tileSize, -layer - 12 + pos.x / world.width));
        verts.Add(new Vector3(offset.x, offset.y  + 12 / tileSize, -layer - 12 + pos.x / world.width));
        verts.Add(new Vector3(1f + offset.x, offset.y  + 12 / tileSize, -layer - 12 + pos.x / world.width));

        int index = (id - 1000) + 0;
        // Add uv's
        uvs.Add(new Vector2(((1 + randomOffSetX) * 12) / textureWidth,
                            (textureHeight - 36 * index) / textureHeight));
        uvs.Add(new Vector2((randomOffSetX * 12) / textureWidth,
                            (textureHeight - 36 * index) / textureHeight));
        uvs.Add(new Vector2((randomOffSetX * 12) / textureWidth,
                            (textureHeight - 36 * (index - 1)) / textureHeight));
        uvs.Add(new Vector2(((1 + randomOffSetX) * 12) / textureWidth,
                            (textureHeight - 36 * (index - 1)) / textureHeight));


        // return the out variables
        UVs = uvs;
        vertices = verts;
    }
}
