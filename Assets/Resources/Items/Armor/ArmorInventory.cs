using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorInventory : Inventory {

    protected override void Awake()
    {
        base.Awake();
        OnInventoryChanged += playerInfo.actionBar.UpdateActionBarImages;
    }

    protected override void SetSlotUI()
    {
        Slot[] transforms = inventoryPanel.GetComponentsInChildren<Slot>();
        for (int i = 0; i < transforms.Length; i++)
        {
            items.Add(new Item());
            slots.Add(transforms[i]);
            slots[i].transform.SetParent(inventoryPanel.transform);
            slots[i].slotNumber = i;
            slots[i].inventory = this;
        }
        inventoryPanel.transform.SetAsFirstSibling();
        inventoryPanel.SetActive(false);
        InventoryChanged();
    }

    public override void AddItem(Item item, int amount = 1, int slot = 0)
    {
        base.AddItem(item, amount, slot);
        playerInfo.actionBar.UpdateActionBarImages();
    }

    public override void RemoveItem(Item item, int amount = 1, int slot = 0)
    {
        base.RemoveItem(item, amount);
        playerInfo.actionBar.UpdateActionBarImages();
    }
}
