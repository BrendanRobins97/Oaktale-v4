// File: ColliderGenerator.cs
// Author: Brendan Robinson
// Date Created: 06/23/2018
// Date Last Modified: 07/25/2018
// Description: 

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ColliderGenerator : MonoBehaviour
{
    private List<Int2> colPositions = new List<Int2>();
    private float width;
    private float height;

    private Int2 blockPosition = Int2.one;
    private Int2 oldBlockPosition = Int2.zero;

    private Rigidbody2D rb;

    private BoxCollider2D collider;

    // Use this for initialization
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        width = collider.size.x;
        height = collider.size.y;
    }

    // Update is called once per frame
    public void Update()
    {
        Vector2 currentVelocity = rb.velocity * Time.deltaTime;
        blockPosition = new Int2((int)(transform.position.x + currentVelocity.x),
            (int)(transform.position.y + currentVelocity.y));

        if (blockPosition.x != oldBlockPosition.x || blockPosition.y != oldBlockPosition.y)
        {
            oldBlockPosition = blockPosition;
            foreach (Int2 pos in colPositions) GameManager.Get<ChunkManager>().DeleteCollider(pos);
            colPositions.Clear();
            for (int x = (int)(transform.position.x + currentVelocity.x - width / 2f - 1);
                x <= (int)(transform.position.x + currentVelocity.x + width / 2f + 1);
                x++)
                for (int y = (int)(transform.position.y + currentVelocity.y - 1);
                    y <= (int)(transform.position.y + currentVelocity.y + height + 1);
                    y++)
                {
                    colPositions.Add(new Int2(x, y));
                    GameManager.Get<ChunkManager>().AddCollider(new Int2(x, y));
                }
        }
    }
}