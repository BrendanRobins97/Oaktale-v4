// File: DecorationData.cs
// Author: Brendan Robinson
// Date Created: 01/01/2018
// Date Last Modified: 07/24/2018
// Description: 

using UnityEngine;

public enum DecorationType
{
    Grounded,
    Mounted,
    Free
}

[CreateAssetMenu(fileName = "Decoration", menuName = "Decoration", order = 3)]
public class DecorationData : ItemData
{
    public DecorationType DecorationType;
    public int Height;

    public DecorationController Prefab;
    public int Width;

    public override void Use1(PlayerInfo playerInfo)
    {
        Int2 pos = Utilities.GetMousePosition();
        if (!GameManager.Get<WorldManager>().SetDecoration(pos, id)) return;
        InstantiateDecoration(pos);
        playerInfo.inventory.RemoveItem(new Item(id));
    }

    protected virtual void SetUpPrefab(int x, int y, ref DecorationController decorationController)
    {
    }

    public DecorationController InstantiateDecoration(int x, int y, Transform container = null)
    {
        DecorationController decorCtrlr = Instantiate(Prefab, new Vector2(x, y), Quaternion.identity, container);
        decorCtrlr.id = id;
        decorCtrlr.position = new Int2(x, y);
        decorCtrlr.facingRight = true;
        SetUpPrefab(x, y, ref decorCtrlr);
        GameManager.Get<WorldManager>().AddDecoration(x, y, decorCtrlr);
        return decorCtrlr;
    }

    public DecorationController InstantiateDecoration(Int2 pos, Transform container = null)
    {
        return InstantiateDecoration(pos.x, pos.y, container);
    }
}