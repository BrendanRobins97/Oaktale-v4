using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public GameObject cellPrefab;

    public Cell[,] cells;

    public int width;
    public int height;

	// Use this for initialization
	void Start () {
        cells = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = Instantiate(cellPrefab, new Vector2(x, y), Quaternion.identity, transform).GetComponent<Cell>();
                cells[x, y].Liquid = 0f;
            }
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}
}
