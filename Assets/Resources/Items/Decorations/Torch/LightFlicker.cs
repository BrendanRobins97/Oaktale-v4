using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

    public int flickerSpeed = 8;
    public int xBaseScale = 40;
    public int yBaseScale = 40;
    public int xVariableScale = 4;
    public int yVariableScale = 4;

    void Update () {
        transform.localScale = new Vector3(xBaseScale + xVariableScale * Mathf.Cos(flickerSpeed * Time.fixedTime),
                                           yBaseScale + yVariableScale * Mathf.Cos(flickerSpeed * Time.fixedTime), 1);
	}
}
