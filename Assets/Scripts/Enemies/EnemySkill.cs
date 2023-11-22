using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private Vector3 playerPosition;

    [SerializeField]
    private ScriptedAnimator[] animators;
    private void Awake()
    {
        playerTransform = GameObject.Find("PlayerEntity").GetComponent<Transform>();
        playerPosition = playerTransform.position;
        playerPosition.y += .01f;
    }

    public void DoAttack(int id)
    {
        transform.position = playerPosition;
        animators[id].PlayAnimation();
    }
}
