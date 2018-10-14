
public class ItemID
{
    public static readonly int Air = 0;
    public static readonly int Dirt = 1;
    public static readonly int Stone = 2;
    public static readonly int Sand = 3;
    public static readonly int Wood = 4;
    public static readonly int Clay = 5;
    public static readonly int StoneBrick = 6;
    public static readonly int Wood2 = 16;
    public static readonly int WoodPlatform = 17;

    public static readonly int CopperOre = 51;
    public static readonly int IronOre = 52;
    public static readonly int Coal = 53;

    public static readonly int Torch = 901;
    

    public static readonly int Furnace = 1101;
    public static readonly int Refinery = 1102;

    public static readonly int CornSeed = 2001;
    public static readonly int TomatoSeed = 2002;
    public static readonly int PotatoSeed = 2003;
    public static readonly int PumpkinSeed = 2004;

    public static readonly int Corn = 2101;
    public static readonly int Tomato = 2102;
    public static readonly int Potato = 2103;
    public static readonly int Pumpkin = 2104;

    public static readonly int Hoe = 5101;
    public static readonly int WateringCan = 5102;

}

public class LayerInfo
{
    public static readonly int BlockIndex = 0;
    public static readonly int WallIndex = 1;
    public static readonly int DecorationIndex = 2;
    public static readonly int ForegroundIndex = 3;
    public static readonly int LiquidIndex = 4;

    public static readonly float Overlay = -64f;
    public static readonly float Foreground = -32f;
    public static readonly float Block = 0f;
    public static readonly float FrontBlock = 0f;
    public static readonly float BackBlock = 0.5f;

    public static readonly float Liquid = 32f;
    public static readonly float Wall = 64f;
}

public class TextureOffset
{
    public static readonly byte Tilled = 9;
    public static readonly byte Watered = 10;
    public static readonly byte Wall = 64;

}