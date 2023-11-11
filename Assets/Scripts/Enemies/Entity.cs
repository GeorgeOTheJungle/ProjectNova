using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using TMPro;
using Enums;
using UnityEngine.EventSystems;

public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] private bool invencible;
    [SerializeField] private EntityData data;
    [SerializeField] private Stats entityStats;
    [SerializeField] private Animator animator;
    [SerializeField] private Skill[] skillList;
    [Header("Health Visuals: "), Space(10)]
    [SerializeField] private GameObject healthVisuals;
    [SerializeField] private TextMeshProUGUI healthText;

    private Skill currentSkill;
    private IEnumerator Start()
    {
        StartCoroutine(TurnLifeVisuals(false, 0.0f));
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
        CombatManager.Instance.onCombatCleanup += HandleCombatCleanUp;
       // CombatManager.Instance.onCombatStart += HandleInitialization;
    }

    private void OnEnable()
    {
        if (CombatManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
        CombatManager.Instance.onCombatCleanup += HandleCombatCleanUp;
        //  CombatManager.Instance.onCombatStart += HandleInitialization;
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatStart;
        CombatManager.Instance.onCombatCleanup -= HandleCombatCleanUp;
        //  CombatManager.Instance.onCombatStart -= HandleInitialization;
    }
    string maxHealth;
    private void HandleCombatStart(GameState gameState)
    {

        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats.health = data.stats.health;
                maxHealth = entityStats.health.ToString();
                // Handle UI update
                healthText.SetText($"{entityStats.health} / {maxHealth}");
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());

                break;
        }

    }

    private void HandleCombatCleanUp()
    {
        StartCoroutine(TurnLifeVisuals(false, 0.0f));
        animator.Play("IdleOut");
    }
    private IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.5f);
        animator.Play("Entrance");
        StartCoroutine(TurnLifeVisuals(true, 0.2f));
    }

    private IEnumerator TurnLifeVisuals(bool active, float delay)
    {
        yield return new WaitForSeconds(delay);
        healthVisuals.SetActive(active);
    }


    public void ReceiveDamage(int damage, bool isMagic)
    {
        if(invencible == false) entityStats.health -= damage;
        healthText.SetText($"{entityStats.health} / {maxHealth}");
        animator.Play("Hit");
    }

    public void PerformAttack()
    {
        currentSkill = null;
        int skillSelect = Random.Range(0, skillList.Length);
        currentSkill = skillList[skillSelect];
        animator.Play(currentSkill.animationKey);
       // CombatManager.Instance.OnActionFinished();
    }
    // Damage calculation for enemies is SkillDamage + a percentage of the skill damage based on its stat
    public int GetDamage(bool isMagic)
    {
        float damage = isMagic ? entityStats.magicDamage : entityStats.physicalDamage / 100f;
        float totalDamage;
        totalDamage = currentSkill.baseDamage + (currentSkill.baseDamage * damage);
        Debug.Log("Total Damage sent to player: " + totalDamage);
        return Mathf.CeilToInt(totalDamage);
    }
}
