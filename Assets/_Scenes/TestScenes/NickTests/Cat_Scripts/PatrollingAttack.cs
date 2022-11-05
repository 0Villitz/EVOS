using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingAttack : EnemyAttack
{
    CatPlayerMoveControls playerMove;
    public float forceX;
    public float forceY;
    public float duration;
    public override void SpecialAttack()
    {
        //base.SpecialAttack();

        playerMove = playerStats.GetComponentInParent<CatPlayerMoveControls>();
        StartCoroutine(playerMove.KnockBack(forceX, forceY, duration, transform.parent));
    }
}
