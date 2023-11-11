using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [SerializeField] private bool isPlayerTurn = true;
    [SerializeField] public List<Entity> enemiesInCombat = new List<Entity>();
    [SerializeField] private float nextTurnDelay = 0.75f;
    private Entity currentEntity;
    public delegate void CombatCleanupEvent();
    public delegate void CombatStartEvent();
    public delegate void CombatEndEvent(CombatResult restult); // value is the result of the combat

    public CombatStartEvent onCombatStart;
    public CombatEndEvent onCombatFinish;
    public CombatCleanupEvent onCombatCleanup;  
    private void Awake()
    {
        Instance = this;
    }

    public void EnterCombat(int index)
    {
        currentEntity = enemiesInCombat[index];
        onCombatStart?.Invoke();
        GameManager.Instance.ChangeGameState(GameState.combatPreparation);
        isPlayerTurn = true;
    }

    public void DealDamageToCurrentEntity(int damage,bool isMagic)
    {
        if (currentEntity == null) return;
        currentEntity.ReceiveDamage(damage,isMagic);
    }

    public void OnActionFinished()
    {
        isPlayerTurn = !isPlayerTurn;
        StartCoroutine(NextTurn());
    }

    public void OnPlayerEscape()
    {
        StartCoroutine(DelayedCleanup());
        // Start stopping everything and call the transition manager.
    }

    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(0.75f);
        onCombatFinish?.Invoke(CombatResult.escape);
        yield return new WaitForSeconds(0.15f);
        onCombatCleanup?.Invoke();
    }

    private IEnumerator NextTurn()
    {
        
        yield return new WaitForSeconds(nextTurnDelay);
        // Tell the enemy to perform a skill.

        if (isPlayerTurn)
        {
            
            // Means the enemy already attacked so it should make the on Round and apply effects.
            OnRoundFinished();
        }
        else
        {
            Debug.Log("Entity turn!");
            currentEntity.PerformAttack();
        }
    }

    public void OnRoundFinished()
    {
        // Apply effects, and reset UI
        StartCoroutine(RoundFinishedAnimation());
    }

    private IEnumerator RoundFinishedAnimation()
    {
        // Tell any effect to do its effect.
        Debug.Log("Round finished! Applying effects");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Player turn!");
        CombatPlayer.Instance.OnPlayerTurn();
    }

    
    /*
     * USE A POOL OF ENEMIES OR DIFERENT PREFABS, ITS NOT GONNA BE POSIBLE RIGHT NOW TO HAVE MULTIPLE ENEMIES AT THE SAME TIME 
     * SO, BETTER HAVE IT LIKE THAT.
     * 
     * - Player enters combat when interacting with an encounter entity [*]
     * - Entity tells the combat manager what character to use. [ ]
     * COMBAT LOOP:
     * - Initialization. [*]
     * - Enemy initialization [*]
     * - UI turns on and player can use it. [*]
     * - Player navigate throught UI [*]
     * - Selects skill [*]
     * - On selects, UI turns off. [*]
     * - Catherine does the attack. [*]
     * - Deal damage to enemy and update UIs. [*]
     * - Apply effect if any.
     * ----------------------------------------------------------------- ENEMY TURN
     * - Enemy selects attack. [*]
     * - Does animation. [*]
     * - Deals damage and update UIs. [*]
     * - Apply effect if any
     * ----------------------------------------------------------------- EFFECTS TURN
     * - Calls an event of "On Combat round ended" and apply all effects (poison or regen). [ ]
     * 
     * 
     * -----------------------------------------------------------------
     * TODO:
     * - Move Action text features to combat navigation
     */
}
