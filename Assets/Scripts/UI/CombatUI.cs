using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    [Header("Stats texts references: "), Space(10)]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private TextMeshPro actionText;
    [SerializeField] private CombatPlayer player;

    string maxHealth;
    string maxEnergy;
    string maxAmmo;
    private IEnumerator Start()
    {
        visual.SetActive(false);
        actionText.gameObject.SetActive(false);
        actionText.SetText(string.Empty);

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

    private void HandleCombatStart(GameState state)
    {
        visual.SetActive(true);
        actionText.gameObject.SetActive(true);
        // Initialize stats on texts
        maxHealth = player.playerStats.health.ToString();
        maxEnergy = player.playerStats.energy.ToString();
        maxAmmo = player.playerStats.ammo.ToString();

        UpdateCombatStats();
    }

    public void UpdateCombatStats()
    {
        healthText.SetText($"{player.playerStats.health} / {maxHealth}");
        energyText.SetText($"{player.playerStats.energy} / {maxEnergy}");
        ammoText.SetText($"{player.playerStats.ammo} / {maxAmmo}");
    }
}
