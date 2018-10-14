using UnityEngine;
using System.Collections;

public class OffsetScroller : MonoBehaviour
{

    public float scrollSpeedX;
    public float scrollSpeedY;
    private Vector2 savedOffset;
    public new MeshRenderer renderer;
    public Transform player;

    void Start()
    {
        savedOffset = renderer.sharedMaterial.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        float x = Mathf.Repeat(transform.position.x * scrollSpeedX, 1);
        float y = Mathf.Repeat((transform.position.y -560)* scrollSpeedY, 1);
        Vector2 offset = new Vector2(x, y);
        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    void OnDisable()
    {
        renderer.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}