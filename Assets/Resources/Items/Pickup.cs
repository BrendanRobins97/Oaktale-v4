// File: Pickup.cs
// Author: Brendan Robinson
// Date Created: 05/18/2018
// Date Last Modified: 07/15/2018
// Description: 

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Pickup : MonoBehaviour
{
    private static float maxFrames = 10f;

    public Item item;
    public int amount;
    public float randomSpawnForce = 80f;

    private Transform player;
    private Rigidbody2D rb;
    public void Initialize(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
        GetComponent<BoxCollider2D>().enabled = true;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().sprite = ItemDatabase.GetItemData(item).icon;
        rb.AddForce(new Vector2(Random.Range(-randomSpawnForce, randomSpawnForce), Random.Range(0, randomSpawnForce)));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player = collision.gameObject.transform;
            StartCoroutine(goToPlayer());
        }

        /*
        if (collision.gameObject.tag.Equals("Pickup"))
        {
            Pickup pickup = collision.gameObject.GetComponent<Pickup>();
            if (pickup.item.id == item.id && rngValue < pickup.rngValue)
            {
                pickup.amount += amount;
                StopAllCoroutines();
                SimplePool.Despawn(gameObject);
            }
        }
        */
    }

    private IEnumerator goToPlayer()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        // Add force towards player
        float distance = 10;
        int count = 0;
        do
        {
            Vector3 dir = player.position - transform.position;
            distance = dir.magnitude;
            rb.velocity = dir.normalized * (5 + 2 * count);
            count++;
            yield return new WaitForFixedUpdate();
        } while (distance > 3f || count <= maxFrames);

        // Add item to inventory and destroy pickup
        player.GetComponent<Player>().playerInfo.inventory.AddItem(item, amount);
        SimplePool.Despawn(gameObject);
    }
}