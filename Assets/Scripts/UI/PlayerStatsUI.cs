using Enums;
using Structs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI physicalDmgText;
    [SerializeField] private TextMeshProUGUI magicalDmgText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI critRateText;

    private void Update()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        Stats stats = PlayerStatsManager.Instance.GetPlayerStats();
        StatsLevel statsLevel = PlayerStatsManager.Instance.GetStatsLevel();
        healthText.SetText($"- {stats.health} (+ {statsLevel.healthLevel * 5})");
        energyText.SetText($"- {stats.energy} (+ {statsLevel.energyLevel * 5})");
        physicalDmgText.SetText($"- {stats.physicalDamage} (+ {statsLevel.physicalDamageLevel * 5})");
        magicalDmgText.SetText($"- {stats.magicDamage} (+ {statsLevel.magicDamageLevel * 5})");
        armorText.SetText($"- {stats.physicalArmor} (+ {statsLevel.physicalArmorLevel * 5})");
        critRateText.SetText($"- {stats.critRate} (+ {statsLevel.critRateLevel * 5})");
    }

}
