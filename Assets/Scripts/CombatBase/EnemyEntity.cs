using Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEntity : Entity
{
    [Header("Enemy Specifics: "), Space(10)]

    [Header("References: "), Space(10)]
    [SerializeField] private GameObject statsGO;
    [SerializeField] private TextMeshProUGUI lifeText;

    [Header("Avaliable Skills: "), Space(10)]
    public Skill[] skills;

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
        currentSkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 0.0f));
                break;
        }
        skillLenght = skills.Length;
        UpdateEntityStatsUI();
    }

    public override void OnEntityTurn()
    {
        currentSkill = null;

        // Choose skill to use
        currentSkill = skills[Random.Range(0, skillLenght)];
        PerformAction(currentSkill);
    }

    public override void OnStart()
    {
        
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
}
