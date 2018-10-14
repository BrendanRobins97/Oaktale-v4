using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    public GameObject playerObject;
    public UIHandler uiHandler;
    public Inventory inventory;
    public ActionBar actionBar;
    public Player player;
    public GameObject equip;
    public DecorationManager decorationManager;
    public bool directionalMining = true;



    private void Awake()
    {
        GameManager.Set(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            directionalMining = !directionalMining;
        }
    }

}
