using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    public float damage = 20f;

    [SerializeField] private DamageType damageType;
    [SerializeField] private List<string> tags;
    [SerializeField] private float baseForce = 15f;

    private List<int> enemiesHit = new List<int>();

    private void OnEnable()
    {
        enemiesHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.enabled) return;
        GameObject enemy = collision.gameObject;
        foreach (string tag in tags)
        {
            if (enemy.tag.Equals(tag))
            {
                if (enemy.GetComponent<Character>().Invicible) return;
                if (enemiesHit.Contains(enemy.GetHashCode())) return;

                enemiesHit.Add(enemy.GetHashCode());
                Vector2 hitDirection = new Vector2(Mathf.Sign(transform.lossyScale.x) * baseForce, baseForce);
                enemy.GetComponent<HealthManager>().Damage(damage);
                enemy.GetComponent<Character>().HitCharacter(hitDirection);
                FloatingText.Instance.SpawnText(enemy.transform.position + Vector3.up, (int)damage, damageType);
            }
        }
    }

}
