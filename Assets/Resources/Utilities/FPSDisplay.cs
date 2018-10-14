using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour {

    float deltaTime = 0f;
    float passedTime = 0f;
    float counter = 0f;
    float displayTime = 0f;
    int frameAmount = 60;


    void Update()
    {
        passedTime += Time.unscaledDeltaTime;
        if (counter == 0)
        {
            displayTime = passedTime;
            passedTime = 0f;
        }

        counter = (counter + 1) % frameAmount;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(.8f * w, h * 10 / 100, 0.2f * w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        
        float fps = 1.0f / displayTime * frameAmount;
        string text = string.Format("{0:0.0} fps", fps);
        GUI.Label(rect, text, style);
    }
}
