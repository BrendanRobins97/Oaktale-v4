using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
{
    public Item item;
    public int amount;

    public int slot;
    public Inventory inventory;

    private Vector2 offset;

	
	public void Clear()
    {
        item = new Item(0);
        amount = 0;
    }

    public void Refresh()
    {
        if (item.id == 0)
        {
            transform.GetComponentInChildren<Text>().text = "";
            transform.GetChild(0).gameObject.SetActive(false);

        } else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            if (amount > 1)
            {
                transform.GetComponentInChildren<Text>().text = amount.ToString();
                
            } else
            {
                transform.GetComponentInChildren<Text>().text = "";
            }

            transform.GetChild(0).GetComponent<Image>().sprite = ItemDatabase.GetItemData(item).icon;
        }

    }

    public void Initialize(Inventory inventory, Item item, int amount, int slot)
    {
        this.inventory = inventory;
        this.item = item;
        this.amount = amount;
        this.slot = slot;
        Refresh();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // What happens when we start to drag the item.
        offset = eventData.position - new Vector2(transform.position.x, transform.position.y);


        this.transform.SetParent(this.transform.parent.parent);
        this.transform.position = eventData.position - offset;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position - offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inventory.slots[slot].transform);
        this.transform.position = inventory.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        inventory.InventoryChanged();
    }

    public void Print()
    {
        Debug.Log("Item: " + item.id + " Amount: " + amount + " Slot: " + slot);
    }
}
