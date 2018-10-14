using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Seed")]
public class SeedData : DecorationData {

    public List<Sprite> stageSprites;
    public List<float> stageTimers;

    public override void Use1(PlayerInfo playerInfo)
    {
        Int2 pos = Utilities.GetMousePosition();
        if (GameManager.Get<WorldManager>().currentWorld.GetTile(0, pos + Int2.down).Info != TextureOffset.Watered)
        {
            return;
        }
        if (GameManager.Get<WorldManager>().SetDecoration(pos, id))
        {
            InstantiateDecoration(pos);
            playerInfo.inventory.RemoveItem(new Item(id));
        }
    }
    protected override void SetUpPrefab(int x, int y, ref DecorationController decorationController)
    {
        decorationController.GetComponent<CropGrowth>().SetCropInfo(stageSprites, stageTimers);
    }
}
