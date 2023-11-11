using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Linq;
using Structs;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [Header("Combat manager: "), Space(10)]
    [SerializeField] private Entity playerEntity;

    [Header("Entity Slots: "), Space(10)]
    [SerializeField] private Entity singleEntity;

    public List<Entity> entityList = new List<Entity>(); // This list is for targeting
    // TODO IF YOU ARE GOING TO MAKE A SPEED STAT THEN YOU NEED A DIFFERENT LIST THAT CONTAINS THE ACTUAL ORDER

    [Header("Combat settings: "), Space(10)]
    [SerializeField] private int entityTurn = 0;
    [SerializeField] private float nextTurnDelay = 0.75f;

    private Entity currentEntity;
    public delegate void CombatCleanupEvent();
    public delegate void CombatStartEvent();
    public delegate void CombatEndEvent(CombatResult restult); // value is the result of the combat

    public CombatStartEvent onCombatStart; // This calls the transition manager!
    public CombatEndEvent onCombatFinish;
    public CombatCleanupEvent onCombatCleanup;

    private int entityListLenght;
    private int totalEnemies;
    private void Awake()
    {
        Instance = this;
    }

    // TODO maybe sort them by speed stat?
    public void EnterCombat(List<EntityData> listOfEnemies)
    {
        entityList.Clear();
        entityList.Add(playerEntity);

        totalEnemies = listOfEnemies.Count;

        //Check total enemies to assign an array (1, 2 or 3 enemies in total is supported)
        switch (totalEnemies)
        {
            case 1:
                singleEntity.SetEntityData(listOfEnemies[0]);
                entityList.Add(singleEntity);
                
                break;
            case 2:
                break;
            case 3:
                break;
        }

        
        
        // Set Enemy list
        // Tell each enemy to set itself with the entity Data

        //foreach(EntityData entity in listOfEnemies)
        //{
        //    entityList.Add(entity);
        //}
        //currentEntity = enemiesInCombat[index];
        entityListLenght = entityList.Count - 1;
        onCombatStart?.Invoke();
        GameManager.Instance.ChangeGameState(GameState.combatPreparation);
        //isPlayerTurn = true;
    }

    // TODO Remove this function from here! Player should be able to target enemies by itself, same for enemies.
    public void DealDamageToTargetEntity(int targetEntity,int damage,DamageType damageType)
    {
        entityList[targetEntity].OnDamageTaken(damage, damageType);
    }

    public void DealDamageToCurrentEntity(int damage,DamageType damageType)
    {
        // THIS IS OBSOLETE
        if (currentEntity == null) return;
      //  currentEntity.OnDamageTaken(damage,isMagic);
    }

    public void ActivateTargets()
    {
        bool selectFirst = true;
        for(int i = 0;i < entityListLenght+1; i++)
        {
            if (entityList[i].entityData.entityID != -1)
            {
                entityList[i].SetTarget(true, selectFirst);
                selectFirst = false;
            }

        }

    }

    public void OnTurnFinished()
    {
        
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
        entityTurn++;
        if (entityTurn > entityListLenght) entityTurn = -1;
        yield return new WaitForSeconds(nextTurnDelay);
        // Tell the enemy to perform a skill.

        if(entityTurn == -1)
        {
            StartCoroutine(OnRoundFinished());
        }
        else
        {
            entityList[entityTurn].OnEntityTurn();
        }
    }

    private IEnumerator OnRoundFinished()
    {
        // Tell any effect to do its effect.
        Debug.Log("Round finished! Applying effects");
        yield return new WaitForSeconds(0.5f);
        entityTurn = 0;
        Debug.Log("Player turn!");
        entityList[entityTurn].OnEntityTurn();
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
