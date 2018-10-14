using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public int slotNumber;
    public Inventory inventory;

    public List<ArmorType> acceptableArmorTypes;
    public List<int> acceptableItems;

    public void OnDrop(PointerEventData eventData)
    {
        ItemIcon droppedItem = eventData.pointerDrag.GetComponent<ItemIcon>();
        Inventory droppedItemInv = droppedItem.inventory;

        if (droppedItemInv == inventory && slotNumber == droppedItem.slot)
        {
            return;
        }
        if (acceptableArmorTypes.Count > 0)
        {
            for (int i = 0; i < acceptableArmorTypes.Count; i++)
            {

                if (ItemDatabase.GetArmor(droppedItemInv.items[droppedItem.slot].id) != null &&
                    ItemDatabase.GetArmor(droppedItemInv.items[droppedItem.slot].id).armorType == acceptableArmorTypes[i])
                {
                    break;
                }
                if (i == acceptableArmorTypes.Count - 1)
                {
                    return;
                }
            }

        }
        if (acceptableItems.Count > 0)
        {
            for (int i = 0; i < acceptableItems.Count; i++)
            {
                if (droppedItemInv.items[droppedItem.slot].id == acceptableItems[i])
                {
                    break;
                }
                if (i == acceptableItems.Count - 1)
                {
                    return;
                }
            }

        }
        if (inventory.items[slotNumber].id == droppedItemInv.items[droppedItem.slot].id)
        {
            GetComponentInChildren<ItemIcon>().amount += droppedItem.amount;
            GetComponentInChildren<ItemIcon>().Refresh();
            Destroy(droppedItem.gameObject);
            
        } else if (inventory.items[slotNumber].id == 0)
        {
            droppedItemInv.items[droppedItem.slot] = new Item();
            droppedItem.inventory = inventory;
            inventory.items[slotNumber] = droppedItem.item;

            droppedItem.slot = slotNumber;
        } else
        {
            Transform item = this.transform.GetChild(0);
            item.GetComponent<ItemIcon>().slot = droppedItem.slot;
            item.transform.SetParent(droppedItemInv.slots[droppedItem.slot].transform);
            item.transform.position = droppedItemInv.slots[droppedItem.slot].transform.position;

            inventory.items[droppedItem.slot] = item.GetComponent<ItemIcon>().item;
            droppedItem.inventory = inventory;
            item.GetComponent<ItemIcon>().inventory = droppedItemInv;
            inventory.items[slotNumber] = droppedItem.item;
            droppedItem.slot = slotNumber;

            
        }
        
        
    }
}
