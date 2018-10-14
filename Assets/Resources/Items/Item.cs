using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : System.IEquatable<Item>
{
    public int id;
    public List<Stat> stats;

    public Item(int id = 0)
    {
        this.id = id;
    }

    public static bool operator == (Item item1, Item item2)
    {
        return (item1.id == item2.id);
    }

    public static bool operator != (Item item1, Item item2)
    {

        return (item1.id != item2.id);
    }
    public bool Equals(Item other)
    {
        return (id == other.id);
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Item item = (Item)obj;
        return (id == item.id);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public bool Empty()
    {
        return (id == 0);
    }
}
