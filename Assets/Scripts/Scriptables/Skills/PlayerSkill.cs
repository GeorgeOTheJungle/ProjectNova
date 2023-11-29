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

   // [HideInInspector] public Sprite smallIcon;
    [HideInInspector] public Sprite icon;

    //[Header("Targeting and resources:"), Space(10)]
    [HideInInspector] public TargetingStyle targetingStyle;

    [HideInInspector] public ResourceType resourceType;
    [HideInInspector] public int resourceAmount;
    private float initialDamage;
    private int initialevel;
    private bool wasUnlocked;
    public void Initialize()
    {
        initialDamage = baseDamage;
        //level = 0;
        initialevel = level;
        wasUnlocked = unlocked;
        //unlocked = false;
    }

    public void RestoreToDefault()
    {
        baseDamage = initialDamage;
        level = initialevel;
        unlocked = wasUnlocked;
    }


    public void ResetToFactory()
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
