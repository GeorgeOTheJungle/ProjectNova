using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;

public class CombatPlayer : MonoBehaviour
{
    [SerializeField] private EntityData entityData;

    public Stats playerStats;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;

    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= OnCombatStart;

    }

    public void OnCombatStart(GameState gameState)
    {
        if (gameState != GameState.combatReady) return;
        // Entrance animation call goes here.
        // Initialization
        playerStats = new Stats();
        playerStats = entityData.stats;
    }
}
