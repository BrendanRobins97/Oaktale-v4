// File: Room.cs
// Author: Brendan Robinson
// Date Created: 07/31/2018
// Date Last Modified: 07/31/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2 position;
    public Int2 size;
    public List<int> adjoinTriangles;

    public Room(Vector2 position, Int2 size)
    {
        this.position = position;
        this.size = size;
        adjoinTriangles = new List<int>();

    }

    public int area
    {
        get { return size.x * size.y; }
    }

    public bool TooCloseTo(Room room)
    {
        return doOverlap(position + new Vector2(-size.x / 2f, size.y / 2f), position + new Vector2(size.x / 2f, -size.y / 2f),
            room.position + new Vector2(-room.size.x / 2f, room.size.y / 2f), room.position + new Vector2(room.size.x / 2f, -room.size.y / 2f));
    }
    bool doOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2)
    {
        // If one rectangle is on left side of other
        if (l1.x > r2.x || l2.x > r1.x)
            return false;

        // If one rectangle is above other
        if (l1.y < r2.y || l2.y < r1.y)
            return false;

        return true;
    }
    public override string ToString()
    {
        return string.Format("Position:{0} Size:{1} Area:{2}", position, size, area);
    }
}