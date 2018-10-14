using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private Int2 currentMousePosition;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        currentMousePosition = Utilities.GetMousePosition();
        //spriteRenderer.sprite = ItemDatabase.GetBlock(block).icon;
        transform.position = new Vector3(currentMousePosition.x + 0.5f, currentMousePosition.y + 0.5f, -30);
    }
}
