using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using UnityEditor.Animations;

[CreateAssetMenu(fileName = "New entity data", menuName = "Entity Data")]
public class EntityData : ScriptableObject
{
    [Header("Entity Data:"), Space(10)]
    public string entityName;
    
    public int entityID;
    public Stats stats;

    [Header("Avaliable Skills"), Space(10)]
    public List<Skill> avaliableSkills;

    [Header("References: "), Space(10)]
    public RuntimeAnimatorController entityAnimator;

}