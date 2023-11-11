using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
[CreateAssetMenu(fileName ="New Skill",menuName ="Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill data"), Space(10)]
    public bool unlocked;
    public string skillName; // Use it for UI.
    public float baseDamage;
    public float critChance;
    public ResourceType resourceType;
    public int resourceAmount;

    public string animationKey = ""; // Use this to call the animation you need on the entity

    public Sprite smallIcon;
    public Sprite largeIcon;
}
