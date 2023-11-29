using Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEntity : Entity
{
    [Header("Enemy Specifics: "), Space(10)]
    [SerializeField] private int targetID;
    [SerializeField] private List<SkillEnemy> skills;
    [Header("References: "), Space(10)]
    [SerializeField] private GameObject uiVisuals;
    [SerializeField] private GameObject statsGO;
    [SerializeField] private GameObject healthBarGO;
    [Header("Entity UI refereneces:"), Space(10)]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image followBar;
    [Header("Boss UI references:"), Space(10)]
    [SerializeField] private List<int> skillPattern;
    [SerializeField] private GameObject shadow;
    private int currentSkillPattern;
    [Space(10)]
    private int skillLenght;
    private EnemySkill currentEnemySkill;
    private EnemySkillManager enemySkillManager;

    private SkillEnemy currentSkill;

    public override void OnAwake()
    {
        enemySkillManager = GetComponentInChildren<EnemySkillManager>();
    }
    public override void OnStart()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }
    public override void AttackEntity()
    {
        float damageMultiplier = currentSkill.damageType == DamageType.physical ? entityStats.physicalDamage : entityStats.magicDamage;

        int damage = CalculateDamageDealt(damageMultiplier, currentSkill.baseDamage);
        bool isCrit = UnityEngine.Random.Range(0.0f, 1.0f) < entityStats.critRate;
        damage *= Mathf.CeilToInt(isCrit ? 2.5f : 1.0f);

        CombatManager.Instance.GetPlayerEntity().OnDamageTaken(damage,
            currentSkill.damageType, currentSkill.statusEffectType, isCrit);
    }

    public override void OnCombatStart(GameState gameState)
    {
        shadow.SetActive(entityData != null);
        if (entityData == null) return;
        // Reset data and stats effects 
        currentEnemySkill = null;
        RemoveStatusEffects();
        currentSkillPattern = 0;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());

                skills = entityData.avaliableSkills;
                skillLenght = skills.Count;
                currentEnemySkill = enemySkillManager.GetEnemySkill(entityData.entityID);

                UpdateEntityStatsUI(false);

                if (entityData.entityType == EntityType.enemy)
                {
                    // statsGO.SetActive(true);
                    StartCoroutine(TurnCommandsVisuals(true, 0.0f));
                }
                else if (entityData.entityType == EntityType.boss)
                {
                    // Create pattern

                    int patternLenght = Random.Range(6, 12);
                    for (int i = 0;i < patternLenght;i++)
                    {
                        skillPattern.Add(Random.Range(0, skillLenght));
                    }

                    // Set UI
                    BossUI.Instance.OpenBossUI();
                    BossUI.Instance.SetBossUI(entityStats.health);
                }

                entityState = EntityState.idle;
                break;
        }

    }
    public override void OnEntityTurn()
    {
        if (entityState == EntityState.dead)
        {
            CombatManager.Instance.OnTurnFinished();
            return;
        }

        if (entityStats.defenseBonus > 0.0f)
        {
            Debug.LogWarning("TODO: GUARD TO IDLE ANIMATIONS");
            PlayAnimation(Constants.GUARD_END_ANIMATION);
            entityStats.defenseBonus = 0.0f;
        }
        currentSkill = null;
        entityState = EntityState.thinking;
        CombatManager.Instance.IsPlayerTurn(false);
        float thinkTime = Random.Range(0.25f, 0.5f);

        if(entityData.entityType == EntityType.boss)
        {
            DoActionPattern();
        }
        else
        {
            Invoke(nameof(ChooseRandomAction), thinkTime);
        }

    }

    private void DoActionPattern()
    {

        currentSkill = skills[skillPattern[currentSkillPattern]];
        currentSkillPattern++;
        if (currentSkillPattern >= skillPattern.Count) currentSkillPattern = 0;
        entityState = EntityState.acting;
        PerformAction(currentSkill);
    }

    private void ChooseRandomAction()
    {
        currentSkill = skills[Random.Range(0, skillLenght)];
        entityState = EntityState.acting;
        PerformAction(currentSkill);
    }
    public override void PerformAction(SkillEnemy skill)
    {
        // Do visuals
        PlayAnimation(currentSkill.animationKey);

    }

    public void PerformSkill(int id)
    {
        if (id == -1)
        {
            currentEnemySkill.DoRandomSkill();
            return;
        }
        currentEnemySkill.DoSkill(id);
    }

    protected override void UpdateEntityStatsUI(bool healthHit)
    {
        switch (entityData.entityType)
        {
            case EntityType.enemy:
                float fill = (float)entityStats.health / (float)entityData.stats.health;
                healthBar.fillAmount = fill;
                if (entityStats.health <= 0)
                {
                    statsGO.SetActive(false);
                }
                break;
            case EntityType.boss:
                BossUI.Instance.UpdateBossUI(entityStats.health);
                break;
        }


    }
    public override void TargetEntity(int entitySlot)
    {

    }
    public override void CombatUICleanUp()
    {
        statsGO.SetActive(false);
    }
    protected override void UpdateEntityUI(bool active)
    {
        statsGO.SetActive(active);
    }

    public override void OpenTargetWindow(bool active, bool preSelect)
    {
        if (entityData.entityID == -1) return; // This is to exclude the player from this.
        if (entityState == EntityState.dead) return;
        targetUI.SetBool("isActive", active);
        targetUI.SetBool("isTargeted", preSelect);
    }

    public void OnEntitySelected()
    {
        // CombatManager.Instance.SetTarget(targetID);
    }
    public override void MoveEntityToTarget()
    {

    }

    public override void OnHeal()
    {
        OnResourceGain(ResourceType.health, CalculateHealing(Mathf.CeilToInt(currentSkill.baseDamage)), RegenStyle.None);
        base.OnHeal();
    }
}
