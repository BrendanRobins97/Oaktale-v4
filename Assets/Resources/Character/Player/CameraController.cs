using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float xOffset = 0f;
    public float yOffset = 0f;
    public float currentXOffset = 0f;
    public float currentYOffset = 0f;

    private Player player;
    private Camera cam;

    Vector2 input;
    Vector3 newPos;
    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        xOffset = Mathf.SmoothDamp(xOffset, 2 * input.x, ref currentXOffset, 0.5f);
        yOffset = Mathf.SmoothDamp(yOffset, 2 * input.y, ref currentYOffset, 0.5f);
        newPos = new Vector3(xOffset, yOffset, transform.position.z);
        transform.localPosition = newPos;
    }

    
}
