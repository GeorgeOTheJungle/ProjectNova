using System.Collections;
using UnityEngine;
using Enums;
using Structs;
using System;
using UnityEditor;

public class EncounterHandler : MonoBehaviour, IInteractable
{

    private int combatID = 0;
    [Header("Encounter Handler"), Space(10)]
    public EncounterInfo encounterInfo;

    private Animator visualAnimator;
    private SpriteRenderer visualRenderer;

    private void Awake()
    {
        if (visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualRenderer == null) visualRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private IEnumerator Start()
    {
        combatID = Guid.NewGuid().GetHashCode();
        //UpdateVisuals();
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

    [ContextMenu("Update Visuals")]
    public void UpdateVisuals()
    {
       // if (encounterData == null) return;
        if (encounterInfo.chibiAnimator == null) return;
        visualAnimator.runtimeAnimatorController = encounterInfo.chibiAnimator;

        if (encounterInfo.chibiPreview == null) return;
        visualRenderer.sprite = encounterInfo.chibiPreview;
    }

    public void SetEncounter()
    {
        if(visualAnimator == null) visualAnimator = GetComponentInChildren<Animator>();
        if (visualRenderer == null) visualRenderer = GetComponentInChildren<SpriteRenderer>();
        //encounterData = encounter;
        UpdateVisuals();
    }

    public void OnInteraction()
    {
        // Tell the combat manager to enter combat mode and give the list to them. Also register to a combat event for when the combat
        // ended.
        CombatArenaManager.Instance.SetArena(encounterInfo.arenaId);
        CombatManager.Instance.EnterCombat(encounterInfo.encounters, combatID);
       // SoundManager.Instance.ChangeToCombatMusic();
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

#if UNITY_EDITOR
[CustomEditor(typeof(EncounterHandler))]
public class EncounterHandlerEditor : Editor
{
    protected EncounterHandler m_encounter;
    private void OnEnable()
    {
        m_encounter = (EncounterHandler)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(5);
        if(GUILayout.Button("Set Encounters"))
        {
            m_encounter.SetEncounter();
        }
    }
}
#endif


