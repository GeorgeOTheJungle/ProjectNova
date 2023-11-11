using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventCall : MonoBehaviour
{
    protected Animator animator;
    protected const string IDLE_ANIMATION = "Idle";
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public abstract void DealPhysicalDamage();

    public abstract void DealMagicDamage();

    public abstract void OnActionFinished();

    public abstract void OnAnimationFinish();
}
