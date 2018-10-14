/* Int2.cs
 * 
 * Struct that holds a 2D vector with 2 ints
 */

using UnityEngine;

[System.Serializable]
public struct Int2 : System.IEquatable<Int2>
{
    public int x;
    public int y;

    public static readonly Int2 zero = new Int2(0, 0);
    public static readonly Int2 one = new Int2(1, 1);
    public static readonly Int2 up = new Int2(0, 1);
    public static readonly Int2 down = new Int2(0, -1);
    public static readonly Int2 left = new Int2(-1, 0);
    public static readonly Int2 right = new Int2(1, 0);
    public static readonly Int2 upLeft = new Int2(-1, 1);
    public static readonly Int2 upRight = new Int2(1, 1);
    public static readonly Int2 downLeft = new Int2(-1, -1);
    public static readonly Int2 downRight = new Int2(1, -1);

    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Int2(Vector2 v)
    {
        this.x = (int)v.x;
        this.y = (int)v.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public static Int2 operator +(Int2 a, Int2 b)
    {
        return new Int2(a.x + b.x, a.y + b.y);
    }

    public static Int2 operator -(Int2 a, Int2 b)
    {
        return new Int2(a.x - b.x, a.y - b.y);
    }

    public static Int2 operator /(Int2 a, int b)
    {
        return new Int2(a.x / b, a.y / b);
    }

    public static Int2 operator *(Int2 a, int b)
    {
        return new Int2(a.x * b, a.y * b);
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.y + ")";
    }

    public static bool operator ==(Int2 a, Int2 b)
    {
        return (a.x == b.x && a.y == b.y);
    }

    public static bool operator !=(Int2 a, Int2 b)
    {
        return (a.x != b.x || a.y != b.y);
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Int2 i = (Int2)obj;
        return (x == i.x && y == i.y);
    }

    public bool Equals(Int2 i)
    {
        return (x == i.x && y == i.y);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() << 2;
    }
}