using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponentInChildren<PlayerStats>();

            playerStats.IncreaseHealth(healAmount);
            Destroy(gameObject);

        }

    }
}
