using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCall : EventCall
{

    public void OnGuard()
    {
        m_entity.OnBuff(Enums.BuffType.defense);
    }

    public void OnEscape()
    {
        CombatManager.Instance.OnPlayerEscape();
    }

    public void OnReload()
    {
        m_entity.OnResourceGain(Enums.ResourceType.ammo, 999);
    }

    public void OnHeal()
    {
        m_entity.OnHeal();
    }

    public void OnBuffReceived()
    {
        m_entity.OnBuff(Enums.BuffType.offense);
    }

    public override void OnActionFinished()
    {
        if (!animator) return;
        m_entity.PlayAnimation(IDLE_ANIMATION);
        m_entity.Next();
        //CombatManager.Instance.OnTurnFinished();
    }

    public override void OnAnimationFinish()
    {
        if (!animator) return;
        m_entity.PlayAnimation(IDLE_ANIMATION);
    }

    public override void DealDamageCall()
    {
        m_entity.AttackEntity();
    }

    
}
