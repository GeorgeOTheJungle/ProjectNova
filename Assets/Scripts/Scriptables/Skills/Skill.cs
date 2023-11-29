using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System;

[Serializable]
[CreateAssetMenu(fileName ="New Skill",menuName ="Skill")]
public abstract class Skill : ScriptableObject
{
    [Header("Skill data"), Space(10)]

    public float baseDamage; // Total damage the skill will do,
    public DamageType damageType;
    public StatusEffectType statusEffectType;
    [Space(5)]
    public string animationKey = ""; // Use this to call the animation you need on the entity
}
