using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalInventory : Inventory {

    public bool open = false;
    private GameObject obj;

    [SerializeField]
    private float maxDistance = 10f;
    protected override void Awake()
    {
        base.Awake();
        obj = gameObject;
    }
    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!open)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }

        }
    }

    public virtual void OpenInventory()
    {
        UIHandler uiHandler = playerInfo.uiHandler;
        inventoryPanel.transform.SetParent(uiHandler.externalInventory.transform);
        uiHandler.externalInventory.gameObject.SetActive(true);
        uiHandler.externInv = this;
        inventoryPanel.SetActive(true);
        open = true;
    }

    public virtual void CloseInventory()
    {
        UIHandler uiHandler = playerInfo.uiHandler;
        inventoryPanel.transform.SetParent(transform);
        uiHandler.externalInventory.SetActive(false);
        inventoryPanel.SetActive(false);
        open = false;
    }
    protected virtual void Update()
    {
        if (Mathf.Abs((playerInfo.transform.position - obj.transform.position).magnitude) > maxDistance)
        {
            CloseInventory();
        }
    }


}
