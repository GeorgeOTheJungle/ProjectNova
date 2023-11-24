using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    [Header("Stats texts references: "), Space(10)]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image energyBar;
    [SerializeField] private Image ammoBar;

    [SerializeField] private TextMeshPro actionText;
    [SerializeField] private Entity player;

    float maxHealth;
    float maxEnergy;
    float maxAmmo;
    private IEnumerator Start()
    {
        visual.SetActive(false);
        actionText.gameObject.SetActive(false);
        actionText.SetText(string.Empty);

        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatUpdate;
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatUpdate;
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= HandleCombatUpdate;
    }

    private void HandleCombatUpdate(GameState state)
    {
        bool active = state == GameState.combatReady;

        visual.SetActive(active);
        actionText.gameObject.SetActive(active);
        // Initialize stats on texts
        maxHealth = player.entityStats.health;
        maxEnergy = player.entityStats.energy;
        maxAmmo = player.entityStats.ammo;

        UpdateCombatStats();
    }

    public void UpdateCombatStats()
    {
        healthBar.fillAmount = player.entityStats.health / maxHealth;
        energyBar.fillAmount = player.entityStats.energy / maxEnergy;
        ammoBar.fillAmount = player.entityStats.ammo / maxAmmo;
    }
}
