using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventCall : EventCall
{
    private EnemyEntity enemyEntity;


    protected override void Awake()
    {
        base.Awake();
        enemyEntity = GetComponentInParent<EnemyEntity>();
    }
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


    public void OnSpellCast(int spellID)
    {
        enemyEntity.PerformSkill(spellID);
    }
    //public override void OnAnimationFinish()
    //{
    //    throw new System.NotImplementedException();
    //}

    public void OnHeal()
    {
        enemyEntity.OnHeal();
    }

    public void OnGuard()
    {
        m_entity.OnBuff(Enums.BuffType.defense);
        m_entity.OnTurnEnd();
    }
}
