using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireStatus : StatusEffect
{
    public override void ApplyEffect()
    {
        if (effectActive == false) return;
        OnEffectUse();
        float damageDealt = currentEntity.entityStats.health * Constants.FIRE_EFFECT_DAMAGE;
        currentEntity.OnDamageTaken(Mathf.CeilToInt(damageDealt),Enums.DamageType.magical,Enums.StatusEffectType.none,false);

    }
}
