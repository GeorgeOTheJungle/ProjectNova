using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;
using UnityEngine.EventSystems;

public class CombatPlayer : MonoBehaviour, IDamageable
{
    public static CombatPlayer Instance;
    [SerializeField] private EntityData entityData;

    public Stats playerStats;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject commandsVisuals;
    [SerializeField] private GameObject firstCommandSelected;

    [SerializeField] private Skill currentAction;

    private bool isGuarding;
    private CombatNavigation combatNavigation;
    private CombatUI combatUI;
    private void Awake()
    {
        Instance = this;
        combatNavigation = GetComponentInChildren<CombatNavigation>();
        combatUI = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<CombatUI>();
    }

    private IEnumerator Start()
    {
        playerStats = new Stats();
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;

    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= HandleCombatStart;

    }

    public void HandleCombatStart(GameState gameState)
    {
        isGuarding = false;
        currentAction = null;
        switch (gameState)
        {
            case GameState.combatPreparation:

                playerStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 1.0f));
                break;
        }

    }

    public void OnPlayerTurn()
    {
        Debug.Log("Its player turn");
        if (isGuarding)
        {
            animator.SetTrigger("endGuard");
            isGuarding = false;
        }

        StartCoroutine(TurnCommandsVisuals(true, 0.0f));
    }
    private IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        animator.Play("Entrance");
    }

    private IEnumerator TurnCommandsVisuals(bool active,float delay)
    {
        yield return new WaitForSeconds(delay);
        commandsVisuals.SetActive(active);
        combatNavigation.StartCombatWindows();
        if (active) EventSystem.current.SetSelectedGameObject(firstCommandSelected);
        EventSystem.current.UpdateModules();
    }

    public void PerformAction(Skill action)
    {
        currentAction = action;

        // Use resource
        switch (currentAction.resourceType)
        {
            case ResourceType.ammo:
                // MAKE THE PUNCH ANIMATION IF THE PLAYER DOESNT HAVE ANY AMMO
                playerStats.ammo-= currentAction.resourceAmount;
                if(playerStats.ammo <= 0)
                {
                    playerStats.ammo = 0;
                    // Here call the change of the command from shoot to hit.
                }
                break;
            case ResourceType.energy:
                playerStats.energy -= currentAction.resourceAmount;
                break;
        }
        combatUI.UpdateCombatStats();

        // Do visuals
        animator.Play(currentAction.animationKey);

        // Perform action
        if (currentAction.baseDamage >= 0)
        {
            combatNavigation.OnSkillSelected();
            CombatManager.Instance.OnActionFinished();
        }
        else
        {
            StartCoroutine(TurnCommandsVisuals(false, 0.0f));
            // Player is escaping
        }
    }

    public void Reload()
    {
        playerStats.ammo = entityData.stats.ammo;
        combatUI.UpdateCombatStats();
        // Here update player UI from hit to shoot.
    }

    #region Resource Methods

    public bool HasAmmo(int amount)
    {
        return playerStats.ammo >= amount;
    }

    public bool HasEnergy(int energy)
    {
        return playerStats.energy >= energy;
    }
    #endregion

    #region Damage Logic
    public void ReceiveDamage(int damage,bool isMagic)
    {
        playerStats.health -= CalculateDamageReceived(damage, isMagic);
        if (isGuarding == false) animator.Play("Hit");
        else animator.SetTrigger("guardHit");
        if (playerStats.health < 0)
        {
            playerStats.health = 0;
        }

        combatUI.UpdateCombatStats();
    }

    public void GuardFromDamage()
    {
        isGuarding = true;
    }

    public void Escape()
    {
        CombatManager.Instance.OnPlayerEscape();
    }

    // Returns total damage, calculation should be... calculated using armor as a base.
    /*
     * Armor is from 0 to 100 percent
     */
    public int CalculateDamageReceived(int damageReceived,bool isMagic)
    {
        float defenseRate = isMagic ? playerStats.magicArmor : playerStats.physicalArmor / 100.0f;
        if (isGuarding) defenseRate += (defenseRate * 2.0f);
        float totalDamage;
        totalDamage = damageReceived - (damageReceived * defenseRate);
        totalDamage = Mathf.Abs(totalDamage);
        return Mathf.CeilToInt(totalDamage);
    }

    public int GetDamage(bool isMagic)
    {
        int baseDamage = isMagic ? playerStats.magicDamage : playerStats.physicalDamage;
        int skillDamage = currentAction.baseDamage;

        bool isCrit = Random.Range(0.0f, 1.0f) < 0.05f + currentAction.critChance;
        float totalDamage = (baseDamage + skillDamage) * (isCrit?2.5f:1);
        return Mathf.CeilToInt(totalDamage);
    }
    #endregion
}

/*
 * ANIMATION LIST:
 * - Entrance
 * - Atk_[Number]
 * - Spc Atk_[Number]
 * - Hit
 * - Death
 */
