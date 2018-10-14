using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public float Liquid { get; set; }

    public Color Color;
    public Color DarkColor;

    public Cell Top { get; set; }
    public Cell Bottom { get; set; }
    public Cell Left { get; set; }
    public Cell Right { get; set; }

    SpriteRenderer LiquidSprite;

    public void Set(int x, int y)
    {

    }

    // Use this for initialization
    void Start () {
        LiquidSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        LiquidSprite.color = Color.Lerp(Color, DarkColor, Liquid / 4f);
    }
}
