using JetBrains.Annotations;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    [SerializeField]
    private EntityData playerData;
    [SerializeField] private Stats playerStats;
    [Space(10)]
    [SerializeField] private StatsLevel statsLevel;
    [SerializeField] private Skill shootSkill;
    [SerializeField] private Skill punchSkill;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerStats = playerData.stats;
    }

    public Stats GetPlayerStats() => playerStats;

    public void UpdatePlayerStats()
    {
        playerStats.health = playerData.stats.health + (5 * statsLevel.healthLevel);
        playerStats.energy = playerData.stats.energy + (5 * statsLevel.energyLevel);
        playerStats.ammo = playerData.stats.ammo + (1 * statsLevel.ammoLevel);

        playerStats.physicalDamage = playerData.stats.physicalDamage + (5 * statsLevel.physicalDamageLevel);
        playerStats.physicalArmor = playerData.stats.physicalArmor + (5 * statsLevel.physicalArmorLevel);

        playerStats.magicDamage = playerData.stats.magicDamage + (5 * statsLevel.magicDamageLevel);
        playerStats.magicArmor = playerData.stats.magicArmor + (5 * statsLevel.magicArmorLevel);

        playerStats.critRate = playerData.stats.critRate + (5 * statsLevel.critRateLevel);
    }

    public void UpgradePlayerStat(PlayerStat stat)
    {
        switch (stat)
        {
            case PlayerStat.health:
                statsLevel.healthLevel++;
                break;
            case PlayerStat.energy:
                statsLevel.energyLevel++;
                break;
            case PlayerStat.ammo:
                statsLevel.ammoLevel++;
                break;
            case PlayerStat.physicalDmg:
                statsLevel.physicalDamageLevel++;
                break;
            case PlayerStat.physicalArmor:
                statsLevel.physicalArmorLevel++;
                break;
            case PlayerStat.magicalDmg:
                statsLevel.magicDamageLevel++;
                break;
            case PlayerStat.magicalArmor:
                statsLevel.magicArmorLevel++;
                break;
            case PlayerStat.critRate:
                statsLevel.critRateLevel++;
                break;
            case PlayerStat.shoot:
                shootSkill.UpdgradeSkill();
                break;
            case PlayerStat.punch:
                punchSkill.UpdgradeSkill();
                break;
        }
        UpdatePlayerStats();
    }

    public int GetCurrentPlayerStat(PlayerStat stat)
    {
        switch (stat)
        {
            case PlayerStat.health:
                return statsLevel.healthLevel;

            case PlayerStat.energy:
                return statsLevel.energyLevel;

            case PlayerStat.ammo:
                return statsLevel.ammoLevel;

            case PlayerStat.physicalDmg:
                return statsLevel.physicalDamageLevel;

            case PlayerStat.physicalArmor:
                return statsLevel.physicalArmorLevel;

            case PlayerStat.magicalDmg:
                return statsLevel.magicDamageLevel;

            case PlayerStat.magicalArmor:
                return statsLevel.magicArmorLevel;

            case PlayerStat.critRate:
                return statsLevel.critRateLevel;
            case PlayerStat.shoot:
                return shootSkill.level;

            case PlayerStat.punch:
                return punchSkill.level;

        }
        return 0;
    }


}
