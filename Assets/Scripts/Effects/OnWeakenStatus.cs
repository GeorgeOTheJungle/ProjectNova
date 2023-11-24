using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWeakenStatus : StatusEffect
{
    private int basePhysicalDamage;
    private int baseMagicalDamage;
    public override void OnEffectStart(Entity entity)
    {
        base.OnEffectStart(entity);
        basePhysicalDamage = entity.entityStats.physicalDamage;
        baseMagicalDamage = entity.entityStats.magicDamage;
    }
    public override void ApplyEffect()
    {
        if (currentEntity == null) return;
        OnEffectUse();
        currentEntity.entityStats.physicalDamage -= Mathf.CeilToInt(currentEntity.entityStats.physicalDamage * Constants.WEAK_DAMAGE_REDUCTION);
        currentEntity.entityStats.magicDamage -= Mathf.CeilToInt(currentEntity.entityStats.magicDamage * Constants.WEAK_DAMAGE_REDUCTION);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        if (currentEntity == null) return;
        currentEntity.entityStats.physicalDamage = basePhysicalDamage;
        currentEntity.entityStats.magicDamage = baseMagicalDamage;
    }

}
