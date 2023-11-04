using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [SerializeField] private List<EnemyData> enemiesInCombat = new List<EnemyData>();

    public delegate void CombatStartEvent();
    public delegate void CombatEndEvent(CombatResult restult); // value is the result of the combat

    public CombatStartEvent onCombatStart;
    public CombatEndEvent onCombatFinish;
    private void Awake()
    {
        Instance = this;
    }

    public void EnterCombat(List<EnemyData> enemyList)
    {
        enemiesInCombat = enemyList;
        onCombatStart?.Invoke();
        // todo OnCombatStart
        // Transition
        // Camera change.
        // Set enemies
        // Set player
        // Start game
    }
}
