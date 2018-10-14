using UnityEngine;

[System.Serializable]
public struct Tile : System.IEquatable<Tile>
{
    public ushort ID;
    public byte Variation;
    public byte Info;

    public static readonly Tile empty = new Tile(0, 0, 0);

    public Tile(int id = 0, byte variation = 0, byte info = 0)
    {
        this.ID = (ushort)id;
        this.Variation = variation;
        this.Info = info;
    }

    public Tile(byte[] bytes)
    {
        this.ID = (ushort)(bytes[0] << 8 + bytes[1]);
        this.Variation = bytes[2];
        this.Info = bytes[3];
    }

    public byte[] ToBytes()
    {
        return new byte[] { (byte)(ID >> 8), (byte)ID, Variation, Info };
    }

    public void RandomVariation(byte numVariations = 4)
    {
        Variation = (byte)Random.Range(0, numVariations);
    }

    public bool Equals(Tile other)
    {
        return (ID == other.ID);
    }

    public bool StrictEquals(Tile other)
    {
        return (ID == other.ID && Variation == other.Variation && Info == other.Info);
    }

    public override string ToString()
    {
        return "(id: " + ID + " variation: " + Variation + "info: " + Info + ")";
    }
}
