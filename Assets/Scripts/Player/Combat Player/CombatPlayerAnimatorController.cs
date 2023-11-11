using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayerAnimatorController : MonoBehaviour
{
    // Manages animations instead of using the animator window.

    [SerializeField] private Animator animator;

    private string currentAnimation = "";
    public void PlayAnimation(string newAnimation)
    {
        if(currentAnimation == newAnimation) return;

        animator.Play(newAnimation);

        currentAnimation = newAnimation;
    }
}
