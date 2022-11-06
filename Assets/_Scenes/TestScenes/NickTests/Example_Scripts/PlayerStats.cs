using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth;
    public float health;

    public bool canTakeDamage = true;

    private Animator anim;
    private CatPlayerMoveControls playerMove;
    public Image healthUI;
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        playerMove = GetComponentInParent<CatPlayerMoveControls>();
        health = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {

        if (canTakeDamage)
        {

            health -= damage;
            anim.SetBool("Damage", true);
            playerMove.hasControl = false;
            //Debug.Log("Player took damage");

            UpdateHealthUI();
            
            if (health <= 0)
            {
                
                GetComponent<PolygonCollider2D>().enabled = false;
                GetComponentInParent<GatherInput>().DisableControls();


            }

            StartCoroutine(DamagePrevention());
            
        }

    }

    public void IncreaseHealth(float healAmount)
    {
        if (health + healAmount < maxHealth)
        {
            health += healAmount;
        }
        else
        {
            health = maxHealth;
        }

        UpdateHealthUI();
    }

    private IEnumerator DamagePrevention()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(0.15f);
        
        if (health > 0)
        {
            canTakeDamage = true;
            playerMove.hasControl = true;
            anim.SetBool("Damage", false);
            
        }
        else
        {
            anim.SetBool("Death", true);
            Debug.Log("The player is dead.");
        }
    }

    public void UpdateHealthUI()
    {
        healthUI.fillAmount = health / maxHealth;
    }

}
