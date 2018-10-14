// File: TerrainGenerator.cs
// Author: Brendan Robinson
// Date Created: 01/01/2018
// Date Last Modified: 08/03/2018
// Description: 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voronoi_Delaunay;

public class TerrainGenerator : MonoBehaviour
{
    #region World Generation

    private World world;

    private void Awake()
    {
        GameManager.Set(this);
    }

    public void GenerateNormalWorld()
    {
        ClearWorld();

        GenerateTerrain();
        GenerateCave();
        FlattenTerrain();
        SetGrass();
    }

    public void SetWorld(World world)
    {
        this.world = world;
    }

    public World GenerateGoodDungeon()
    {
        int roomGenerationRadius = 30;
        Int2 roomSize;
        Vector2 roomPoint;

        Collections.allRooms = new List<Room>();
        for (int i = 0; i < 150; i++)
        {
            roomSize = new Int2((int)Utilities.NormalizedRandom(18, 48), (int)Utilities.NormalizedRandom(12, 36));
            roomPoint = Utilities.RandomPointInCircle(roomGenerationRadius);

            Collections.allRooms.Add(new Room(roomPoint, roomSize));
        }

        for (int i = 0; i < 5; i++) SeparateRooms();

        int roomCount = Collections.allRooms.Count;
        for (int i = 0; i < roomCount; i++)
            for (int j = i + 1; j < roomCount; j++)
            {
                if (Collections.allRooms[i] == null || Collections.allRooms[j] == null) continue;

                if (Collections.allRooms[i].TooCloseTo(Collections.allRooms[j])) Collections.allRooms[i] = null;
            }

        Collections.allRooms.RemoveAll(room => room == null);
        Collections.allRooms.RemoveAll(room => room.size.x < 24 || room.size.y < 16);


        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

        foreach (Room room in Collections.allRooms)
        {
            if (room.position.x < minX) minX = (int)room.position.x;
            if (room.position.y < minY) minY = (int)room.position.y;
            if (room.position.x > maxX) maxX = (int)room.position.x;
            if (room.position.y > maxY) maxY = (int)room.position.y;
        }

        int border = 128;
        int width = maxX - minX + 2 * border;
        int height = maxY - minY + 2 * border;
        Int2 offset = new Int2(width / 2, height / 2);
        World newWorld = new World("Dungeon", width, height);
        world = newWorld;

        Collections.allRooms.ForEach(x => x.position += offset.ToVector2());

        Collections.allRooms = Collections.allRooms.OrderBy(point => point.position.x).ToList();
        Triangle superTriangle = Delaunay.SuperTriangle(Collections.allRooms);
        List<Triangle> delauneyTriangles = Delaunay.Triangulate(superTriangle, Collections.allRooms);
        List<DelaunayEdge> delaunayEdges = Delaunay.DelaunayEdges(superTriangle, delauneyTriangles);

        Graph<int> W = new Graph<int>(Enumerable.Range(0, delaunayEdges.Count));

        foreach (DelaunayEdge edge in delaunayEdges)
            W.SetEdge(edge.start, edge.end,
                (int)Mathf.Sqrt(
                    (Collections.allRooms[edge.start].position.x -
                     Collections.allRooms[edge.end].position.x) *
                    (Collections.allRooms[edge.start].position.x -
                     Collections.allRooms[edge.end].position.x) +
                    (Collections.allRooms[edge.start].position.y -
                     Collections.allRooms[edge.end].position.y) *
                    (Collections.allRooms[edge.start].position.y -
                     Collections.allRooms[edge.end].position.y)));

        Prim<int> prim = new Prim<int>();
        Dictionary<Graph<int>.Node, Graph<int>.Node> tree = new Dictionary<Graph<int>.Node, Graph<int>.Node>();
        prim.prim(W, W.FindVertex(0), ref tree);

        Tile tile = new Tile(12);
        StaticWorld(tile, tile);

        foreach (Room room in Collections.allRooms)
            Rectangle((int)(room.position.x - room.size.x / 2f), (int)(room.position.y - room.size.y / 2f),
                room.size.x, room.size.y);

        foreach (KeyValuePair<Graph<int>.Node, Graph<int>.Node> kv in tree)
        {
            //Debug.DrawLine(Collections.allRooms[kv.Key.context].position,
            //Collections.allRooms[kv.Value.context].position, Color.red, 100f);

            Int2 startPoint = new Int2((int)Collections.allRooms[kv.Key.context].position.x,
                (int)Collections.allRooms[kv.Key.context].position.y);
            Int2 endPoint = new Int2((int)Collections.allRooms[kv.Value.context].position.x,
                (int)Collections.allRooms[kv.Value.context].position.y);

            Hallway(startPoint, endPoint);
        }


        return newWorld;
    }

    private void Hallway(Int2 startPoint, Int2 endPoint)
    {
        Int2 currentPoint = startPoint;

        int xDirection = (int)Mathf.Sign(endPoint.x - startPoint.x);
        while (currentPoint.x != endPoint.x)
        {
            Rectangle(currentPoint.x, currentPoint.y, 4, 4);
            currentPoint.x += xDirection;
        }

        int yDirection = (int)Mathf.Sign(endPoint.y - startPoint.y);
        while (currentPoint.y != endPoint.y)
        {
            Rectangle(currentPoint.x, currentPoint.y, 4, 4);
            currentPoint.y += yDirection;
        }
    }

    private bool SeparateRooms()
    {
        bool changed = false;
        foreach (Room room in Collections.allRooms)
        {
            //Debug.Log(room);
            Vector2 oldPos = room.position;
            Vector2 separation = ComputeSeparation(room);
            if (separation != Vector2.zero) changed = true;
            //Debug.Log("Separation: " + separation);
            room.position = new Vector2(oldPos.x += separation.x, oldPos.y += separation.y);
            //Debug.Log(room);
        }

        return changed;
    }

    private Vector2 ComputeSeparation(Room agent)
    {
        int neighbours = 0;
        Vector2 v = new Vector2();

        foreach (Room room in Collections.allRooms)
            if (room != agent)
                if (agent.TooCloseTo(room))
                {
                    v += Difference(room, agent, "x");
                    //v.y += Difference(room, agent, "y");
                    neighbours++;
                }

        if (neighbours == 0)
            return v;

        v.x /= neighbours / 2f;
        v.y /= neighbours / 2f;
        v.x *= -1;
        v.y *= -1;
        return v;
    }

    private Vector2 Difference(Room room, Room agent, string type)
    {
        Vector2 difference = Vector2.zero;
        if (room.position.x + room.size.x / 2f > agent.position.x + agent.size.x / 2f)
            difference.x = agent.position.x + agent.size.x - room.position.x;
        else
            difference.x = agent.position.x - room.position.x - room.size.x;
        if (room.position.y + room.size.y / 2f > agent.position.y + agent.size.y / 2f)
            difference.y = agent.position.y + agent.size.y - room.position.y;
        else
            difference.y = agent.position.y - room.position.y - room.size.y;
        return difference;
    }

    public void GenerateDungeon()
    {
        int xOffset = 32;
        int yOffset = 32;
        int currentX = xOffset;

        Tile tile = new Tile(12);
        StaticWorld(tile, tile);

        int randomNumDungeons = Random.Range(6, 10);

        for (int i = 0; i < randomNumDungeons; i++)
        {
            int randomWidth = Random.Range(16, 64);
            int randomHeight = Random.Range(16, 32);
            Rectangle(currentX, yOffset, randomWidth, randomHeight);
            Rectangle(currentX, yOffset, randomWidth, randomHeight, 13, 1);
            Rectangle(currentX + 1, yOffset + 1, randomWidth - 2, randomHeight - 2, 11, 1);
            HorizontalLine(currentX, yOffset - 1, randomWidth, 12);
            HorizontalLine(currentX, yOffset + 4, randomWidth, ItemID.WoodPlatform);

            int randomDecorationSpot = Random.Range(currentX, currentX + randomWidth);
            Decoration(randomDecorationSpot, yOffset, 908);
            GameManager.Get<SpawnManager>().SpawnPack(new Int2(randomDecorationSpot, yOffset),
                GameManager.Get<Prefabs>().mouse, 4, 4);
            currentX += randomWidth;

            if (i < randomNumDungeons - 1)
            {
                world.DeleteTile(0, currentX, yOffset);
                world.DeleteTile(0, currentX, yOffset + 1);
                world.DeleteTile(0, currentX, yOffset + 2);

                Decoration(currentX, yOffset, 905);
            }

            currentX += 1;
        }
    }

    public void GenerateMaplestoryShop()
    {
        int width = 32;
        int height = 16;
        int border = 40;
        Tile tile = new Tile(4);
        StaticWorld(tile, tile);
        Rectangle(border, border, width, height);
        world.SetTile(2, border + 4, border + 8, ItemID.Torch);
        world.SetTile(2, border + width - 4, border + 8, ItemID.Torch);
    }

    public void GenerateFlatWorld()
    {
        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
                if (y < world.Height / 2)
                {
                    world.SetTile(0, x, y, ItemID.Stone);
                    world.SetTile(1, x, y, ItemID.Stone);
                    world.SetTile(2, x, y, 0);
                }
                else if (y < world.Height / 2 + 3)
                {
                    world.SetTile(0, x, y, ItemID.Dirt);
                    world.SetTile(1, x, y, ItemID.Dirt);
                    world.SetTile(2, x, y, 0);
                }

                else
                {
                    world.SetTile(0, x, y, 0);
                    world.SetTile(1, x, y, 0);
                    world.SetTile(2, x, y, 0);
                }
    }

    public void ClearWorld()
    {
        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
            {
                world.SetTile(0, x, y, 0);
                world.SetTile(1, x, y, 0);
                world.SetTile(2, x, y, 0);
            }
    }

    #endregion

    #region Terrain Functions

    public void GenerateTerrain()
    {
        int undergroundLayer = 500;
        for (int x = 0; x < world.Width; x++)
        {
            float stone = Utilities.PerlinNoise(x + world.Seed, 0, 80, 15, 1);
            stone += Utilities.PerlinNoise(x + world.Seed, 0, 50, 30, 1);
            stone += undergroundLayer;

            float dirt = Utilities.PerlinNoise(x + world.Seed, 0, 100f, 35, 1);
            dirt += Utilities.PerlinNoise(x + world.Seed, 100, 50, 30, 1);
            dirt += undergroundLayer;

            for (int y = 0; y < world.Height; y++)
                if (y < stone)
                {
                    world.SetTile(0, x, y, ItemID.Stone);
                    world.SetTile(1, x, y, ItemID.Stone);
                    world.SetTile(2, x, y, 0);
                    if (Utilities.PerlinNoise(x, y, 45, 17, 1) > 14.5f)
                    {
                        world.SetTile(0, x, y, ItemID.Dirt);
                        world.SetTile(1, x, y, ItemID.Dirt);
                        world.SetTile(2, x, y, 0);
                    }

                    if (y < undergroundLayer + 24 && Utilities.PerlinNoise(x + world.Seed, y, 50, 10, 1.1f) > 11.95f)
                    {
                        world.SetTile(0, x, y, ItemID.Coal);
                        world.SetTile(1, x, y, ItemID.Dirt);
                        world.SetTile(2, x, y, 0);
                    }

                    if (y < undergroundLayer && Utilities.PerlinNoise(x + world.Seed, y, 40, 10, 1.2f) > 15.1f)
                    {
                        world.SetTile(0, x, y, ItemID.CopperOre);
                        world.SetTile(1, x, y, ItemID.Dirt);
                        world.SetTile(2, x, y, 0);
                    }

                    if (y < undergroundLayer / 2 && Utilities.PerlinNoise(x + world.Seed, y, 35, 10, 1.21f) > 15.2f)
                    {
                        world.SetTile(0, x, y, ItemID.IronOre);
                        world.SetTile(1, x, y, ItemID.Dirt);
                        world.SetTile(2, x, y, 0);
                    }
                }
                else if (y < dirt)
                {
                    world.SetTile(0, x, y, ItemID.Dirt);
                    world.SetTile(1, x, y, ItemID.Dirt);
                    world.SetTile(2, x, y, 0);
                }
                else
                {
                    world.SetTile(0, x, y, 0);
                    world.SetTile(1, x, y, 0);
                    world.SetTile(2, x, y, 0);
                }
        }
    }

    public void GenerateCave()
    {
        int undergroundLayer = 500;

        float perlinValue;

        float xHead;
        float yHead;

        float xNoise = 7f / 2048f + world.Seed;
        float yNoise = 1163f / 2048f + world.Seed;

        int numCaves = 8;
        int maxLength, length;

        for (int n = 0; n < numCaves; n++)
        {
            xHead = Mathf.Clamp(Random.Range(n * world.Width / numCaves - 16, n * world.Width / numCaves + 16), 20,
                world.Width - 20);
            yHead = Random.Range(undergroundLayer - 20, undergroundLayer + 40);
            maxLength = Random.Range(150, 400);
            length = 0;
            while (yHead > 20 && length < maxLength)
            {
                perlinValue = Mathf.Deg2Rad * Utilities.PerlinNoise(xNoise, yNoise, 250, 180, 1);

                xHead -= 5 * Mathf.Cos(perlinValue);
                yHead -= 1 * Mathf.Sin(perlinValue);

                xNoise -= 2;
                yNoise -= 6;

                float caveRadius = 3f + Utilities.PerlinNoise(xHead, yHead, 90f, 2.1f, 2.2f) +
                                   Utilities.PerlinNoise(xHead, yHead, 70f, 1.8f, 4.5f);
                CarveCircle((int)xHead, (int)yHead, (int)caveRadius, 0.75f, 1.33f);
                length++;
            }
        }

        numCaves = 6;
        for (int n = 0; n < numCaves; n++)
        {
            xHead = Mathf.Clamp(Random.Range(n * world.Width / numCaves - 16, n * world.Width / numCaves + 16), 20,
                world.Width - 20);
            yHead = Random.Range(120, 280);
            maxLength = Random.Range(100, 300);
            length = 0;
            while (yHead > 20 && length < maxLength)
            {
                perlinValue = Mathf.Deg2Rad * Utilities.PerlinNoise(xNoise, yNoise, 250, 180, 1);

                xHead -= 5 * Mathf.Cos(perlinValue);
                yHead -= 1 * Mathf.Sin(perlinValue);

                xNoise -= 2;
                yNoise -= 6;

                float caveRadius = 3f + Utilities.PerlinNoise(xHead, yHead, 90f, 2.1f, 2.2f) +
                                   Utilities.PerlinNoise(xHead, yHead, 70f, 1.8f, 4.5f);
                CarveCircle((int)xHead, (int)yHead, (int)caveRadius, 0.75f, 1.33f);
                length++;
            }
        }
    }

    public void FlattenTerrain()
    {
        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
            {
                Tile block = world.GetTile(0, x, y);
                if (block.ID == 1 || block.ID == 2)
                {
                    if (world.HasTile(0, x, y + 1)) continue;
                    if (!world.HasTile(0, x, y - 1)) continue;
                    if (world.HasTile(0, x - 1, y) && !world.HasTile(0, x + 1, y))
                        world.SetTile(0, x, y, new Tile(block.ID, 0, 1));
                    else if (!world.HasTile(0, x - 1, y) && world.HasTile(0, x + 1, y))
                        world.SetTile(0, x, y, new Tile(block.ID, 0, 2));
                }
            }

        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
            {
                Tile block = world.GetTile(0, x, y);
                if (block.ID == 1 || block.ID == 2)
                {
                    if (world.HasTile(1, x, y + 1)) continue;
                    if (!world.HasTile(1, x, y - 1)) continue;
                    if (world.HasTile(1, x - 1, y) && !world.HasTile(1, x + 1, y))
                        world.SetTile(1, x, y, new Tile(block.ID, 0, 1));
                    else if (!world.HasTile(1, x - 1, y) && world.HasTile(1, x + 1, y))
                        world.SetTile(1, x, y, new Tile(block.ID, 0, 2));
                }
            }
    }

    public void SetGrass()
    {
        Tile grassTile;
        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
            {
                Tile block = world.GetTile(0, x, y);
                if (block.ID == 1 && !world.HasTile(0, x, y + 1))
                {
                    grassTile = new Tile(1001, 0, world.GetTile(0, x, y).Info);
                    grassTile.RandomVariation();
                    world.SetTile(3, x, y + 1, grassTile);
                }
            }
    }

    public void StaticWorld(Tile block, Tile wall)
    {
        for (int x = 0; x < world.Width; x++)
            for (int y = 0; y < world.Height; y++)
            {
                world.SetTile(0, x, y, block);
                world.SetTile(1, x, y, wall);
            }
    }

    #endregion

    #region Utility Functions

    private void CarveCircle(int x, int y, int radius, float widthScale, float heightScale)
    {
        for (int i = -radius; i < radius; i++)
            for (int j = -radius; j < radius; j++)
                if (i * i * widthScale + j * j * heightScale <= radius * radius)
                    world.DeleteTile(0, x + i, y + j);
    }

    private void Rectangle(int x, int y, int width, int height, int tile = 0, int layer = 0)
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                world.SetTile(layer, x + i, y + j, tile);
    }

    private void HorizontalLine(int x, int y, int width, int tile = 0, int layer = 0)
    {
        for (int i = 0; i < width; i++)
            world.SetTile(layer, x + i, y, tile);
    }

    private void VerticalLine(int x, int y, int height, int tile = 0, int layer = 0)
    {
        for (int i = 0; i < height; i++)
            world.SetTile(layer, x + i, y, tile);
    }

    private void Decoration(int x, int y, int tile = 0)
    {
        DecorationData decoration = ItemDatabase.GetDecoration(tile);
        if (!decoration) return;
        for (int i = x; i < x + decoration.Width; i++)
            for (int j = y; j < y + decoration.Height; j++)
                world.SetTile(2, i, j, ushort.MaxValue);
        world.SetTile(2, x, y, tile);
    }

    #endregion
}