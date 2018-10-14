using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Liquid", menuName = "Liquid", order = 1)]
public class LiquidData : ItemData {

    public void GetLiquidMeshData(ref World world, Int2 pos, Int2 offset, out List<Vector3> vertices, out List<Vector2> UVs)
    {
        List<Vector3> verts = new List<Vector3>(4);

        List<Vector2> uvs = new List<Vector2>(4);

        Tile tileAbove = world.GetTile(4, pos.x, pos.y + 1);
        float liquidAmount;
        if (tileAbove.ID == 8001 && tileAbove.Info > 0)
        {
            liquidAmount = 1f;

        }
        else
        {
            liquidAmount = (float)world.GetTile(4, pos).Info / byte.MaxValue;

        }

        verts.Add(new Vector3(1f + offset.x, offset.y, - .001f + pos.x / world.Width));
        verts.Add(new Vector3(offset.x, offset.y, - .001f + pos.x / world.Width));
        verts.Add(new Vector3(offset.x, liquidAmount + offset.y, - .001f + pos.x / world.Width));
        verts.Add(new Vector3(1f + offset.x, liquidAmount + offset.y, - .001f + pos.x / world.Width));

        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, liquidAmount));
        uvs.Add(new Vector2(1, liquidAmount));

        // return the out variables
        UVs = uvs;
        vertices = verts;

    }
}
