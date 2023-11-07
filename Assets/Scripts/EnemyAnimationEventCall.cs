using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventCall : EventCall
{
    private Entity entity;

    private void Awake()
    {
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
}
