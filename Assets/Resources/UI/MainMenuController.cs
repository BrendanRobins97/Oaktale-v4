using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour {

    [SerializeField]
    private GameObject home;
    [SerializeField]
    private GameObject worldSelect;
    [SerializeField]
    private GameObject worldCreation;
    [SerializeField]
    private GameObject characterSelect;

    [SerializeField]
    private GameObject newWorld;

    [SerializeField]
    private GameObject worldName;
    [SerializeField]
    private GameObject worldType;

    public void HomeScreen()
    {
        home.SetActive(true);
        worldSelect.SetActive(false);
        worldCreation.SetActive(false);
        characterSelect.SetActive(false);
    }

    public void WorldSelectScreen()
    {
        home.SetActive(false);
        worldSelect.SetActive(true);
        worldCreation.SetActive(false);
        characterSelect.SetActive(false);
    }

    public void WorldCreationScreen()
    {
        home.SetActive(false);
        worldSelect.SetActive(false);
        worldCreation.SetActive(true);
        characterSelect.SetActive(false);
    }

    public void CharacterSelectScreen()
    {
        home.SetActive(false);
        worldSelect.SetActive(false);
        worldCreation.SetActive(false);
        characterSelect.SetActive(true);
    }

    /*
    public void GenerateNewWorld()
    {
        string name = worldName.GetComponent<Text>().text;
        World newWorld; //= new World(name, 1000, 800);
        switch (worldType.GetComponent<Dropdown>().value)
        {
            case 0:
                TerrainGenerator.GenerateNormalWorld(ref newWorld);
                break;
            case 1:
                TerrainGenerator.GenerateFlatWorld(ref newWorld);
                break;
            default:
                TerrainGenerator.GenerateNormalWorld(ref newWorld);
                break;
        }
        WorldSave.SaveWorld(ref newWorld);
        WorldSelectScreen();

    }
    */
}
