using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
[CreateAssetMenu(fileName ="New Skill",menuName ="Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill data"), Space(10)]
    public bool unlocked;
    [Min(0)] public int level = 0;
    public bool isSelfTarget;
    public string skillName; // Use it for UI.
    [TextArea] public string skillDescription;

    public int initialUnlockCost;
    public float baseDamage; // Total damage the skill will do,
    private float initialDamage;
    public float damageUpgradeAmount;
    public DamageType damageType;
    public ResourceType resourceType;
    public TargetingStyle targetingStyle;
    public int resourceAmount;

    public string animationKey = ""; // Use this to call the animation you need on the entity

    [Space(10)]
    public Sprite smallIcon;
    public Sprite largeIcon;

    private int baseXPCost = 200;

    public void Initialize()
    {
        initialDamage = baseDamage;
        level = 0;
        unlocked = false;
    }

    public void UpdgradeSkill()
    {
        level++;
        baseDamage += damageUpgradeAmount;
    }

    public int RequiredXp()
    {
        switch(level)
        {
            case -1:
                return initialUnlockCost;
            default:
                return initialUnlockCost + (baseXPCost * level);
        }
    }

    public void ResetToLevel1()
    {
        if (unlocked == false) return;
        level = 1;

    }
}
