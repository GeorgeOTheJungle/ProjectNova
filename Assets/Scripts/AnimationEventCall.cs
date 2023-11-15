using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCall : EventCall
{
    private PlayerEntity player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<PlayerEntity>();
    }
    public void OnGuard()
    {
        m_entity.OnBuff(Enums.BuffType.defense);
        m_entity.Next();
    }

    public void OnEscape()
    {
        CombatManager.Instance.OnPlayerEscape();
    }

    public void OnReload()
    {
        m_entity.OnResourceGain(Enums.ResourceType.ammo, 999, RegenStyle.None);
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
    }

    public override void OnAnimationFinish()
    {
        if (!animator) return;
        m_entity.PlayAnimation(IDLE_ANIMATION);
    }

    public override void DealDamageCall() => m_entity.AttackEntity();

    public void RegenEnergy(RegenStyle regenStyle)
    {
        m_entity.OnResourceGain(Enums.ResourceType.energy, 15, regenStyle);
    }

    public void OnPlayerDeath()
    {
        CombatManager.Instance.OnPlayerDefeat();
    }


}
