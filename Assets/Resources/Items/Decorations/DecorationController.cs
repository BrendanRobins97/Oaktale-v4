// File: DecorationController.cs
// Author: Brendan Robinson
// Date Created: 07/09/2018
// Date Last Modified: 07/24/2018
// Description: 

using UnityEngine;

public class DecorationController : MonoBehaviour
{
    public bool facingRight = true;
    public int id = 0;
    public Int2 position = Int2.zero;

    private void OnMouseDown()
    {
        if (GameManager.Get<PlayerInfo>().equip.GetComponentInChildren<Pickaxe>())
        {
            Debug.Log("test");

            GameManager.Get<WorldManager>().DeleteDecoration(position);
        }
    }
}