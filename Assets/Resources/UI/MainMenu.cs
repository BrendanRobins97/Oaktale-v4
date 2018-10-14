using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    Canvas HomeCanvas;
    [SerializeField]
    Canvas WorldCanvas;
    [SerializeField]
    Canvas CharacterCanvas;


    public void NextScene()
    {
        SceneManager.LoadScene("Scenes/MainScene");
    }

    public void Home()
    {
        HomeCanvas.gameObject.SetActive(true);
        WorldCanvas.gameObject.SetActive(false);
        CharacterCanvas.gameObject.SetActive(false);
    }

    public void WorldSelect()
    {
        HomeCanvas.gameObject.SetActive(false);
        WorldCanvas.gameObject.SetActive(true);
        CharacterCanvas.gameObject.SetActive(false);
    }

    public void CharacterSelect()
    {
        HomeCanvas.gameObject.SetActive(false);
        WorldCanvas.gameObject.SetActive(false);
        CharacterCanvas.gameObject.SetActive(true);
    }
}
