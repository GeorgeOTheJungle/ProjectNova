using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEntity : Entity
{
    [Header("Player Specific Visuals: "), Space(10)]
    [SerializeField] private GameObject commandsVisuals;

    private CombatNavigation combatNavigation;
    private CombatUI combatUI;
    public override void OnAwake()
    {
        combatNavigation = GetComponentInChildren<CombatNavigation>();
        combatUI = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<CombatUI>();
    }

    public override void OnCombatStart(GameState gameState)
    {
        currentSkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 1.0f));
                break;
        }

        UpdateEntityStatsUI();
    }

    public override void OnEntityTurn()
    {
        if (entityStats.defenseBonus > 0.0f)
        {
            PlayAnimation(GUARD_HIT_ANIMATION);
            entityStats.defenseBonus = 0.0f;
        }
        currentSkill = null;
        StartCoroutine(TurnCommandsVisuals(true, 0.0f));
    }

    public override void OnStart()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    public override void PerformAction(Skill skill)
    {
        currentSkill = skill;

        // Use resource
        UseResource();    
        // Do visuals
        PlayAnimation(currentSkill.animationKey);

        // Perform action
        if (currentSkill.baseDamage >= 0) combatNavigation.OnSkillSelected();
        else StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    public override void TargetEntity(int entitySlot) // Set entity to attack and deal damage
    {
        targetEntity = entitySlot;
        PerformAction(preSelectedSkill);
       // CombatManager.Instance.ActivateTargets();
    }
    public override void AttackEntity()
    {
        // Once the player targets the enemy
        // Perform action.
        // Deal damage.
        // Pass to next turn.
        CombatManager.Instance.entityList[targetEntity].OnDamageTaken(CalculateDamageDealt(), currentSkill.damageType);
    }

    protected override void UpdateEntityStatsUI()
    {
        combatUI.UpdateCombatStats();
    }

    public override void CombatUICleanUp()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    private void UseResource()
    {
        switch (currentSkill.resourceType)
        {
            case ResourceType.ammo:
                entityStats.ammo -= currentSkill.resourceAmount;
                if (entityStats.ammo <= 0)
                {
                    entityStats.ammo = 0;
                    // Here call the change of the command from shoot to hit.
                }
                break;
            case ResourceType.energy:
                entityStats.energy -= currentSkill.resourceAmount;
                break;
        }
        combatUI.UpdateCombatStats();
    }

    protected override void UpdateEntityUI(bool active)
    {
        commandsVisuals.SetActive(active);

        if(active) combatNavigation.StartCombatWindows();
    }

    #region Resource Methods

    public bool HasAmmo(int amount)
    {
        return entityStats.ammo >= amount;
    }

    public bool HasEnergy(int energy)
    {
        return entityStats.energy >= energy;
    }
    #endregion

    //#region Corutines
    //private IEnumerator DelayEntrance()
    //{
    //    yield return new WaitForSeconds(0.25f);
    //    PlayAnimation(ENTRANCE_ANIMATION);
    //}

    //private IEnumerator TurnCommandsVisuals(bool active, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    commandsVisuals.SetActive(active);
    //    combatNavigation.StartCombatWindows();
    //    //if (active) EventSystem.current.SetSelectedGameObject(firstCommandSelected);
    //    //EventSystem.current.UpdateModules();
    //}

    //#endregion

}
