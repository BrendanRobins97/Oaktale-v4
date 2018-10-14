using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceInventory : ExternalInventory {

    [SerializeField]
    private Slot inputSlot;
    [SerializeField]
    private Slot fuelSlot;
    [SerializeField]
    private Slot outputSlot;

    private Item input = new Item(51);
    private Item fuel = new Item(53);
    private Item output = new Item(1);

    protected override void SetSlotUI()
    {
        inventoryPanel = transform.GetChild(0).gameObject;
        slots.Add(inputSlot);
        slots.Add(fuelSlot);
        slots.Add(outputSlot);

        inputSlot.inventory = this;
        fuelSlot.inventory = this;
        outputSlot.inventory = this;

        inputSlot.slotNumber = 0;
        fuelSlot.slotNumber = 1;
        outputSlot.slotNumber = 2;

        inputSlot.acceptableItems.Add(51);
        inputSlot.acceptableItems.Add(52);

        fuelSlot.acceptableItems.Add(53);

        outputSlot.acceptableItems.Add(0);
        items.Add(new Item());
        items.Add(new Item());
        items.Add(new Item());

        inventoryPanel.transform.SetAsFirstSibling();
        inventoryPanel.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (items[0] == input && items[1] == fuel && (items[2].Empty() || items[2] == output))
        {
            Item outputItem = new Item(1);
            if (items[2] == outputItem)
            {
                slots[2].GetComponentInChildren<ItemIcon>().amount += 1;
                slots[2].GetComponentInChildren<ItemIcon>().Refresh();
            }
            else
            {
                items[2] = new Item(1);
                GameObject itemObj = Instantiate(itemIconPrefab, slots[2].transform);
                itemObj.GetComponent<ItemIcon>().Initialize(this, new Item(1), 1, 2);
            }
            
            RemoveItem(input);
            RemoveItem(fuel);
        }
    }

    

}
