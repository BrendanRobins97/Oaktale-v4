// File: Door.cs
// Author: Brendan Robinson
// Date Created: 07/12/2018
// Date Last Modified: 07/14/2018
// Description: 

using UnityEngine;

public class Door : DecorationController
{
    [SerializeField] private float maxOpenDistance = 5f;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private BoxCollider2D collisionCollider;

    private bool open = false;
    private SpriteRenderer spriteRenderer;
    private Player player;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameManager.Get<PlayerInfo>().player;
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if ((player.transform.position - transform.position).magnitude > maxOpenDistance ||
                player.FeetPosition == position ||
                player.FeetPosition == position + Int2.up ||
                player.FeetPosition == position + Int2.up + Int2.up)
                return;
            open = !open;
            if (open)
            {
                if (!player.facingRight)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    transform.position = position.ToVector2() + Vector2.right;
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    transform.position = position.ToVector2();
                }

                spriteRenderer.sprite = openSprite;
                collisionCollider.enabled = false;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.position = position.ToVector2();

                spriteRenderer.sprite = closedSprite;
                collisionCollider.enabled = true;
            }
        }
    }
}