using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCall : EventCall
{
    public override void DealPhysicalDamage()
    {
        CombatManager.Instance.DealDamageToCurrentEntity(CombatPlayer.Instance.GetDamage(false), false);

    }

    public override void DealMagicDamage()
    {
        CombatManager.Instance.DealDamageToCurrentEntity(CombatPlayer.Instance.GetDamage(true), true);
    }

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
}
