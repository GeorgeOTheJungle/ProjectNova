using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Structs;
using System;

public class EncounterHandler : MonoBehaviour, IInteractable
{
    [Header("Encounter Handler"), Space(10)]
    public int combatID = 0;
    [SerializeField] private EncounterData encounterData;

    private IEnumerator Start()
    {
        combatID = Guid.NewGuid().GetHashCode();
        yield return new WaitForEndOfFrame();
        CombatManager.Instance.onCombatFinish += HandleCombatResults;
    }

    private void OnEnable()
    {
        if(CombatManager.Instance) CombatManager.Instance.onCombatFinish += HandleCombatResults;
    }

    private void OnDisable()
    {
        if(CombatManager.Instance) CombatManager.Instance.onCombatFinish -= HandleCombatResults;
    }


    public void OnInteraction()
    {
        // Tell the combat manager to enter combat mode and give the list to them. Also register to a combat event for when the combat
        // ended.
        CombatArenaManager.Instance.SetArena(encounterData.arenaId);
        CombatManager.Instance.EnterCombat(encounterData.encounter, combatID);
    }

    public void OnPlayerEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerExit()
    {
        throw new System.NotImplementedException();
    }

    
    private void HandleCombatResults(CombatResult result,int id)
    {
        if (id != combatID) return;
        if (result == CombatResult.victory) gameObject.SetActive(false);
    }


}


