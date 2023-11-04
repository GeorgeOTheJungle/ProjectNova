using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;

[CreateAssetMenu(fileName ="New enemy data",menuName ="Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Data:"), Space(10)]
    public string enemyName;
    public Stats stats;

    [Header("Avaliable Skills: "), Space(10)]
    public string soon = "Soon";
    [Header("References: "),Space(10)]
    public Animator enemyAnimator;
}

