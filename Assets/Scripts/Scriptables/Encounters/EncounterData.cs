using Structs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter Data")]

public class EncounterData : ScriptableObject
{
    [Header("Encounter Handler"), Space(10)]
    [Tooltip("Keep this number Unique")]
    public int arenaId;
    public EntityData[] encounter = new EntityData[1];

    public Sprite chibiPreview;
    public RuntimeAnimatorController chibiAnimator;

}
