using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventCall : EventCall
{
    public override void DealDamageCall()
    {
        m_entity.AttackEntity();
    }

    public override void OnActionFinished()
    {
        if (!animator) return;
        m_entity.PlayAnimation(IDLE_ANIMATION);
        CombatManager.Instance.OnTurnFinished();
    }

    public override void OnAnimationFinish()
    {
        if (!animator) return;
       m_entity.PlayAnimation(IDLE_ANIMATION);
    }

    //public override void OnAnimationFinish()
    //{
    //    throw new System.NotImplementedException();
    //}
}
