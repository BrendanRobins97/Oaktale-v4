// File: Hammer.cs
// Author: Brendan Robinson
// Date Created: 07/12/2018
// Date Last Modified: 07/14/2018
// Description: 

using UnityEngine;

public class Hammer : Tool
{
    [SerializeField] private int miningRange = 5;
    [SerializeField] private float cooldown = 0.25f;

    private float cooldownTimer;

    protected override void Update()
    {
        base.Update();
        cooldownTimer -= Time.deltaTime;
    }

    public override void PrimaryUse()
    {
        if (cooldownTimer > 0) return;
        base.PrimaryUse();
        int directionX = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x);
        int directionY = (int)(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y);

        if (Mathf.Abs(directionX) > miningRange || Mathf.Abs(directionY) > miningRange ||
            GameManager.Get<WorldManager>().currentWorld.GetTile(0, Utilities.GetMousePosition()).ID == 0) return;

        Int2 mousePos = Utilities.GetMousePosition();
        Tile currentTile = GameManager.Get<WorldManager>().currentWorld.GetTile(0, mousePos);
        currentTile.Info = (byte)((currentTile.Info + 1) % 9);
        GameManager.Get<WorldManager>().SetBlock(mousePos, currentTile);

        mousePos += Int2.up;
        currentTile = GameManager.Get<WorldManager>().currentWorld.GetTile(3, mousePos);
        currentTile.Info = (byte)((currentTile.Info + 1) % 9);
        GameManager.Get<WorldManager>().SetForeground(mousePos, currentTile);

        cooldownTimer = cooldown;
    }
}