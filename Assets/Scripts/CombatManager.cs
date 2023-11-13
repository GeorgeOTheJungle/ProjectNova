using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System.Linq;
using Structs;
using UnityEditor.Tilemaps;
using System.Runtime.InteropServices.WindowsRuntime;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [Header("Combat manager: "), Space(10)]
    [SerializeField] private Entity playerEntity;

    [Header("Entity Slots: "), Space(10)]
    [SerializeField] private Entity singleEntity;
    [Space(5)]

    [SerializeField] private Entity[] doubleEntity;
    [Space(5)]

    [SerializeField] private List<Entity> tripleEntity;
    public List<Entity> entityList = new List<Entity>(); // This list is for targeting
    // TODO IF YOU ARE GOING TO MAKE A SPEED STAT THEN YOU NEED A DIFFERENT LIST THAT CONTAINS THE ACTUAL ORDER

    [Header("Combat settings: "), Space(10)]
    [SerializeField] private int entityTurn = 0;
    [SerializeField] private float nextTurnDelay = 0.75f;
    [SerializeField] private int xpStored = 0;
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
    public void EnterCombat(EntityData[] listOfEnemies)
    {
        entityList.Clear();
        entityList.Add(playerEntity);

        totalEnemies = listOfEnemies.Length;

        //Check total enemies to assign an array (1, 2 or 3 enemies in total is supported)
        switch (totalEnemies)
        {
            case 1:
  
                entityList.Add(singleEntity);
                singleEntity.SetEntityData(listOfEnemies[0], 1);
                break;
            case 2:
                doubleEntity[0].gameObject.SetActive(true);
                doubleEntity[1].gameObject.SetActive(true);

                doubleEntity[0].SetEntityData(listOfEnemies[0],1);
                doubleEntity[1].SetEntityData(listOfEnemies[1],2);

                xpStored += doubleEntity[0].entityData.stats.xpYield;
                xpStored += doubleEntity[1].entityData.stats.xpYield;

                entityList.Add(doubleEntity[0]);
                entityList.Add(doubleEntity[1]);
                break;
            case 3:
                break;
        }
  

        entityListLenght = entityList.Count - 1;
        onCombatStart?.Invoke();
        GameManager.Instance.ChangeGameState(GameState.combatPreparation);
        //isPlayerTurn = true;
    }

    public void ActivateTargets()
    {
        bool selectFirst = true;
        for(int i = 0;i < entityListLenght+1; i++)
        {
            if (entityList[i].entityData.entityID != -1)
            {
                if (entityList[i].EntityDead() == false)
                {
                    entityList[i].OpenTargetWindow(true, selectFirst);
                    selectFirst = false;
                }
            }
        }

    }

    public void OnTurnFinished()
    {      
        StartNextEntityTurn();
    }

    public void OnPlayerEscape()
    {
        Debug.LogWarning("TODO ESCAPE CLEANUP");
       // StartCoroutine(DelayedCleanup());
    }

    public void SetTarget(int target)
    {
        target++;
        playerEntity.TargetEntity(target);
        for (int i = 0; i < entityListLenght + 1; i++)
        {
            if (entityList[i].entityData.entityID != -1)
            {
                entityList[i].CloseTargetWindow();
                //entityList[i].OpenTargetWindow(true, selectFirst);
               //selectFirst = false;
            }

        }
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

        entityTurn++;
        if (entityTurn > entityListLenght) entityTurn = -1;
        if (entityTurn == -1)
        {
            StartCoroutine(OnRoundFinished());
        }
        else
        {
            entityList[entityTurn].OnEntityTurn();
        }
    }

    private void VictoryCall()
    {
        // Add here victory music
        GameManager.Instance.ChangeGameState(GameState.combatEnded);
        onCombatFinish?.Invoke(CombatResult.victory);
    }

    public int GetXPStored() => xpStored;

    public void StartCleanup() => onCombatCleanup?.Invoke();

    #region Corutines
    private IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(0.75f);
        onCombatFinish?.Invoke(CombatResult.escape);
        yield return new WaitForSeconds(0.15f);
        onCombatCleanup?.Invoke();
    } 

    private IEnumerator OnRoundFinished()
    {
        // Tell any effect to do its effect.
        Debug.Log("Round finished! Applying effects");
        yield return new WaitForSeconds(0.5f);
        entityTurn = 0;
        entityList[entityTurn].OnEntityTurn();
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
    #endregion
}
