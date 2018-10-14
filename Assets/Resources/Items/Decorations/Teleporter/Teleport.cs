using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public int worldIndex;
    public Int2 teleportPosition = new Int2(64, 300);

    public bool playerInTeleporter = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && playerInTeleporter)
        {
            TeleportPlayer();
        }
    }
    public void TeleportPlayer()
    {
        GameManager.Get<WorldManager>().SwitchWorld(worldIndex, teleportPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerInTeleporter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            playerInTeleporter = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    void OnCollisionExit(Collision collision)
    {
        
    }

}
