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
    [SerializeField] private GameObject statsGO;
    [SerializeField] private TextMeshProUGUI lifeText;

    private int skillLenght;
    public override void AttackEntity()
    {
        CombatManager.Instance.GetPlayerEntity().OnDamageTaken(CalculateDamageDealt(), currentSkill.damageType);
    }

    public override void OnAwake()
    {
        
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

    public override void OnStart()
    {
        
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

        lifeText.SetText($"{entityStats.health} / {entityData.stats.health}");
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

    public void OnEntitySelected()
    {
        CombatManager.Instance.SetTarget(targetID);
    }

    public override void MoveEntityToTarget()
    {
        
    }
}
