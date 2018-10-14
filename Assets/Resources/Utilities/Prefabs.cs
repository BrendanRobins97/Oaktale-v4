using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{

    public GameObject pickup;

    public GameObject mouse;

    // Use this for initialization
    void Awake()
    {
        GameManager.Set(this);
    }

}
