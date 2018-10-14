/// 
/// Marshals events and data between ConsoleController and uGUI.
/// Copyright (c) 2014-2015 Eliot Lash
/// 
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;
public class ConsoleView : MonoBehaviour
{
    ConsoleController console = new ConsoleController();

    bool didShow = false;

    public GameObject viewContainer; //Container for console view, should be a child of this GameObject
    public Text logTextArea;
    public InputField inputField;

    public PlayerInfo playerInfo;
    public bool visible = false;

    void Start()
    {
        if (console != null)
        {
            console.visibilityChanged += onVisibilityChanged;
            console.logChanged += onLogChanged;
            GameManager.Set(this);
        }
        updateLogStr(console.log);
        console.playerInfo = playerInfo;
    }

    ~ConsoleView()
    {
        console.visibilityChanged -= onVisibilityChanged;
        console.logChanged -= onLogChanged;
    }

    void Update()
    {
        //Toggle visibility when tilde key pressed
        if (Input.GetKeyDown(";"))
        {
            toggleVisibility();
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.ActivateInputField();

        }

        //Toggle visibility when 5 fingers touch.
        if (Input.touches.Length == 5)
        {
            if (!didShow)
            {
                toggleVisibility();
                didShow = true;
            }
        }
        else
        {
            didShow = false;
        }
    }

    void toggleVisibility()
    {
        setVisibility(!viewContainer.activeSelf);
        visible = viewContainer.activeSelf;
    }

    void setVisibility(bool visible)
    {
        viewContainer.SetActive(visible);
    }

    void onVisibilityChanged(bool visible)
    {
        setVisibility(visible);
    }

    void onLogChanged(string[] newLog)
    {
        updateLogStr(newLog);
    }

    void updateLogStr(string[] newLog)
    {
        if (newLog == null)
        {
            logTextArea.text = "";
        }
        else
        {
            logTextArea.text = string.Join("\n", newLog);
        }
    }

    /// 
    /// Event that should be called by anything wanting to submit the current input to the console.
    /// 
    public void runCommand()
    {
        console.runCommandString(inputField.text);
        inputField.text = "";
    }

}