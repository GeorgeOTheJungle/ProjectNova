using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New effect",menuName ="StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [Header("Status effect:")]
    public float effectValue;
    public int effectTime;
}
