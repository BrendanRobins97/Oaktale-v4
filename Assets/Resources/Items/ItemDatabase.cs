using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{

    public static ItemData[] items;
    public static Dictionary<int, ItemData> itemDictionary = new Dictionary<int, ItemData>();
    public static Dictionary<string, int> ids = new Dictionary<string, int>();

    public static void ConstructDatabase()
    {
        items = new ItemData[ushort.MaxValue];
        ItemData[] itemList = Resources.LoadAll<ItemData>("Items");
        for (int i = 0; i < itemList.Length; i++)
        {
            items[itemList[i].id] = itemList[i];
            itemDictionary.Add(itemList[i].id, itemList[i]);

            ids.Add(itemList[i].name.Trim().ToLower(), itemList[i].id);
        }
    }

    public static void PrintAllItems()
    {
        foreach (ItemData item in items)
        {
            if (item != null)
            {
                Debug.Log("ID: " + item.id + ", Name: " + item.name);
            }
        }
    }

    public static bool ValidID(int id)
    {
        if (id == 0)
        {
            return false;
        }
        return ids.ContainsValue(id);
    }
    public static int GetID(string tileName)
    {
        return ids[tileName.Trim().ToLower()];
    }
    public static T GetItemData<T>(int id) where T : ItemData
    {
        if (items[id] is T)
        {
            return (T)items[id];
        }
        return null;
    }
    public static T GetItemData<T>(Item item) where T : ItemData
    {
        return GetItemData<T>(item.id);
    }
    public static ItemData GetItemData(int id)
    {
        return items[id];
    }

    public static ItemData GetItemData(Item item)
    {
        return items[item.id];
    }

    public static BlockData GetBlock(int id)
    {
        if (items[id] is BlockData)
        {
            return (BlockData)items[id];
        }
        return null;
    }

    public static BlockData GetBlock(Tile tile)
    {
        return GetBlock(tile.ID);
    }

    public static DecorationData GetDecoration(int id)
    {
        if (items[id] is DecorationData)
        {
            return (DecorationData)items[id];
        }
        return null;
    }

    public static DecorationData GetDecoration(Tile tile)
    {
        return GetDecoration(tile.ID);
    }

    public static ForegroundData GetForeground(int id)
    {
        if (items[id] is ForegroundData)
        {
            return (ForegroundData)items[id];
        }
        return null;
    }
    public static ArmorData GetArmor(int id)
    {
        if (items[id] is ArmorData)
        {
            return (ArmorData)items[id];
        }
        return null;
    }
    public static ForegroundData GetForeground(Tile tile)
    {
        return GetForeground(tile.ID);
    }

    public static LiquidData GetLiquid(int id)
    {
        if (items[id] is LiquidData)
        {
            return (LiquidData)items[id];
        }
        return null;
    }

    public static LiquidData GetLiquid(Tile tile)
    {

        return GetLiquid(tile.ID);
    }

    public static bool IsLiquid(int id)
    {
        if (id == 8001 || id == 8002)
        {
            return true;
        }
        return false;
    }

    public static bool IsLiquid(Tile tile)
    {
        return IsLiquid(tile.ID);
    }


    public static int GetLayer(int id)
    {
        if (items[id] is BlockData)
        {
            BlockData tileData = (BlockData)items[id];
            return tileData.layer;
        }
        else if (items[id] is ForegroundData)
        {
            ForegroundData tileData = (ForegroundData)items[id];
            return tileData.layer;
        }
        return 0;
    }
}
