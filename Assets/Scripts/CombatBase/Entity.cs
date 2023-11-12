using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using TMPro;
using Enums;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

public abstract class Entity : MonoBehaviour
{
    [Header("ENTITY: "), Space(10)]
    public EntityData entityData;
    public Stats entityStats;
    [SerializeField] protected bool isInvencible = false;
    //[SerializeField] protected List<Effect> effectLists;
    [SerializeField] protected GameObject targetUI;
    [SerializeField] private Button targetButton;
    [Header("Particles Visuals: "), Space(10)]
    [SerializeField] protected ParticleSystem buffParticles;
    private string currentAnimation;

    protected const string ENTRANCE_ANIMATION = "Entrance";
    protected const string HIT_ANIMATION = "Hit";
    protected const string GUARD_HIT_ANIMATION = "GuardHit";
    //protected const string END_GUARD_ANIMATION = "endGuard"; // <-- Not global
    protected const string IDLE_OUT = "IdleOut";

    protected List<Skill> skills;

    protected Skill currentSkill;
    protected Animator animator;

    protected int targetEntity;
    protected Skill preSelectedSkill;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        OnAwake();
    }

    private IEnumerator Start()
    {
        //entityStats = new Stats();
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;
        CombatManager.Instance.onCombatCleanup += HandleCombatCleanup;
        OnStart();
    }


    private void OnEnable()
    {
        if(GameManager.Instance) GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;
        if(CombatManager.Instance) CombatManager.Instance.onCombatCleanup += HandleCombatCleanup;
    }

    private void OnDisable()
    {
        if (GameManager.Instance) GameManager.Instance.onGameStateChangeTrigger -= OnCombatStart;
        CombatManager.Instance.onCombatCleanup -= HandleCombatCleanup;
    }

    public abstract void OnAwake(); // Runs in the awake

    public abstract void OnStart(); // Runs after the first frame in the start corutine


    public abstract void OnCombatStart(GameState gameState);
    
    public abstract void OnEntityTurn();

    protected abstract void UpdateEntityStatsUI(); // Update entity stats UI

    protected abstract void UpdateEntityUI(bool active);
    
    public virtual void OnDamageTaken(int damage,DamageType damageType)
    {
        if (entityStats.defenseBonus == 0) PlayAnimation(HIT_ANIMATION);
        else PlayAnimation(GUARD_HIT_ANIMATION);

        if (isInvencible) return;
        entityStats.health -= CalculateDamageReceived(damage,damageType);

    

        if(entityStats.health <= 0)
        {
            entityStats.health = 0;
            // TODO Add death animation call here!
        }

        UpdateEntityStatsUI();
    }

    public virtual void OnHeal()
    {
        OnResourceGain(ResourceType.health, CalculateHealing(Mathf.CeilToInt(currentSkill.baseDamage)));
        UpdateEntityStatsUI();
        // TODO ADD HEALING VISUALS HERE.
    }

    public virtual void OnBuff(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.offense:
                if (buffParticles) buffParticles.Play();
                else Debug.LogWarning("No buff particles found, did you forgot to add them?");
                entityStats.buffBonus = 1f;
                break; 
            case BuffType.defense:
                entityStats.defenseBonus = 1;
                break;
        }

    }
    
    public virtual void PreSelectSkill(Skill skill)
    {
        preSelectedSkill = skill;
    }

    public abstract void PerformAction(Skill skill);

    public virtual void OnRoundFinish()
    {

    }

    public abstract void TargetEntity(int entitySlot);

    public abstract void AttackEntity();

    public abstract void CombatUICleanUp();

    public int CalculateDamageDealt()
    {
        // Calculate the initial damage
        float baseDamageMultiplier = currentSkill.damageType == DamageType.physical ? entityStats.physicalDamage : entityStats.magicDamage ;
        baseDamageMultiplier /= 100;
        float baseSkillDamage = currentSkill.baseDamage;
        float baseDamage = baseSkillDamage * baseDamageMultiplier;

        // Check if its crit or not
        bool isCrit = Random.Range(0.0f, 1.0f) < entityStats.critRate;
        baseDamage *= isCrit?2.5f:1.0f;

        // Check if there is a damage buff and apply it
        float bonusDamage = baseSkillDamage * entityStats.buffBonus;
        baseDamage += bonusDamage;

        // Round it and send it.
        int totalDamage = Mathf.CeilToInt(baseDamage);

        //Debug.Log($"Damage calculated by {transform.name} is {totalDamage}, and it was crit? {isCrit}");
        return totalDamage;
    }

    public int CalculateDamageReceived(int damageReceived,DamageType damageReceivedType)
    {
        float totalDamage = damageReceived;
        float defenseRate;
        // Calculate Initial defense Rating
        float baseDefenseRate = damageReceivedType == DamageType.physical ? entityStats.physicalArmor : entityStats.magicArmor ;
        defenseRate = baseDefenseRate / 100;

        // Calculate bonus defense rate 
        float bonusDefenseRate = entityStats.defenseBonus;
        defenseRate += bonusDefenseRate;
        totalDamage = totalDamage - (totalDamage * defenseRate);
        //Debug.Log($"Damage result: Defense Rate: {defenseRate}, Bonus Defense Rate: {bonusDefenseRate}, Total Damage: {totalDamage}");
        totalDamage = Mathf.Abs(totalDamage);
       
        return Mathf.CeilToInt(totalDamage);
    }

    public int CalculateHealing(int baseHealing)
    {
        float healAmount = baseHealing + (entityStats.magicDamage * 0.2f);
        return Mathf.CeilToInt(healAmount);
    }

    private void HandleCombatCleanup()
    {
        PlayAnimation(IDLE_OUT);
        CombatUICleanUp();
    }

    public void PlayAnimation(string nextAnimation)
    {
        animator.Play(nextAnimation,0,0.0f);
        currentAnimation = nextAnimation;
    }

    public virtual void OnResourceGain(ResourceType resourceType, int resourceGain)
    {
        switch (resourceType)
        {
            case ResourceType.energy:
                entityStats.energy += resourceGain;
                if (entityStats.energy > entityData.stats.energy) entityStats.energy = entityData.stats.energy;
                break;
            case ResourceType.ammo:
                entityStats.ammo += resourceGain;
                if(entityStats.ammo > entityData.stats.ammo) entityStats.ammo = entityData.stats.ammo;
                break;
            case ResourceType.health:
                entityStats.health += resourceGain;
                if (entityStats.health > entityData.stats.health) entityStats.health = entityData.stats.health;
                break;
        }
        UpdateEntityStatsUI();
    }

    #region Corutines
    protected IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        PlayAnimation(ENTRANCE_ANIMATION);
    }

    protected IEnumerator TurnCommandsVisuals(bool active, float delay)
    {
        yield return new WaitForSeconds(delay);
        //commandsVisuals.SetActive(active);
        //combatNavigation.StartCombatWindows();
        UpdateEntityUI(active);

        //if (active) EventSystem.current.SetSelectedGameObject(firstCommandSelected);
        //EventSystem.current.UpdateModules();
    }

    #endregion

    #region Enemy Entity ONLY

    public virtual void OpenTargetWindow(bool active,bool preSelect)
    {
        if (entityData.entityID == -1) return;
 
        targetUI.SetActive(active);
        if(preSelect) targetButton.Select();
    }
    public void CloseTargetWindow()
    {
        targetUI.SetActive(false);
    }
    public void SetEntityData(EntityData data)
    {
        entityData = data;
        skills = entityData.avaliableSkills;
        animator.runtimeAnimatorController = data.entityAnimator;
    }

    public void Next()
    {
        CombatManager.Instance.OnTurnFinished();
    }

    #endregion

    //[SerializeField] private bool invencible; <-- Global
    //[SerializeField] private EntityData data; <-- Global
    //[SerializeField] private Stats entityStats; <-- Global
    //[SerializeField] private Animator animator; <-- Global
    //[SerializeField] private Skill[] skillList;  <-- Enemy Specific
    //[Header("Health Visuals: "), Space(10)]
    //[SerializeField] private GameObject healthVisuals; <-- Enemy Specific
    //[SerializeField] private TextMeshProUGUI healthText; <-- Enemy Specific

    //private Skill currentSkill; <-- Global
    //private IEnumerator Start()
    //{
    //    StartCoroutine(TurnLifeVisuals(false, 0.0f));
    //    yield return new WaitForEndOfFrame();
    //    GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
    //    CombatManager.Instance.onCombatCleanup += HandleCombatCleanUp;
    //   // CombatManager.Instance.onCombatStart += HandleInitialization;
    //}

    //private void OnEnable()
    //{
    //    if (CombatManager.Instance == null) return;
    //    GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
    //    CombatManager.Instance.onCombatCleanup += HandleCombatCleanUp;
    //    //  CombatManager.Instance.onCombatStart += HandleInitialization;
    //}

    //private void OnDisable()
    //{
    //    GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
    //    CombatManager.Instance.onCombatCleanup -= HandleCombatCleanUp;
    //    //  CombatManager.Instance.onCombatStart -= HandleInitialization;
    //}
    //string maxHealth;
    //private void HandleCombatStart(GameState gameState)
    //{

    //    switch (gameState)
    //    {
    //        case GameState.combatPreparation:
    //            entityStats.health = data.stats.health;
    //            maxHealth = entityStats.health.ToString();
    //            // Handle UI update
    //            healthText.SetText($"{entityStats.health} / {maxHealth}");
    //            break;
    //        case GameState.combatReady:
    //            StartCoroutine(DelayEntrance());

    //            break;
    //    }

    //}

    //private void HandleCombatCleanUp()
    //{
    //    StartCoroutine(TurnLifeVisuals(false, 0.0f));
    //    animator.Play("IdleOut");
    //}
    //private IEnumerator DelayEntrance()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    animator.Play("Entrance");
    //    StartCoroutine(TurnLifeVisuals(true, 0.2f));
    //}

    //private IEnumerator TurnLifeVisuals(bool active, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    healthVisuals.SetActive(active);
    //}


    //public void ReceiveDamage(int damage, bool isMagic)
    //{
    //    if(invencible == false) entityStats.health -= damage;
    //    healthText.SetText($"{entityStats.health} / {maxHealth}");
    //    animator.Play("Hit");
    //}

    //public void PerformAttack()
    //{
    //    currentSkill = null;
    //    int skillSelect = Random.Range(0, skillList.Length);
    //    currentSkill = skillList[skillSelect];
    //    animator.Play(currentSkill.animationKey);
    //   // CombatManager.Instance.OnActionFinished();
    //}
    //// Damage calculation for enemies is SkillDamage + a percentage of the skill damage based on its stat
    //public int GetDamage(bool isMagic)
    //{
    //    float damage = isMagic ? entityStats.magicDamage : entityStats.physicalDamage / 100f;
    //    float totalDamage;
    //    totalDamage = currentSkill.baseDamage + (currentSkill.baseDamage * damage);
    //    Debug.Log("Total Damage sent to player: " + totalDamage);
    //    return Mathf.CeilToInt(totalDamage);
    //}
}
