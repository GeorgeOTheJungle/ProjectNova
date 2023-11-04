using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
    }

    public void SetIdleAnimation(float lastY)
    {
        m_animator.SetFloat("lastY", lastY) ;
    }

    public void SetRunAnimation(bool isMoving,float moveY)
    {
        m_animator.SetBool("isMoving", isMoving);
        m_animator.SetFloat("moveY", moveY);
    }

    public void InteractAnimation()
    {
        m_animator.SetTrigger("interact");
    }
}
