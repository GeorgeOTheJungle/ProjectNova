using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventCall : EventCall
{
    public override void DealDamageCall()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActionFinished()
    {
        if (!animator) return;
        animator.Play(IDLE_ANIMATION);
        CombatManager.Instance.OnTurnFinished();
    }

    public override void OnAnimationFinish()
    {
        if (!animator) return;
        animator.Play(IDLE_ANIMATION);
    }

    //public override void OnAnimationFinish()
    //{
    //    throw new System.NotImplementedException();
    //}
}
