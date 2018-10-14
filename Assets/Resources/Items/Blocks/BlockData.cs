// File: BlockData.cs
// Author: Brendan Robinson
// Date Created: 01/01/2018
// Date Last Modified: 07/15/2018
// Description: 

using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "Block", order = 1)]
public class BlockData : ItemData
{
    // Constant values to calculate vertices correctly

    public int layer;
    public int health;

    public override void Use1(PlayerInfo playerInfo)
    {
        Int2 pos = Utilities.GetMousePosition();
        Tile block = new Tile(id);
        switch (id)
        {
            case 8:
                block.Variation = (byte)((pos.x + pos.y) % 2);
                break;
            case 9:
                block.Variation = (byte)(pos.x % 2);
                break;
            case 14:
                block.Variation = (byte)(pos.x % 2 + 2 * (pos.y % 2));
                break;
            case 16:
                block.Variation = (byte)((pos.x % 4 + pos.y % 4) % 4);
                break;
            default:
                block.RandomVariation();
                break;
        }

        if (GameManager.Get<WorldManager>().SetBlock(pos, block)) playerInfo.inventory.RemoveItem(new Item(id));
    }

    public override void Use2(PlayerInfo playerInfo)
    {
        Int2 pos = Utilities.GetMousePosition();
        Tile block = new Tile(id);

        switch (id)
        {
            case 8:
                block.Variation = (byte)((pos.x + pos.y) % 2);
                break;
            case 9:
                block.Variation = (byte)(pos.x % 2);
                break;
            case 14:
                block.Variation = (byte)(pos.x % 2 + 2 * (pos.y % 2));
                break;
            case 16:
                block.Variation = (byte)((pos.x % 4 + pos.y % 4) % 4);
                break;
            default:
                block.RandomVariation();
                break;
        }

        if (GameManager.Get<WorldManager>().SetWall(pos, id)) playerInfo.inventory.RemoveItem(new Item(id));
    }

}