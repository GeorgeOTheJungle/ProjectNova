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

    [SerializeField] private CombatPlayerAnimatorController animator;
    [SerializeField] private GameObject commandsVisuals;
    [SerializeField] private GameObject firstCommandSelected;

    private Skill currentAction; // READ ONLY

    [Header("Particle Visuals:"),Space(10)]
    [SerializeField] private ParticleSystem buffParticles;
    private bool isGuarding;
    private CombatNavigation combatNavigation;
    private CombatUI combatUI;

    private const string ENTRANCE_ANIMATION = "Entrance";
    private const string HIT_ANIMATION = "Hit";
    private const string END_GUARD_ANIMATION = "endGuard";
    private const string GUARD_HIT_ANIMATION = "GuardHit";

    private void Awake()
    {
        Instance = this;
        combatNavigation = GetComponentInChildren<CombatNavigation>();
        animator = GetComponent<CombatPlayerAnimatorController>();
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
        if (isGuarding)
        {
            animator.PlayAnimation(END_GUARD_ANIMATION);
            isGuarding = false;
        }
        currentAction = null;
        StartCoroutine(TurnCommandsVisuals(true, 0.0f));
    }

    private IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        animator.PlayAnimation(ENTRANCE_ANIMATION);
       // animator.Play("Entrance");
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
        Debug.Log("Performing action: " + currentAction.skillName);
        animator.PlayAnimation(currentAction.animationKey);

        // Perform action
        if (currentAction.baseDamage >= 0)
        {
            combatNavigation.OnSkillSelected();
        }
        else
        {
            StartCoroutine(TurnCommandsVisuals(false, 0.0f));
            // Player is escaping
        }
    }

    #region Non Aggresive Skills
    public void Heal()
    {
        playerStats.health += CalculateHealing(Mathf.CeilToInt(currentAction.baseDamage));
        if (playerStats.health > entityData.stats.health) playerStats.health = entityData.stats.health;
        combatUI.UpdateCombatStats();
    }

    public void Buff()
    {
        buffParticles.Play();
        playerStats.buffBonus = currentAction.baseDamage;
    }

    #endregion
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
        if (isGuarding == false) animator.PlayAnimation(HIT_ANIMATION);
        else animator.PlayAnimation(GUARD_HIT_ANIMATION);
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
        int baseDamage = isMagic ? playerStats.magicDamage : playerStats.physicalDamage ;
        float skillDamage = currentAction.baseDamage;

        bool isCrit = Random.Range(0.0f, 1.0f) < 0.05f + currentAction.critChance;
        float buffDamage = baseDamage * playerStats.buffBonus;
        float totalDamage = ((baseDamage + skillDamage) + buffDamage) * (isCrit?2.5f:1);
        Debug.Log("Damage sent to entity is: " + totalDamage);
        return Mathf.CeilToInt(totalDamage);
    }
    #endregion

    private int CalculateHealing(int baseHealing)
    {
        float healAmount = baseHealing + (playerStats.magicDamage * 0.2f);
        return Mathf.CeilToInt(healAmount);
    }

}
