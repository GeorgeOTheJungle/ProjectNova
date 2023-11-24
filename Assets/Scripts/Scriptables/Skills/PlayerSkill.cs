using Enums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Player Skill",menuName ="Skill/PlayerSkill")]
public class PlayerSkill : Skill
{

    // [Header("Player Skill upgrades:"),Space(10)]
    [HideInInspector] public bool canBeUpgraded = true;
    [HideInInspector] public bool unlocked;
    [HideInInspector] [Min(0)] public int level = 0;
    [HideInInspector] public int initialUnlockCost;
    private int baseXPCost = 200;
    
    [HideInInspector] public float damageUpgradeAmount;

    [HideInInspector] public string skillName; // Use it for UI.
    [HideInInspector] [TextArea] public string skillDescription;

    [HideInInspector] public Sprite smallIcon;
    [HideInInspector] public Sprite largeIcon;

    //[Header("Targeting and resources:"), Space(10)]
    [HideInInspector] public TargetingStyle targetingStyle;

    [HideInInspector] public ResourceType resourceType;
    [HideInInspector] public int resourceAmount;
    private float initialDamage;
    public void Initialize()
    {
        if (unlocked) return;
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
        switch (level)
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
