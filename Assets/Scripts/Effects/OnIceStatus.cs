using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class OnIceStatus : StatusEffect
{
    private int basePhysicalStat;
    private int baseMagicalStat;
    public override void OnEffectStart(Entity entity)
    {
        base.OnEffectStart(entity);
        basePhysicalStat = entity.entityStats.physicalArmor;
        baseMagicalStat = entity.entityStats.magicArmor;
    }

    public override void ApplyEffect()
    {
        if (currentEntity == null) return;
        OnEffectUse();
        currentEntity.entityStats.physicalArmor -= Mathf.CeilToInt(currentEntity.entityStats.physicalArmor * Constants.ICE_RESISTANCE_REDUCTION);
        currentEntity.entityStats.magicArmor -= Mathf.CeilToInt(currentEntity.entityStats.magicArmor * Constants.ICE_RESISTANCE_REDUCTION);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        if (currentEntity == null) return;
        currentEntity.entityStats.physicalArmor = basePhysicalStat;
        currentEntity.entityStats.magicArmor = baseMagicalStat;
    }
}
