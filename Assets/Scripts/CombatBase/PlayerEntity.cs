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
        Debug.Log(gameState);
        currentSkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                Debug.Log("Setting up Player");
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                Debug.Log("Player Ready for combat!");
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 1.0f));
                break;
        }
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
        Debug.Log("Starting!");
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    public override void PerformAction(Skill skill)
    {
        currentSkill = skill;

        // Use resource
        UseResource();    
        // Do visuals
        Debug.Log("Performing action: " + currentSkill.skillName);
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
        Debug.Log("Dealing Damage");
        CombatManager.Instance.entityList[targetEntity].OnDamageTaken(CalculateDamageDealt(), currentSkill.damageType);
    }

    protected override void UpdateEntityUI()
    {
        combatUI.UpdateCombatStats();
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

    #region Corutines
    private IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        PlayAnimation(ENTRANCE_ANIMATION);
    }

    private IEnumerator TurnCommandsVisuals(bool active, float delay)
    {
        yield return new WaitForSeconds(delay);
        commandsVisuals.SetActive(active);
        combatNavigation.StartCombatWindows();
        //if (active) EventSystem.current.SetSelectedGameObject(firstCommandSelected);
        //EventSystem.current.UpdateModules();
    }

    #endregion

}
