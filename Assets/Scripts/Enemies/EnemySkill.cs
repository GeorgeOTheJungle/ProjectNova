using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    public int entityID;
    private Transform playerTransform;
    [SerializeField] private bool lookForPlayerCenter = false;
    private Vector3 playerPosition;

    private ScriptedAnimator[] animators;
    private void Awake()
    {
        if (lookForPlayerCenter) playerTransform = GameObject.Find("PlayerCenter").GetComponent<Transform>();
        else playerTransform = GameObject.Find("PlayerEntity").GetComponent<Transform>();

        animators = GetComponentsInChildren<ScriptedAnimator>();
        playerPosition = playerTransform.position;
        playerPosition.y += .01f;
        transform.position = playerPosition;
    }

    public void DoSkill(int id)
    { 
        if (animators == null) return;
        animators[id].PlayAnimation();
    }

    public void DoRandomSkill()
    {
        int i = Random.Range(0, animators.Length);
        animators[i].PlayAnimation();
    }
}
