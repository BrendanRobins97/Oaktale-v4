using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour {

    public GameObject craftingUI;
    public GameObject craftingPanel;
    public GameObject bluePrintIconPrefab;

    public Blueprint[] bluePrints;
    public Inventory inventory;

	void Start () {
        inventory = GetComponent<Inventory>();
        bluePrints = Resources.LoadAll<Blueprint>("Items/Blueprints");
        for (int i = 0; i < bluePrints.Length; i++)
        {
            AddBluePrintToUI(bluePrints[i]);
        }
	}


    private void AddBluePrintToUI(Blueprint blueprint)
    {
        GameObject newBlueprint = Instantiate(bluePrintIconPrefab, craftingPanel.transform);
        newBlueprint.GetComponent<Image>().sprite = blueprint.output.icon;

    }

    public void Craft(Blueprint blueprint)
    {
        for (int i = 0; i < blueprint.inputs.Length; i++)
        {
            if (!inventory.HasEnoughItem(new Item(blueprint.inputs[i].id), blueprint.inputAmounts[i]))
            {
                Debug.Log("Not enough items bruh");
                return;
            }
        }
        for (int i = 0; i < blueprint.inputs.Length; i++)
        {
            inventory.RemoveItem(new Item(blueprint.inputs[i].id), blueprint.inputAmounts[i]);
        }
        inventory.AddItem(new Item(blueprint.output.id), blueprint.outputAmount);
    }
}
