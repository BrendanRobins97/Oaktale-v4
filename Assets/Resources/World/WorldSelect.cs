using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour {

    public GameObject ButtonPrefab;
    private string fileExtension = ".world";
    private string worldFolder = "world";
    private Button[] worldButtons;

    private void OnEnable()
    {
        CreateWorldButtons();
    }
    private void OnDisable()
    {
        DeleteWorldButtons();
    }
    private void CreateWorldButtons()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/" + worldFolder + "/", "*" + fileExtension);
        worldButtons = new Button[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            string path = Path.GetFileNameWithoutExtension(files[i]);
            Button button = Instantiate(ButtonPrefab, gameObject.transform).GetComponent<Button>();

            button.GetComponentInChildren<Text>().text = path;
            button.GetComponent<RectTransform>().localPosition = new Vector3(0, -80 * i);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 60);
            button.onClick.AddListener(() => {
                WorldSave.LoadWorld(path);
                StartGame();
            });
            worldButtons[i] = button;
        }
    }
    private void DeleteWorldButtons()
    {
        for (int i = 0; i < worldButtons.Length; i++)
        {
            if (worldButtons[i].gameObject != null)
            {
                Destroy(worldButtons[i].gameObject);
            }
            
        }
    }
    private void StartGame()
    {
        SceneManager.LoadScene("Scenes/MainScene");
        
    }
}
