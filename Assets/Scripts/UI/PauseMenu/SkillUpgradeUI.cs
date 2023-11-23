using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUpgradeUI : MonoBehaviour
{
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button secondSelectedButton;
    [Space]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button additionalButton;

    [SerializeField] private Button backButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private TextMeshProUGUI currentXPText;
    [Space]
    [SerializeField] private GameObject skillPreviewWindow;
    [SerializeField] private GameObject skillUpgradeWindow;
    [Space(10)]
    [SerializeField] private List<SkillDisplay> skillDisplayList;
    [SerializeField] private List<PlayerSkill> skillList;
    [SerializeField] private List<LevelIndicatorUI> levelIndicatorUIs;
    [Space(10)]
    [Header("Skill description references: ")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    private int skillCount;
    private int currentSkill;
    private PlayerSkill currentSkillSelected;

    private void Start()
    {
        skillList = SkillManager.Instance.GetAllSkills();
        skillCount = skillList.Count;

        skillPreviewWindow.SetActive(true);
        skillUpgradeWindow.SetActive(false);
        for (int i = 0;i < skillCount; i++)
        {
            skillDisplayList[i].SetDisplay(skillList[i],i);
        }
        foreach(var ind in levelIndicatorUIs)
        {
            ind.SetVisual(false);
        }
    }

    private void OnEnable()
    {
        firstSelectedButton.Select();
        skillPreviewWindow.SetActive(true);
        skillUpgradeWindow.SetActive(false);

        backButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        skillPreviewWindow.SetActive(true);
        skillUpgradeWindow.SetActive(false);

        backButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(false);
    }

    public void OnSkillSelected(PlayerSkill skillSelected,int id)
    {
        skillPreviewWindow.SetActive(false);
        skillUpgradeWindow.SetActive(true);
        secondSelectedButton.Select();

        currentSkill = id;
        currentSkillSelected = skillSelected;

        backButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(true);
        
        UpdateSkillInfo();
    }

    public void NavigateSkills(int dir)
    {
        currentSkill += dir;
        if (currentSkill < 0) currentSkill = skillList.Count;
        else if (currentSkill > skillList.Count - 1) currentSkill = 0;

        currentSkillSelected = skillList[currentSkill];
        // Check if skill is capable to be upgrades
        UpdateSkillInfo();

    }

    private void UpdateSkillInfo()
    {
        string cost = string.Empty;

        // Check if current skill is already maxed out if yes, ignore the other parts
        if(currentSkillSelected.level > 3)
        {
            upgradeButton.interactable = false;
            upgradeText.SetText("Maxed!");
            cost = "Maxed!";
        }
        else
        {
            if (currentSkillSelected.unlocked)
            {
                upgradeText.SetText("Upgrade");
                upgradeButton.interactable = SkillManager.Instance.SkillCanBeUpgraded(currentSkillSelected);
                cost = $"Upgrade cost: {currentSkillSelected.RequiredXp()}";
            }
            else
            {
                upgradeText.SetText("Unlock");
                upgradeButton.interactable = SkillManager.Instance.SkillCanBeUnlocked(currentSkillSelected);
                cost = $"Unlock cost: {currentSkillSelected.initialUnlockCost}";
            }

        }

        // Check if the first selected button is interactable or not. 
        if (upgradeButton.interactable == false && EventSystem.current.alreadySelecting == false) additionalButton.Select();

        // Update bottom Text
        string levelText = currentSkillSelected.level <= 3 ? currentSkillSelected.level.ToString() : "Maxed";
        skillNameText.SetText($"{currentSkillSelected.skillName} ({levelText}) | {cost}");
        skillDescriptionText.SetText(currentSkillSelected.skillDescription);
        currentXPText.SetText($"XP: {SkillManager.Instance.GetCurrentXP()}");

        foreach (var ind in levelIndicatorUIs)
        {
            ind.SetVisual(false);
        }

        for(int i = 0;i < currentSkillSelected.level; i++)
        {
            levelIndicatorUIs[i].SetVisual(true);
        }
    }

    public void UpgradeSkill()
    {
        if(currentSkillSelected.unlocked == false)
        {
            SkillManager.Instance.UnlockSkill(currentSkillSelected);
        }
        else
        {
            SkillManager.Instance.UpgradeSkill(currentSkillSelected);
        }
        UpdateSkillInfo();
    }

    public void ResetSkills()
    {
        SkillManager.Instance.ResetXP();
        UpdateSkillInfo();
    }
}
