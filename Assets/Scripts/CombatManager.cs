using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] private List<EntityData> enemiesInCombat = new List<EntityData>();

    public delegate void CombatStartEvent();
    public delegate void CombatEndEvent(CombatResult restult); // value is the result of the combat

    public CombatStartEvent onCombatStart;
    public CombatEndEvent onCombatFinish;
    private void Awake()
    {
        Instance = this;
    }

    public void EnterCombat(List<EntityData> enemyList)
    {
        enemiesInCombat = enemyList;
        onCombatStart?.Invoke();
        GameManager.Instance.ChangeGameState(GameState.combatPreparation);
        // todo OnCombatStart
        // Transition
        // Camera change.
        // Set enemies
        // Set player
        // Start game
    }
}
