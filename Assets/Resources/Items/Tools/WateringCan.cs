using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : Tool {

    public override void PrimaryUse()
    {
        base.PrimaryUse();
        Int2 mousePosition = Utilities.GetMousePosition();
        Tile tile = GameManager.Get<WorldManager>().currentWorld.GetTile(0, mousePosition);
        if (tile.Info == TextureOffset.Tilled)
        {
            GameManager.Get<WorldManager>().SetBlock(mousePosition, new Tile(tile.ID, tile.Variation, TextureOffset.Watered));
        }
    }
}
