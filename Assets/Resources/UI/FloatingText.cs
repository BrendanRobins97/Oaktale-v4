using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DamageType { Player, Enemy, Heal }
public class FloatingText : MonoBehaviour
{

    public GameObject floatingTextPrefab;
    public Color enemyDamageColor;
    public Color playerDamageColor;
    public Color playerHealColor;
    public static FloatingText Instance;
    // Use this for initialization
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public void SpawnText(Vector2 pos, int damage, DamageType damageType)
    {
        GameObject floatingText = Instantiate(floatingTextPrefab, pos, Quaternion.identity);
        Text damageText = floatingText.GetComponentInChildren<Text>();
        damageText.text = damage.ToString();
        damageText.color = GetColor(damageType);
        Destroy(floatingText, 1.9f);
    }

    private Color GetColor(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Player:
                return playerDamageColor;
            case DamageType.Enemy:
                return enemyDamageColor;
            case DamageType.Heal:
                return playerHealColor;
            default:
                return playerDamageColor;
        }
    }
}
