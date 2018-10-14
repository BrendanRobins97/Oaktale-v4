using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUI : MonoBehaviour {

    // Create a public variable where we can assign the GUISkin
    public GUISkin customSkin;

    private bool toggleTxt;
    private int toolbarInt = 0;
    private string[] toolbarStrings = { "Toolbar1", "Toolbar2", "Toolbar3" };
    private int selGridInt = 0;
    private string[] selStrings = { "Grid 1", "Grid 2", "Grid 3", "Grid 4" };
    private float hSliderValue = 0f;
    private float hSbarValue = 0f;

    // Apply the Skin in our OnGUI() function
    private void OnGUI()
    {
        GUI.skin = customSkin;

        GUI.BeginGroup(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300));
        GUI.Box(new Rect(0, 0, 300, 300), "This is the title of a box");
        GUI.Button(new Rect(0, 25, 100, 20), "I am a button");
        GUI.Label(new Rect(0, 50, 100, 20), "I'm a Label!");
        toggleTxt = GUI.Toggle(new Rect(0, 75, 200, 30), toggleTxt, "I am a Toggle button");
        toolbarInt = GUI.Toolbar(new Rect(0, 110, 250, 25), toolbarInt, toolbarStrings);
        selGridInt = GUI.SelectionGrid(new Rect(0, 160, 200, 40), selGridInt, selStrings, 2);
        hSliderValue = GUI.HorizontalSlider(new Rect(0, 210, 100, 30), hSliderValue, 0.0f, 1.0f);
        hSbarValue = GUI.HorizontalScrollbar(new Rect(0, 230, 100, 30), hSbarValue, 1.0f, 0.0f, 10.0f);
        GUI.EndGroup();
    }
}
