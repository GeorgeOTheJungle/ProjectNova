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
    [Header("References: "), Space(10)]
    [SerializeField] private GameObject uiVisuals;
    [SerializeField] private GameObject statsGO;
    [SerializeField] private GameObject healthBarGO;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image followBar;
    [SerializeField] private Animator fakeHealthBar;

    private int skillLenght;

    public override void OnAwake()
    {

    }
    public override void OnStart()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }
    public override void AttackEntity()
    {
        CombatManager.Instance.GetPlayerEntity().OnDamageTaken(CalculateDamageDealt(), currentSkill.damageType);
    }

    public override void OnCombatStart(GameState gameState)
    {
        if (entityData == null) return;
        currentSkill = null;
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
    public override void PerformAction(Skill skill)
    {
        // Do visuals
        PlayAnimation(currentSkill.animationKey);

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
        targetUI.SetBool("isActive", true);
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
