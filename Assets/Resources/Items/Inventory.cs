using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public delegate void InventoryAction();
    public static event InventoryAction OnInventoryChanged;

    public List<Slot> slots = new List<Slot>();
    public List<Item> items = new List<Item>();

    public GameObject inventoryPanelPrefab;
    public GameObject slotPrefab;
    public GameObject itemIconPrefab;

    [SerializeField]
    protected GameObject inventoryPanel;
    protected PlayerInfo playerInfo;

    public int maxSlots = 32;

    protected virtual void Awake()
    {
        //ItemDatabase.ConstructDatabase();
        playerInfo = FindObjectOfType<PlayerInfo>();
        SetSlotUI();
        

    }

    public virtual void InventoryChanged()
    {
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
        
    }

    protected virtual void SetSlotUI()
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

        //inventoryPanel.SetActive(false);
        InventoryChanged();
    }
    

    #region ItemManagement

    public virtual void AddItem(Item item, int amount = 1, int slot = 0)
    {
        if (!ItemDatabase.ValidID(item.id)) return;
        for (int i = slot; i < slots.Count; i++)
        {
            if (items[i].id == item.id)
            {
                ItemIcon itemObj = slots[i].GetComponentInChildren<ItemIcon>();
                itemObj.amount += amount;
                itemObj.Refresh();
                return;
            }
        }
        for (int i = slot; i < slots.Count; i++)
        {
            if (items[i].id == 0)
            {
                items[i] = item;
                GameObject itemObj = Instantiate(itemIconPrefab, slots[i].transform);
                itemObj.GetComponent<ItemIcon>().Initialize(this, item, amount, i);
                return;
            }
        }
        InventoryChanged();
    }
    public virtual void AddItem(int id, int amount = 1, int slot = 0)
    {
        AddItem(new Item(id), amount, slot);
    }

    public virtual void RemoveItem(Item item, int amount = 1, int slot = 0)
    {
        if (!ItemDatabase.ValidID(item.id)) return;
        for (int i = slot; i < slots.Count; i++)
        {
            if (items[i].id == item.id)
            {
                ItemIcon itemObj = slots[i].GetComponentInChildren<ItemIcon>();
                itemObj.amount -= amount;
                if (itemObj.amount <= 0)
                {
                    Destroy(itemObj.gameObject);
                    items[i].id = 0;
                }
                itemObj.Refresh();
                return;
            }
        }
        InventoryChanged();
    }
    public virtual void RemoveItem(int id, int amount = 1, int slot = 0)
    {
        RemoveItem(new Item(id), amount, slot);
    }

    public virtual bool HasEnoughItem(Item item, int amount = 1)
    {
        if (!ItemDatabase.ValidID(item.id)) return false;
        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i].id == item.id)
            {
                ItemIcon itemObj = slots[i].GetComponentInChildren<ItemIcon>();

                return itemObj.amount >= amount;
            }
        }
        return false;
    }
    public virtual void HasEnoughItem(int id, int amount = 1)
    {
        HasEnoughItem(new Item(id), amount);
    }

    public virtual void AddAllItems(int count = 999)
    {
        foreach (ItemData item in ItemDatabase.itemDictionary.Values)
        {
            if (item.id == 0) continue;
            AddItem(new Item(item.id), item.stackSize);
        }
    }

    #endregion

    public virtual void PrintInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log("Slot " + i + ": " + items[i].id);
        }
    }
}
