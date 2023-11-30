using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Enums;
using UnityEngine.UI;


public abstract class Entity : MonoBehaviour
{
    [Header("ENTITY: "), Space(10)]
    public EntityData entityData;
    public Stats entityStats;
    [SerializeField] protected bool isInvencible = false;
    [SerializeField] protected EntityState entityState;
    //[SerializeField] protected List<Effect> effectLists;
    [SerializeField] protected Animator targetUI;
    [SerializeField] protected Transform entityVisual;

    private Vector3 originalPosition;
    [Header("Particles Visuals: "), Space(10)]
    [SerializeField] protected GameObject damageNumbers;
    [SerializeField] protected Transform damageNumbersSpawn;
    [SerializeField] protected ParticleSystem buffParticles;

    [SerializeField] protected StatusEffect onFireEffect;
    [SerializeField] protected StatusEffect onIceEffect;
    [SerializeField] protected StatusEffect onWeakEffect;
    private string currentAnimation;

    [SerializeField] protected Animator animator;
    [HideInInspector] public SoundHandler soundHandler;
    protected int targetEntity;

    #region Initialization

    private void Awake()
    {
        soundHandler = GetComponentInChildren<SoundHandler>();  
        OnAwake();
    }

    private IEnumerator Start()
    {
        originalPosition = entityVisual.position;
        entityState = EntityState.inactive;
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;
        CombatManager.Instance.onCombatCleanup += HandleCombatCleanup;
        CombatManager.Instance.onCombatFinish += HandleCombatEnd;
        OnStart();
    }

    private void OnEnable()
    {
        if (GameManager.Instance) GameManager.Instance.onGameStateChangeTrigger += OnCombatStart;
        if (CombatManager.Instance) CombatManager.Instance.onCombatCleanup += HandleCombatCleanup;
        if (CombatManager.Instance) CombatManager.Instance.onCombatFinish += HandleCombatEnd;
    }

    private void OnDisable()
    {
        if (GameManager.Instance) GameManager.Instance.onGameStateChangeTrigger -= OnCombatStart;
        CombatManager.Instance.onCombatCleanup -= HandleCombatCleanup;
        CombatManager.Instance.onCombatFinish -= HandleCombatEnd;
    }

    public abstract void OnAwake(); // Runs in the awake

    public abstract void OnStart(); // Runs after the first frame in the start corutine

    public abstract void OnCombatStart(GameState gameState); // Event called from game manager.

    #endregion

    public abstract void OnEntityTurn();

    protected abstract void UpdateEntityStatsUI(bool healthHit); // Update entity stats UI

    protected abstract void UpdateEntityUI(bool active);

    public virtual void OnDamageTaken(int damage, DamageType damageType, StatusEffectType statusEffectType, bool wasCrit)
    {
        if (isInvencible) return;
        if (entityState == EntityState.dead || entityState == EntityState.inactive) return;

        if (entityStats.defenseBonus == 0)
        {
     
            PlayAnimation(Constants.HIT_ANIMATION);
        }
        else
        {
            Debug.LogWarning("TODO MAKE PLAYER GUARD HIT ANIMATION");
            //PlaySound(Constants.HIT_SOUND);
            PlayAnimation(Constants.GUARD_HIT_ANIMATION);
        }

        int dmg = CalculateDamageReceived(damage, damageType, wasCrit);
        if (statusEffectType == StatusEffectType.fire && onIceEffect.EffectActive() ||
            statusEffectType == StatusEffectType.ice && onFireEffect.EffectActive())
        {
            Debug.Log("Combo!");
            dmg += Mathf.CeilToInt(dmg * 0.5f);
        }
        entityStats.health -= dmg;

        CheckIfStatusEffect(statusEffectType);

        if (entityStats.health <= 0)
        {
            entityStats.health = 0;
            PlaySound(Constants.DEATH_SOUND);
            PlayAnimation(Constants.DEATH_ANIMATION);
            if(entityData.entityType != EntityType.player) CombatManager.Instance.OnEnemyDefeated();
            entityState = EntityState.dead;
            onFireEffect.RemoveEffect();
            onIceEffect.RemoveEffect();
        }
        UpdateEntityStatsUI(true);
    }

    public void PlaySound(int id)
    {
        if (soundHandler == false) return;
        if(id == -1)
        {
            soundHandler.PlayHitSound();
        } else if(id == -2)
        {
            soundHandler.PlayDeathSound();
        }
        else
        {
                soundHandler.PlayAudio(id);
        }
    }

    private void CheckIfStatusEffect(StatusEffectType statusEffectType)
    {
        if (statusEffectType != StatusEffectType.none)
        {
            float f = Random.Range(0.0f, 1.0f);
            float chance;
            switch (statusEffectType)
            {
                case StatusEffectType.fire:
                    chance = Constants.FIRE_EFFECT_CHANCE;
                    break;
                case StatusEffectType.ice:
                    chance = Constants.ICE_EFFECT_CHANCE;
                    break;
                case StatusEffectType.weaken:
                    chance = Constants.WEAK_EFFECT_CHANCE;
                    break;
                default: chance = 0.0f; break;
            }
            if (f < chance)
            {
                switch (statusEffectType)
                {
                    case StatusEffectType.fire:
                        onFireEffect.OnEffectStart(this);
                        break;
                    case StatusEffectType.ice:
                        onIceEffect.OnEffectStart(this);
                        break;
                    case StatusEffectType.weaken:
                        onWeakEffect.OnEffectStart(this);
                        break;
                }
            }
        }
    }

    public virtual void OnHeal()
    { 
        UpdateEntityStatsUI(true);
        // TODO ADD HEALING VISUALS HERE.
    }

    protected int buffDuration = 0;
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
        buffDuration = 3;

    }

    public virtual void PerformAction(SkillEnemy skill)
    {

    }

    public virtual void PerformAction(PlayerSkill skill)
    {

    }

    public virtual void OnRoundFinish()
    {
        onFireEffect.ApplyEffect();
        onIceEffect.ApplyEffect();
        onWeakEffect.ApplyEffect();
    }

    public abstract void TargetEntity(int entitySlot);

    public abstract void AttackEntity();

    public abstract void CombatUICleanUp();

    public void PlayAnimation(string nextAnimation)
    {
        animator.Play(nextAnimation, 0, 0.0f);
        currentAnimation = nextAnimation;
    }

    public virtual void OnResourceGain(ResourceType resourceType, int resourceGain, RegenStyle regenStyle)
    {
        switch (resourceType)
        {
            case ResourceType.energy:
                resourceGain *= regenStyle == RegenStyle.Punch ? 2 : 1;
                entityStats.energy += resourceGain;
                if (entityStats.energy > entityData.stats.energy) entityStats.energy = entityData.stats.energy;
                break;
            case ResourceType.ammo:
                entityStats.ammo += resourceGain;
                if (entityStats.ammo > entityData.stats.ammo) entityStats.ammo = entityData.stats.ammo;
                break;
            case ResourceType.health:
                entityStats.health += resourceGain;
                if (entityStats.health > entityData.stats.health) entityStats.health = entityData.stats.health;
                break;
        }
        UpdateEntityStatsUI(false);
    }

    public abstract void MoveEntityToTarget(); // Use this for melee attacks

    public void ReturnToOriginalPosition()
    {
        StartCoroutine(ReturnToPositionAnimation());
    }


    #region Entity Calculations
    public int CalculateDamageDealt(float _damageMultiplier, float _skillDamage)
    {
        // Calculate the initial damage
        float baseDamageMultiplier = _damageMultiplier;
        baseDamageMultiplier /= 100;
        float baseSkillDamage = _skillDamage;
        float baseDamage = baseSkillDamage * baseDamageMultiplier;

        // Check if there is a damage buff and apply it
        float bonusDamage = baseSkillDamage * entityStats.buffBonus;
        baseDamage += bonusDamage;

        // Round it and send it.
        int totalDamage = Mathf.CeilToInt(baseDamage);

        //Debug.Log($"Damage calculated by {transform.name} is {totalDamage}, and it was crit? {isCrit}");
        return totalDamage;
    }

    public int CalculateDamageReceived(int damageReceived, DamageType damageReceivedType, bool wasCrit)
    {
        float totalDamage = damageReceived;
        float defenseRate;
        // Calculate Initial defense Rating
        float baseDefenseRate = damageReceivedType == DamageType.physical ? entityStats.physicalArmor : entityStats.magicArmor;
        defenseRate = baseDefenseRate / 100;

        // Calculate bonus defense rate 
        float bonusDefenseRate = entityStats.defenseBonus;
        defenseRate += bonusDefenseRate;
        totalDamage = totalDamage - (totalDamage * defenseRate);
        //Debug.Log($"Damage result: Defense Rate: {defenseRate}, Bonus Defense Rate: {bonusDefenseRate}, Total Damage: {totalDamage}");
        totalDamage = Mathf.Abs(totalDamage);

        DamageNumbers dmgNumbers = Instantiate(damageNumbers, damageNumbersSpawn.position, Quaternion.identity).GetComponent<DamageNumbers>();
        dmgNumbers.SetDamage(Mathf.CeilToInt(totalDamage), wasCrit, damageReceivedType == DamageType.magical);
        return Mathf.CeilToInt(totalDamage);
    }

    public int CalculateHealing(int baseHealing)
    {
        float healAmount = baseHealing + (entityStats.magicDamage * 0.2f);
        return Mathf.CeilToInt(healAmount);
    }
    #endregion

    #region Combat END Methods
    private void HandleCombatCleanup()
    {
        entityState = EntityState.inactive;
        PlayAnimation(Constants.IDLE_OUT);
        CombatUICleanUp();

        RemoveStatusEffects();
        if (entityData == null) return;
        if (entityData.entityID == -1) return;
        entityData = null;
    }

    protected void RemoveStatusEffects()
    {
        onFireEffect.RemoveEffect();
        onIceEffect.RemoveEffect();
        onWeakEffect.RemoveEffect();
    }

    private void HandleCombatEnd(CombatResult result, int id)
    {
        if (entityData == null) return;
        if (entityData.entityID != -1) return;
        switch (result)
        {
            case CombatResult.escape:
                break;
            case CombatResult.defeat:
                break;
            case CombatResult.victory:
                PlayAnimation("Celebration");
                break;
        }
    }

    #endregion

    #region Corutines
    protected IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        PlayAnimation(Constants.ENTRANCE_ANIMATION);
    }

    protected IEnumerator TurnCommandsVisuals(bool active, float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateEntityUI(active);
    }

    private IEnumerator ReturnToPositionAnimation()
    {
        PlayAnimation("MeleeReturn");
        float returnSpeed = 15.0f;
        while (Vector3.Distance(entityVisual.position, originalPosition) > 0.03f)
        {
            entityVisual.position = Vector3.MoveTowards(entityVisual.position, originalPosition, returnSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        entityVisual.position = originalPosition;
        PlayAnimation("MeleeEnd");
    }

    #endregion

    #region Enemy Entity ONLY

    public virtual void OpenTargetWindow(bool active, bool preSelect)
    {

    }
    public void CloseTargetWindow()
    {
        targetUI.SetBool("isActive", false);
        targetUI.SetBool("isTargeted", false);
    }
    public void SetEntityData(EntityData data, int id)
    {
        entityData = data;
        entityState = EntityState.idle;
        //skills = entityData.avaliableSkills;
        animator.runtimeAnimatorController = data.entityAnimator;
    }

    public void OnTurnEnd()
    {
        entityState = EntityState.idle;
        CombatManager.Instance.OnTurnFinished();
    }

    public bool EntityDead()
    {
        return entityState == EntityState.dead;
    }

    public bool IsAfflictedByFire() => onFireEffect.EffectActive();

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
