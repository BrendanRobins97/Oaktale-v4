using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour {

    public PlayerInfo playerInfo;
    
    public GameObject actionSlot;
    public GameObject highlight;

    public GameObject[] actionSlots;
    
    public int index;
    private int prevIndex;

    private Inventory inventory;
    private RectTransform highlightRect;

    void Awake () {
        inventory = playerInfo.inventory;
        actionSlots = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            actionSlots[i] = Instantiate(actionSlot, transform);
            actionSlots[i].GetComponentInChildren<Text>().text = (i+1).ToString();

        }
        highlightRect = Instantiate(highlight, transform).GetComponent<RectTransform>();
        index = 0;
        prevIndex = 1;
        UpdateActionBarImages();
        
    }
	
    public void UpdateActionBarImages()
    {
        for (int i = 0; i < 10; i++)
        {
            Transform image = actionSlots[i].transform.GetChild(0) ;

            if (inventory.items[i].id == 0)
            {
                image.gameObject.SetActive(false);
                actionSlots[i].transform.GetChild(2).GetComponent<Text>().text = "";
            } else
            {
                image.gameObject.SetActive(true) ;
                image.GetComponent<Image>().sprite = ItemDatabase.GetItemData(inventory.items[i].id).icon;
                int amount = inventory.slots[i].GetComponentInChildren<ItemIcon>().amount;
                if (amount > 1)
                {
                    actionSlots[i].transform.GetChild(2).GetComponent<Text>().text = amount.ToString();
                } else
                {
                    actionSlots[i].transform.GetChild(2).GetComponent<Text>().text = "";
                }
                
            }
            
        }
        
    }
	// Update is called once per frame
	void Update () {
        if (!GameManager.Get<ConsoleView>().visible)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                index = Utilities.Mod(index - 1, 10);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                index = Utilities.Mod(index + 1, 10);
            }
            if (Input.GetKey(KeyCode.Alpha1))
            {
                index = 0;
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                index = 1;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                index = 2;
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                index = 3;
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                index = 4;
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                index = 5;
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                index = 6;
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                index = 7;
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                index = 8;
            }
            if (Input.GetKey(KeyCode.Alpha0))
            {
                index = 9;
            }
        }
        
        if (prevIndex != index)
        {
            ItemDatabase.GetItemData(inventory.items[prevIndex].id).Deactivate(playerInfo);
            ItemDatabase.GetItemData(inventory.items[index].id).Activate(playerInfo);
            prevIndex = index;
        }
        highlightRect.anchoredPosition = new Vector3(4 + index * 72, 0, 0);

    }
}
