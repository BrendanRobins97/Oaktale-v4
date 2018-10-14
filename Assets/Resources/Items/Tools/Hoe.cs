using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoe : Tool {

    public override void PrimaryUse()
    {
        base.PrimaryUse();
        Int2 mousePosition = Utilities.GetMousePosition();
        Tile tile = GameManager.Get<WorldManager>().currentWorld.GetTile(0, mousePosition);
        Tile tileAbove = GameManager.Get<WorldManager>().currentWorld.GetTile(0, mousePosition + Int2.up);
        if (tile.ID == ItemID.Dirt && tileAbove.ID == 0)
        {
            GameManager.Get<WorldManager>().currentWorld.DeleteTile(3, mousePosition + Int2.up);
            GameManager.Get<WorldManager>().SetBlock(mousePosition, new Tile(tile.ID, tile.Variation, TextureOffset.Tilled));
        }
    }

}
