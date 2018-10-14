/* World.cs
 * 
 * World class that contains the information on the world's size and tile contents.
 * Contents include the blocks, walls, and decorations of the world
 */
using UnityEngine;

public enum LayerIndex { Block = 0, Wall = 1, Decoration = 2, Foreground = 3, Liquid = 4 }

[System.Serializable]

public class World
{

    private static readonly int NumLayers = 5;
    private static readonly int SeedRange = 10000;
    private static readonly int MaxNameLength = 32;

    // Public variables
    public char[] Name { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Seed { get; private set; }

    private Layer[] layers;

    #region Setup

    // Constructor
    public World(string name, int width, int height, int seed = -1)
    {
        this.Name = new char[MaxNameLength];
        char[] charArr = name.ToCharArray();
        for (int i = 0; i < charArr.Length; i++)
        {
            this.Name[i] = charArr[i];
        }
        for (int i = charArr.Length; i < MaxNameLength; i++)
        {
            this.Name[i] = '\0';
        }
        this.Width = width;
        this.Height = height;

        layers = new Layer[NumLayers];
        for (int i = 0; i < NumLayers; i++)
        {
            layers[i] = new Layer(width, height);
        }

        if (seed == -1)
        {
            this.Seed = UnityEngine.Random.Range(0, 10000);

        }
        else
        {
            this.Seed = seed;
        }
    }

    public World(byte[] byteArray)
    {   /*
        Name = new char[maxNameLength];
        for (int i = 0; i < maxNameLength; i++)
        {
            Name[i] = (char)byteArray[i];
        }
        Width = byteArray[maxNameLength] << 24 |
            byteArray[maxNameLength + 1] << 16 |
            byteArray[maxNameLength + 2] << 8 |
            byteArray[maxNameLength + 3];
        Height = byteArray[maxNameLength + 4] << 24 |
            byteArray[maxNameLength + 5] << 16 |
            byteArray[maxNameLength + 6] << 8 |
            byteArray[maxNameLength + 7];
        Seed = byteArray[maxNameLength + 8] << 24 |
            byteArray[maxNameLength + 9] << 16 |
            byteArray[maxNameLength + 10] << 8 |
            byteArray[maxNameLength + 11];
        
        blocks = new Tile[Width * Height];
        decorations = new Tile[Width * Height];
        walls = new Tile[Width * Height];
        foreground = new Tile[Width * Height];
        for (int i = maxNameLength + 12; i < maxNameLength + 12 + 4 * Width * Height; i += 4)
        {
            blocks[(i - (maxNameLength + 12)) / 4] = new Tile((ushort)(byteArray[i] >> 8 | byteArray[i+1]), byteArray[i + 2], byteArray[i + 3]);

        }
        for (int i = maxNameLength + 12 + 4 * Width * Height; i < maxNameLength + 12 + 8 * Width * Height; i += 4)
        {
            decorations[(i - (maxNameLength + 12 + 4 * Width * Height)) / 4] = new Tile((ushort)(byteArray[i] >> 8 | byteArray[i + 1]), byteArray[i + 2], byteArray[i + 3]);
        }
        for (int i = maxNameLength + 12 + 8 * Width * Height; i < maxNameLength + 12 + 12 * Width * Height; i += 4)
        {
            walls[(i - (maxNameLength + 12 + 8 * Width * Height)) / 4] = new Tile((ushort)(byteArray[i] >> 8 | byteArray[i + 1]), byteArray[i + 2], byteArray[i + 3]);
        }
        for (int i = maxNameLength + 12 + 12 * Width * Height; i < maxNameLength + 12 + 16 * Width * Height; i += 4)
        {
            foreground[(i - (maxNameLength + 12 + 12 * Width * Height)) / 4] = new Tile((ushort)(byteArray[i] >> 8 | byteArray[i + 1]), byteArray[i + 2], byteArray[i + 3]);
        }
        
    }
    public byte[] ToBytes()
    {
        
        byte[] byteArray = new byte[MaxNameLength + 4 + 4 + 4 + 16 * Width * Height];
        /*
        for (int i = 0; i < maxNameLength; i++)
        {
            byteArray[i] = (byte)Name[i];
        }

        byteArray[maxNameLength] = (byte)(Width >> 24);
        byteArray[maxNameLength + 1] = (byte)(Width >> 16);
        byteArray[maxNameLength + 2] = (byte)(Width >> 8);
        byteArray[maxNameLength + 3] = (byte)(Width);

        byteArray[maxNameLength + 4] = (byte)(Height >> 24);
        byteArray[maxNameLength + 5] = (byte)(Height >> 16);
        byteArray[maxNameLength + 6] = (byte)(Height >> 8);
        byteArray[maxNameLength + 7] = (byte)(Height);

        byteArray[maxNameLength + 8] = (byte)(Seed >> 24);
        byteArray[maxNameLength + 9] = (byte)(Seed >> 16);
        byteArray[maxNameLength + 10] = (byte)(Seed >> 8);
        byteArray[maxNameLength + 11] = (byte)(Seed);

        byte[] arr;
        for (int i = maxNameLength + 12; i < maxNameLength + 12 + 4 * Width * Height; i+=4)
        {
            arr = blocks[(i - (maxNameLength + 12))/4].ToBytes();
            byteArray[i] = arr[0];
            byteArray[i+1] = arr[1];
            byteArray[i+2] = arr[2];
            byteArray[i+3] = arr[3];
        }
        for (int i = maxNameLength + 12 + 4 * Width * Height; i < maxNameLength + 12 + 8 * Width * Height; i += 4)
        {
            arr = decorations[(i -(maxNameLength + 12 + 4 * Width * Height)) / 4].ToBytes();
            byteArray[i] = arr[0];
            byteArray[i + 1] = arr[1];
            byteArray[i + 2] = arr[2];
            byteArray[i + 3] = arr[3];
        }
        for (int i = maxNameLength + 12 + 8 * Width * Height; i < maxNameLength + 12 + 12 * Width * Height; i += 4)
        {
            arr = walls[(i - (maxNameLength + 12 + 8 * Width * Height))/4].ToBytes();
            byteArray[i] = arr[0];
            byteArray[i + 1] = arr[1];
            byteArray[i + 2] = arr[2];
            byteArray[i + 3] = arr[3];
        }
        for (int i = maxNameLength + 12 + 12 * Width * Height; i < maxNameLength + 12 + 16 * Width * Height; i += 4)
        {
            arr = foreground[(i - (maxNameLength + 12 + 12 * Width * Height))/4].ToBytes();
            byteArray[i] = arr[0];
            byteArray[i + 1] = arr[1];
            byteArray[i + 2] = arr[2];
            byteArray[i + 3] = arr[3];
        }
        
        return byteArray;
        */
    }

    public string WorldName()
    {
        int lastChar = 0;
        for (int i = 0; i < MaxNameLength; i++)
        {
            lastChar = i;
            if (Name[i] == 0)
            {
                break;
            }
            lastChar = i;
        }
        return new string(Name, 0, lastChar);
    }

    public void SetSeed(int seed)
    {
        this.Seed = seed;
    }

    #endregion

    #region BlockManagement

    public bool HasTile(int layer, int x, int y)
    {
        return layers[layer].HasTile(x, y);
    }
    public bool HasTile(int layer, Int2 pos)
    {
        return HasTile(layer, pos.x, pos.y);
    }
    public bool HasTile(int layer, Int2 bottomLeft, Int2 topRight)
    {
        for (int x = bottomLeft.x; x < topRight.x; x++)
        {
            for (int y = bottomLeft.y; y < topRight.y; y++)
            {
                if (HasTile(layer, x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Tile GetTile(int layer, int x, int y)
    {
        return layers[layer].GetTile(x, y);

    }
    public Tile GetTile(int layer, Int2 pos)
    {
        return GetTile(layer, pos.x, pos.y);
    }

    public void SetTile(int layer, int x, int y, Tile tile)
    {
        switch (tile.ID)
        {
            case 8:
                tile.Variation = (byte)((x + y) % 2);
                break;
            case 9:
                tile.Variation = (byte)(x % 2);
                break;
            case 14:
                tile.Variation = (byte)(x % 2 + 2 * (y % 2));
                break;
            case 16:
                tile.Variation = (byte)((x % 4 + y % 4) % 4);
                break;
            default:
                tile.RandomVariation();
                break;
        }
        layers[layer].SetTile(x, y, tile);
    }
    public void SetTile(int layer, int x, int y, int id)
    {
        SetTile(layer, x, y, new Tile((ushort)id));
    }
    public void SetTile(int layer, Int2 pos, Tile tile)
    {
        SetTile(layer, pos.x, pos.y, tile);
    }
    public void SetTile(int layer, Int2 pos, int id)
    {
        SetTile(layer, pos.x, pos.y, new Tile((ushort)id));
    }

    public void DeleteTile(int layer, int x, int y)
    {
        layers[layer].DeleteTile(x, y);
    }
    public void DeleteTile(int layer, Int2 pos)
    {
        DeleteTile(layer, pos.x, pos.y);
    }

    #endregion

    #region Collision

    public BlockHit HorizontalCollision(float xOrigin, float yOrigin, float xOffset)
    {
        BlockHit blockHit = new BlockHit(false, 0, Vector2.zero);
        Vector2 position;
        Tile block = GetTile(0, (int)xOrigin, (int)yOrigin);


        if (block.ID != 0)
        {

            if (xOffset > 0)
            {
                position = new Vector2(Mathf.Clamp(xOrigin + xOffset, (int)(xOrigin), (int)(xOrigin) + 1), yOrigin);
                switch (block.Info)
                {
                    case 2:
                        if ((yOrigin - (int)yOrigin) - (xOrigin - (int)xOrigin) < xOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = (yOrigin - (int)yOrigin) - (xOrigin - (int)xOrigin);
                            blockHit.normal.x = -1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (xOffset < 0)
            {
                position = new Vector2(Mathf.Clamp(xOrigin + xOffset, (int)(xOrigin), (int)(xOrigin) + 1), yOrigin);

                switch (block.Info)
                {
                    case 1:
                        if ((xOrigin - (int)xOrigin) - (1 - yOrigin + (int)yOrigin) < -xOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = (xOrigin - (int)xOrigin) - (1 - yOrigin + (int)yOrigin);
                            blockHit.normal.x = 1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    case 2:
                        blockHit.hit = true;
                        blockHit.distance = xOrigin - (int)position.x - 1;
                        blockHit.normal.x = 1;
                        break;
                    default:
                        break;
                }
            }
            if (blockHit.distance < 0)
            {
                blockHit.distance = 0;
            }
            if (blockHit.hit == true)
            {
                return blockHit;

            }
        }

        position = new Vector2(xOrigin + xOffset, yOrigin);
        block = GetTile(0, (int)position.x, (int)position.y);
        if (block.ID != 0)
        {
            if (xOffset > 0)
            {
                switch (block.Info)
                {
                    case 1:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.x - xOrigin;
                        blockHit.normal.x = -1;
                        break;
                    case 2:
                        if ((int)position.x - xOrigin + (yOrigin - (int)yOrigin) < xOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = (int)position.x - xOrigin + (yOrigin - (int)(yOrigin));
                            blockHit.normal.x = -1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    default:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.x - xOrigin;
                        blockHit.normal.x = -1;
                        break;
                }
            }
            else if (xOffset < 0)
            {
                switch (block.Info)
                {
                    case 1:
                        if (xOrigin - (int)position.x - 1 + (yOrigin - (int)yOrigin) < -xOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = xOrigin - (int)position.x - 1 + (yOrigin - (int)yOrigin);
                            blockHit.normal.x = 1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    case 2:
                        blockHit.hit = true;
                        blockHit.distance = xOrigin - (int)position.x - 1;
                        blockHit.normal.x = 1;
                        break;
                    default:
                        blockHit.hit = true;
                        blockHit.distance = xOrigin - (int)position.x - 1;
                        blockHit.normal.x = 1;
                        break;
                }
            }
            if (blockHit.distance < 0)
            {
                blockHit.distance = 0;
            }
        }
        return blockHit;
    }
    public BlockHit HorizontalCollision(Vector2 origin, float xOffset)
    {
        return HorizontalCollision(origin.x, origin.y, xOffset);
    }
    public BlockHit VerticalCollision(float xOrigin, float yOrigin, float yOffset)
    {
        BlockHit blockHit = new BlockHit(false, 0, Vector2.zero);
        Vector2 position;
        Tile block = GetTile(0, (int)xOrigin, (int)yOrigin);


        if (block.ID != 0)
        {
            if (yOffset > 0)
            {
                position = new Vector2(xOrigin, Mathf.Clamp(yOrigin + yOffset, (int)(yOrigin), (int)(yOrigin) + 1));

                switch (block.Info)
                {
                    case 1:

                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                    case 2:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                    default:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                }
            }
            else if (yOffset < 0)
            {
                position = new Vector2(xOrigin, Mathf.Clamp(yOrigin + yOffset, (int)(yOrigin), (int)(yOrigin) + 1));

                switch (block.Info)
                {
                    case 1:
                        if ((yOrigin - (int)yOrigin) - (1 - xOrigin + (int)xOrigin) < -yOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = (yOrigin - (int)yOrigin) - (1 - xOrigin + (int)xOrigin);
                            blockHit.normal.x = 1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    case 2:
                        if ((yOrigin - (int)yOrigin) - (xOrigin - (int)xOrigin) < -yOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = (yOrigin - (int)yOrigin) - (xOrigin - (int)xOrigin);
                            blockHit.normal.x = -1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (blockHit.distance < 0)
            {
                blockHit.distance = 0;
            }
            if (blockHit.hit == true)
            {
                return blockHit;

            }
        }
        position = new Vector2(xOrigin, yOrigin + yOffset);
        block = GetTile(0, (int)position.x, (int)position.y);
        if (block.ID != 0)
        {
            if (yOffset > 0)
            {
                switch (block.Info)
                {
                    case 1:

                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                    case 2:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                    default:
                        blockHit.hit = true;
                        blockHit.distance = (int)position.y - yOrigin;
                        blockHit.normal.y = -1;
                        break;
                }
            }
            else if (yOffset < 0)
            {
                switch (block.Info)
                {
                    case 1:
                        if (yOrigin - (int)position.y - 1 + (xOrigin - (int)xOrigin) < -yOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = yOrigin - (int)position.y - 1 + (xOrigin - (int)xOrigin);
                            blockHit.normal.x = 1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    case 2:
                        if (yOrigin - (int)position.y - 1 + (1 - xOrigin + (int)xOrigin) < -yOffset)
                        {
                            blockHit.hit = true;
                            blockHit.distance = yOrigin - (int)position.y - 1 + (1 - xOrigin + (int)xOrigin);
                            blockHit.normal.x = -1;
                            blockHit.normal.y = 1;
                        }
                        break;
                    default:
                        blockHit.hit = true;
                        blockHit.distance = yOrigin - (int)position.y - 1;
                        blockHit.normal.y = 1;
                        break;
                }
            }
            if (blockHit.distance < 0)
            {
                blockHit.distance = 0;
            }
        }
        return blockHit;


    }

    public BlockHit VerticalCollision(Vector2 origin, float yOffset)
    {
        return VerticalCollision(origin.x, origin.y, yOffset);
    }

    private BlockHit Collision(float x, float y, int direction, int blockInfo)
    {
        BlockHit blockHit = new BlockHit(true, 0, Vector2.zero);
        switch (blockInfo)
        {
            case 1:
                if (y < -x + 1)
                {
                    switch (direction)
                    {

                        default:
                            break;
                    }
                    blockHit.hit = true;
                }
                break;
            case 2:
                if (y > x)
                {
                    blockHit.hit = false;

                }
                break;
            default:
                break;
        }
        return blockHit;
    }

    #endregion

    #region Utilities
    // Checks if the coordinate is within the world boundaries
    //
    private bool InRange(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return true;
        }
        return false;
    }

    private bool InRange(Int2 pos)
    {
        return InRange(pos.x, pos.y);
    }

    public Texture2D TextureMap()
    {
        Texture2D tex = new Texture2D(Width, Height);
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Color color;
                switch (GetTile(0, i, j).ID)
                {
                    case 0:
                        color = Color.white;
                        break;
                    case 1:
                        color = Color.green;
                        break;
                    case 2:
                        color = Color.grey;
                        break;
                    case 51:
                        color = Color.red;
                        break;
                    case 52:
                        color = Color.blue;
                        break;
                    case 53:
                        color = Color.black;
                        break;
                    default:
                        color = Color.magenta;
                        break;
                }

                tex.SetPixel(i, j, color);
            }

        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return tex;
    }

    public void DisplaySpriteMap(float x = 0, float y = 0)
    {
        GameObject obj = new GameObject();
        Texture2D tex = TextureMap();
        obj.transform.position = new Vector3(x, y);
        obj.AddComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
    }
    #endregion

}

public struct BlockHit
{
    public bool hit;
    public float distance;
    public Vector2 normal;

    public BlockHit(bool hit, float distance, Vector2 normal)
    {
        this.hit = hit;
        this.distance = distance;
        this.normal = normal;
    }
}
