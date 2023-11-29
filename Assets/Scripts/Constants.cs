using System.Collections.Generic;
using UnityEngine;


public static class Constants
{
    // ------------------------------------------------
    public const float FADE_TIME = 0.5f;
    public const float TRANSITION_TIME = 1.0f;
    public const float WAIT_TIME = 0.5f;
    public const string FADE_TO_BLACK = "Fade_To_Black";
    // ------------------------------------------------


    // -------------- EFFECTS -------------
    // ------------------------------------------------
    public const int FIRE_EFFECT_TURNS = 3;
    public const float FIRE_EFFECT_CHANCE = 0.25f;
    public const float FIRE_EFFECT_DAMAGE = 0.05f;
    // ------------------------------------------------
    public const int ICE_EFFECT_TURNS = 2;
    public const float ICE_EFFECT_CHANCE = 0.25f;
    public const float ICE_RESISTANCE_REDUCTION = 0.25f;
    // ------------------------------------------------
    public const int WEAK_EFFECT_TURNS = 3;
    public const float WEAK_EFFECT_CHANCE = 0.5f;
    public const float WEAK_DAMAGE_REDUCTION = 0.5f;

    // ------------------------------------------------ SOUNDS
    //public const int HIT_SOUND = 0;
    public const int DEATH_SOUND = 1;

    // ------------------------------------------------ ANIMATIONS
    public const string ENTRANCE_ANIMATION = "Entrance";
    public const string HIT_ANIMATION = "Hit";
    public const string GUARD_HIT_ANIMATION = "GuardHit";
    public const string GUARD_END_ANIMATION = "GuardEnd"; // TODO MAKE TRANSITIONS FROM GUARD TO IDLE
    public const string IDLE_OUT = "IdleOut";
    public const string DEATH_ANIMATION = "Death";
}

