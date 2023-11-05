using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class EncounterHandler : MonoBehaviour, IInteractable
{
    [Header("Encounter Handler"), Space(10)]
    [SerializeField] private List<EntityData> enemyDatas = new List<EntityData>();
    public void OnInteraction()
    {
        // Tell the combat manager to enter combat mode and give the list to them. Also register to a combat event for when the combat
        // ended.
        CombatManager.Instance.EnterCombat(enemyDatas);
    }

    public void OnPlayerEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerExit()
    {
        throw new System.NotImplementedException();
    }

    
    private void HandleCombatResults(CombatResult result)
    {

    }
}
