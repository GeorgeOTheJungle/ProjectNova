using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventCall : EventCall
{
    private Entity entity;

    protected override void Awake()
    {
        base.Awake();
        entity = GetComponentInParent<Entity>();
    }
    public override void DealMagicDamage()
    {
        CombatPlayer.Instance.ReceiveDamage(entity.GetDamage(true),true);
    }

    public override void DealPhysicalDamage()
    {
        CombatPlayer.Instance.ReceiveDamage(entity.GetDamage(false), false);
    }

    public override void OnActionFinished()
    {
        if (!animator) return;
        animator.Play(IDLE_ANIMATION);
        CombatManager.Instance.OnActionFinished();
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
