using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractComponent : MonoBehaviour
{
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float detectionRadius = 3.0f;
    [SerializeField] private Animator interactQueueAnimator;

    private int frames = 0;
    private IInteractable targetInteractable;
    private PlayerAnimatorController playerAnimatorController;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.interactTrigger += HandleInteraction;
    }

    private void OnEnable()
    {
        if (InputComponent.Instance == null) return;
        InputComponent.Instance.interactTrigger += HandleInteraction;
    }

    private void OnDisable()
    {
        InputComponent.Instance.interactTrigger -= HandleInteraction;
    }

    private void Update()
    {
        frames++;
        if(frames % 10 == 0) InteractableCheck();
    }

    private void HandleInteraction()
    {
        if (GameManager.Instance._gameState != Enums.GameState.exploration) return;
        if(targetInteractable == null) return;
        targetInteractable.OnInteraction();
        playerAnimatorController.InteractAnimation();
    }

    private void InteractableCheck()
    {
        int maxColliders = 1;
        Collider[] hitColliders = new Collider[maxColliders];
        int numberColliders = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, hitColliders,interactableMask);
        if (numberColliders == 0)
        {
            targetInteractable = null;
            interactQueueAnimator.SetBool("isActive", false);
            return;
        }
        for (int i = 0;i < numberColliders;i++)
        {

            if (hitColliders[i].TryGetComponent(out IInteractable interactable))
            {
                targetInteractable = interactable;
                interactQueueAnimator.SetBool("isActive", true);
            }
        }     
    }
}
