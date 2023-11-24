using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{

    protected bool effectActive = false;

    [SerializeField] protected ParticleSystem onStartParticles;
    [SerializeField] protected ParticleSystem onEffectParticles;

    protected Entity currentEntity;
    protected int turnsLeft = 0;

    public bool EffectActive() => effectActive;

    public virtual void OnEffectStart(Entity entity)
    {
        effectActive = true;
        currentEntity = entity;
        turnsLeft = Constants.FIRE_EFFECT_TURNS;
        if(onStartParticles) onStartParticles.Play();
    }

    protected void OnEffectUse()
    {
        turnsLeft--;
        if(onEffectParticles) onEffectParticles.Play();
        if(turnsLeft <= 0)
        {
            turnsLeft = 0;
            effectActive = false;
            onStartParticles.Stop();
        }
    }

    public virtual void RemoveEffect()
    {
        turnsLeft = 0;
        effectActive = false;
    }

    public abstract void ApplyEffect();
}

// ---- STATUS EFFECT THINGS
/*
 * On Fire: Deal damage equivalent to a % of health/.
 * On Ice: Increase damage received.
 * Weak: Deal less damage.
 * 
 * They all take some turns to dispell.
 */
