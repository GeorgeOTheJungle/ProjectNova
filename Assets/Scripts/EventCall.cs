using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventCall : MonoBehaviour
{
    protected Animator animator;
    protected Entity m_entity;
    protected const string IDLE_ANIMATION = "Idle";
    protected virtual void Awake()
    {
        m_entity = GetComponentInParent<Entity>();
        animator = GetComponent<Animator>();
    }
    public abstract void DealDamageCall();

    public abstract void OnActionFinished();

    public abstract void OnAnimationFinish();

    public virtual void OnMeleeReady()
    {
        m_entity.MoveEntityToTarget();
    }

    public virtual void OnMeleeReturn()
    {
        m_entity.ReturnToOriginalPosition();
    }
}
