using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCall : EventCall
{

    public void OnGuard()
    {
        CombatPlayer.Instance.GuardFromDamage();
    }

    public void OnEscape()
    {
        CombatPlayer.Instance.Escape();
    }

    public void OnReload()
    {
        CombatPlayer.Instance.Reload();
    }

    public void OnHeal()
    {
        CombatPlayer.Instance.Heal();
    }

    public void OnBuffReceived()
    {
        CombatPlayer.Instance.Buff();
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

    public override void DealDamageCall()
    {
        m_entity.AttackEntity();
    }
}
