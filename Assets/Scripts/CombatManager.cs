using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [Header("Combat manager: "), Space(10)]
    [SerializeField] private int currentCombatID;
    [SerializeField] private Entity playerEntity;

    [Header("Entity Slots: "), Space(10)]

    [SerializeField] private Entity[] entitiesPlacement;

    public List<Entity> entityList = new List<Entity>(); // This list is for targeting
    public List<Entity> combatOrder = new List<Entity>();
    // TODO IF YOU ARE GOING TO MAKE A SPEED STAT THEN YOU NEED A DIFFERENT LIST THAT CONTAINS THE ACTUAL ORDER

    [Header("Combat settings: "), Space(10)]
    [SerializeField] private int combatTurn = 0;
    [SerializeField] private float nextTurnDelay = 0.75f;
    [SerializeField] private int xpStored = 0;
    [SerializeField] private Animator targetAllAnimator;
    private Entity currentEntity;
    public delegate void CombatCleanupEvent();
    public delegate void CombatStartEvent();
    public delegate void CombatEndEvent(CombatResult restult,int id); // value is the result of the combat

    public CombatStartEvent onCombatStart; // This calls the transition manager!
    public CombatEndEvent onCombatFinish;
    public CombatCleanupEvent onCombatCleanup;

    private int entityListLenght;
    private int combatListLenght;
    private int totalEnemies;
    private void Awake()
    {
        Instance = this;
    }

    public void EnterCombat(EntityData[] listOfEnemies,int combatID)
    {
        // Clear previous list.
        entityList.Clear();
        combatOrder.Clear();

        // Set combat ID
        currentCombatID = combatID;

        // Add player to the combat order
        combatOrder.Add(playerEntity);

        // Calculate enemies lenght
        totalEnemies = listOfEnemies.Length;

        // Set and add enemies to combat order
        int initialEnemyPlacement = IsEven(totalEnemies)?1:0;

        for (int i = 0; i < totalEnemies; i++)
        {
            entitiesPlacement[initialEnemyPlacement].SetEntityData(listOfEnemies[i], i);
            xpStored += listOfEnemies[i].stats.xpYield;
            entityList.Add(entitiesPlacement[initialEnemyPlacement]);
            combatOrder.Add(entitiesPlacement[initialEnemyPlacement]);
            initialEnemyPlacement++;
        }
        entityListLenght = entityList.Count - 1;
        combatListLenght = combatOrder.Count - 1;
        onCombatStart?.Invoke();
        GameManager.Instance.ChangeGameState(GameState.combatPreparation);

        combatOrder.OrderBy(w => w.entityData.stats.speed).ToList();
    }

    public void OnTurnFinished()
    {      
        StartNextEntityTurn();
    }

    public void OnPlayerEscape()
    {
        Debug.LogWarning("TODO ESCAPE CLEANUP");
        StartCoroutine(DelayedCleanup());
    }

    public void OnEnemyDefeated()
    {
        totalEnemies--;
    }

    private void StartNextEntityTurn()
    {
        if (totalEnemies == 0)
        {
            Invoke(nameof(VictoryCall), 1.0f);
            return;
        }

        combatTurn++;
        if (combatTurn > combatListLenght) combatTurn = -1;
        if (combatTurn == -1)
        {
            StartCoroutine(OnRoundFinished());
        }
        else
        {
            combatOrder[combatTurn].OnEntityTurn();
        }
    }

    private void VictoryCall()
    {
        // Add here victory music
        GameManager.Instance.ChangeGameState(GameState.combatEnded);
        onCombatFinish?.Invoke(CombatResult.victory, currentCombatID);
    }

    public int GetXPStored() => xpStored;

    public void StartCleanup()
    {
        onCombatCleanup?.Invoke();
        combatOrder.Clear();
        entityList.Clear();
    }

    #region Targeting

    /*
     * Player chooses a skill, if it requires a target, open target system
     * [*] Check if player requires a global target or a single target.
     * [*] Pre select the one in the middle.
     * [?] Open health ui of the enemy.
     * [*] Navigate using up and down buttons. It shouldnt be a button. Should be a int 
     * [*] On player target slection, set target using what i already have for targeting.
     * [*] Do animation.
     * 
     * ------------------
     * [*] The player should be able to return to skill/attack selection if K is pressed.
    */

    private int currentTarget = 0;
    private bool targeting;
    private void Update()
    {
        if (targeting == false) return;
        if (Input.GetKeyDown(KeyCode.W))
        {
            HandleTargetNavigation(1);
        } else if (Input.GetKeyDown(KeyCode.S))
        {
            HandleTargetNavigation(-1);
        } else if (Input.GetKeyDown(KeyCode.J))
        {      
            ConfirmSelection(currentTarget);
        } else if (Input.GetKeyDown(KeyCode.K))
        {
            CombatNavigation.Instance.OpenWindow(CombatNavigation.Instance.lastWindow);
            CombatNavigation.Instance.EnableMenuNavigation(true);
            CloseTargetWindow();
        }
    }
    public void OpenTargetWindow(TargetingStyle targetingStyle)
    {
        switch (targetingStyle)
        {
            case TargetingStyle.single:
                bool selectFirst = true;
                for (int i = 0; i < entityListLenght + 1; i++)
                {
                    if (entityList[i].EntityDead() == false)
                    {
                        entityList[i].OpenTargetWindow(true, selectFirst);
                        if (selectFirst) currentTarget = i;
                        selectFirst = false;
                    }
                }
 
                break;
            case TargetingStyle.multiple:
                currentTarget = -1;
                targetAllAnimator.SetBool("isActive", true);
                break;
        }
        Invoke(nameof(DelayedTargetingUI), 0.1f);

    }
    private void DelayedTargetingUI()
    {
        targeting = true;
    }
    public void ConfirmSelection(int target)
    {
        if (targeting == false) return;

        CloseTargetWindow();
        targetAllAnimator.SetBool("isActive", false);
        playerEntity.TargetEntity(target);

        targeting = false;
    }
    private void HandleTargetNavigation(int navigationIndex)
    {
        if(targeting == false) return;
        entityList[currentTarget].OpenTargetWindow(false, false);
        currentTarget += navigationIndex;

        if (currentTarget > entityListLenght) currentTarget = 0;
        else if (currentTarget < 0) currentTarget = entityListLenght ;

        if (entityList[currentTarget].EntityDead())
        {
            HandleTargetNavigation(navigationIndex);
            return;
        }
        entityList[currentTarget].OpenTargetWindow(true, true);
    }
    private void CloseTargetWindow()
    {
        targeting = false;
        for (int i = 0; i < entityListLenght + 1; i++)
        {
            if (entityList[i].entityData.entityID != -1)
            {
                entityList[i].CloseTargetWindow();
            }
        }
        targetAllAnimator.SetBool("isActive", false);
    }

    public void AttackAllEntities(int damage, DamageType damageType)
    {
        foreach(var entity in entityList)
        {
            entity.OnDamageTaken(damage,damageType);
        }
    }

    #endregion

    #region Corutines
    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(0.75f);
        onCombatFinish?.Invoke(CombatResult.escape, currentCombatID);
        yield return new WaitForSeconds(0.15f);
        onCombatCleanup?.Invoke();
    } 

    private IEnumerator OnRoundFinished()
    {
        // Tell any effect to do its effect.
        Debug.Log("Round finished! Applying effects");
        yield return new WaitForSeconds(0.5f);
        combatTurn = 0;
        combatOrder[combatTurn].OnEntityTurn();
    }

    #endregion

    #region Get Methods

    public Transform GetEntityTransform(int entityId)
    {
        return entityList[entityId].transform;
    }

    public Entity GetPlayerEntity()
    {
        return playerEntity;
    }

    private bool IsEven(int amount) => ((amount & 1) == 0);
    #endregion
}
