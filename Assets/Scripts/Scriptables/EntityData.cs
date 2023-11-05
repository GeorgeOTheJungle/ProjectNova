using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

[CreateAssetMenu(fileName = "New entity data", menuName = "Entity Data")]
public class EntityData : ScriptableObject
{
    [Header("Entity Data:"), Space(10)]
    public string entityName;
    public Stats stats;

    [Header("Avaliable Skills: "), Space(10)]
    public string soon = "Soon";
    [Header("References: "), Space(10)]
    public Animator entityAnimator;
}