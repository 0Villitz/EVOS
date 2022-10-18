using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    public float health;

    public bool canTakeDamage = true;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        // play hurt anim

        if (health <= 0)
        {
            GetComponent<PolygonCollider2D>().enabled = false;
            GetComponentInChildren<GatherInput>().DisableControls();
            Debug.Log("The player is dead.");
        }
    }

}
