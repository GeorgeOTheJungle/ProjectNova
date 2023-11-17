using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatUpgrade : MonoBehaviour
{
    [SerializeField] private PlayerStat stat;
    [SerializeField] private LevelIndicatorUI[] levelIndicatorUIs;

    [SerializeField] private GameObject upgradeButton;

    private int level;
    private bool maxed = false;
    
    private void Start()
    {
        level = PlayerStatsManager.Instance.GetCurrentPlayerStat(stat);

        UpdateUI();
    }

    private void UpdateUI()
    {
        maxed = level >= 6;
        upgradeButton.SetActive(!maxed);

        foreach (var indicator in levelIndicatorUIs)
        {
            indicator.SetVisual(false);
        }
        levelIndicatorUIs[0].SetVisual(true);
        for (int i = 1; i < level; i++)
        {
            levelIndicatorUIs[i].SetVisual(true);
        }
        
    }

    public void UpgradeStat()
    {
        PlayerStatsManager.Instance.UpgradePlayerStat(stat);

        level = PlayerStatsManager.Instance.GetCurrentPlayerStat(stat);

        UpdateUI();
    }
}
