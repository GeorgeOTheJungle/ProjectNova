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
    [SerializeField] private Image healthBar;
    [SerializeField] private Image followBar;
    [SerializeField] private Animator fakeHealthBar;

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

        CombatManager.Instance.GetPlayerEntity().OnDamageTaken(CalculateDamageDealt(damageMultiplier,currentSkill.baseDamage), currentSkill.damageType);
    }

    public override void OnCombatStart(GameState gameState)
    {
        if (entityData == null) return;
        //currentSkill = null;
        currentEnemySkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 0.0f));
                skills = entityData.avaliableSkills;
                skillLenght = skills.Count;
                entityState = EntityState.idle;
                UpdateEntityStatsUI();
                currentEnemySkill = enemySkillManager.GetEnemySkill(entityData.entityID);

                statsGO.SetActive(true);
                fakeHealthBar.Play("IdleOut");
                break;
        }

    }
    public override void OnEntityTurn()
    {
        if(entityState == EntityState.dead)
        {
            CombatManager.Instance.OnTurnFinished();
            return;
        }

        currentSkill = null;
        entityState = EntityState.thinking;
        float thinkTime = Random.Range(0.25f, 0.5f);

        Invoke(nameof(ChooseRandomAction), thinkTime);
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
        if(id == -1)
        {
            currentEnemySkill.DoRandomSkill();
            return;
        }
        currentEnemySkill.DoSkill(id);
    }
    protected override void UpdateEntityStatsUI()
    {
        float fill = (float)entityStats.health / (float)entityData.stats.health;
        healthBar.fillAmount = fill;
        if(entityStats.health <= 0)
        {
            statsGO.SetActive(false);
            fakeHealthBar.SetTrigger("endTrigger");
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
}
