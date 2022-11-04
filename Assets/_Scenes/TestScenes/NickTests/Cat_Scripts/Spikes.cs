using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float damage;

    public float forceX;
    public float forceY;
    public float duration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stats"))
        {
            collision.GetComponent<PlayerStats>().TakeDamage(damage);
            CatPlayerMoveControls playerMove = collision.GetComponentInParent<CatPlayerMoveControls>();

            StartCoroutine(playerMove.KnockBack(forceX, forceY, duration, transform));
        }

    }
}
