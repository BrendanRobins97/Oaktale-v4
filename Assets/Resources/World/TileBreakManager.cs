using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBreakManager : MonoBehaviour {

    private static int numTileBreakSprites = 4;

    private float healthPercent;

    public float maxHealth;
    public float currentHealth;
    public Int2 position;
    public int tileInfo;
    public Sprite[] tileBreakSheet;

    private SpriteRenderer spriteRenderer;
    public float respawnRate = 0.25f;


    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        healthPercent = currentHealth / maxHealth;
        if (healthPercent >= 1)
        {
            spriteRenderer.sprite = null;
        }
        else if (healthPercent >= 0.75)
        {
            spriteRenderer.sprite = tileBreakSheet[0 + tileInfo];
        }
        else if (healthPercent >= 0.5)
        {
            spriteRenderer.sprite = tileBreakSheet[9 + tileInfo];
        }
        else if (healthPercent >= 0.25)
        {
            spriteRenderer.sprite = tileBreakSheet[18 + tileInfo];
        }
        else if (healthPercent > 0)
        {
            spriteRenderer.sprite = tileBreakSheet[27 + tileInfo];
        }
        currentHealth += respawnRate * Time.deltaTime;
    }
}
