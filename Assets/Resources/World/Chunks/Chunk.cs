using UnityEngine;

public abstract class Chunk : MonoBehaviour
{
    // Keeps track of its position
    protected Int2 pos;
    protected World currentWorld;

    public Int2 Position
    {
        get { return pos; }
        set
        {
            pos = value;
            UpdateChunk();  // Update chunk if position changes
        }
    }

    public abstract void UpdateChunk();

    public void UpdateWorld()
    {
        currentWorld = GameManager.Get<WorldManager>().currentWorld;
    }
}