using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour {

    public Material ambientLightMaterial;
    public float minLight = .04f;


    [Range(0, 100)]
    [SerializeField]
    private float TimeChangeSpeed = 10f;

    private void Awake()
    {
        GameManager.Set(this);
    }

    private void OnGUI()
    {
        GUIStyle customButton = new GUIStyle("button");
        customButton.fontSize = 30;
        if (GUI.Button(new Rect(20, 100, 200, 80), "Night", customButton))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeToNight());
        }
        if (GUI.Button(new Rect(20, 200, 200, 80), "Day", customButton))
        {
            StopAllCoroutines();
            StartCoroutine(ChangeToDay());
        }
        if (GUI.Button(new Rect(20, 300, 200, 80), "Save", customButton))
        {
            WorldSave.SaveWorld(ref GameManager.Get<WorldManager>().currentWorld);
        }
    }

    public void SetDay()
    {
        ambientLightMaterial.color = new Color(1, 1, 1);
    }

    public void SetNight()
    {
        ambientLightMaterial.color = new Color(minLight, minLight, minLight);
    }

    IEnumerator ChangeToNight()
    {
        while (ambientLightMaterial.color.r >= minLight)
        {
            ambientLightMaterial.color = ambientLightMaterial.color - new Color(.005f, .005f, .005f);
            yield return new WaitForSeconds(.1f / TimeChangeSpeed);
        }
    }

    IEnumerator ChangeToDay()
    {
        while (ambientLightMaterial.color.r <= 1)
        {
            ambientLightMaterial.color = ambientLightMaterial.color + new Color(.005f, .005f, .005f);
            yield return new WaitForSeconds(.1f / TimeChangeSpeed);
        }
    }
}
