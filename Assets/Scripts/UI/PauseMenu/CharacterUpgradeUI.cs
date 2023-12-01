using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("Main Windows")]
  //  [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private StatUpgrade[] statUpgrades;
   // private void currentButtonPressed;
    private bool inCharacterWindow = true;
    [SerializeField] private TextMeshProUGUI xpText;

    [SerializeField] private GameObject defaultSelection;
    [SerializeField] private Button defaultButton;
    private EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        inCharacterWindow = false;
        SwapWindows();
        firstSelectedButton.Select();
        UpdateXPUI();
    }

    private void Update()
    {
        //if(eventSystem.currentSelectedGameObject == null)
        //{
            
        //    defaultButton.Select();
        //}
    }

    public void SwapWindows()
    {
        inCharacterWindow = !inCharacterWindow;
    }

    public void UpdateXPUI()
    {

        xpText.SetText($"XP: {SkillManager.Instance.GetCurrentXP()}");
        foreach( var upgradeButton in statUpgrades )
        {
            upgradeButton.UpdateUI();
        }

        for (int i = 0; i < statUpgrades.Length; i++)
        {
            if (statUpgrades[i].CanBePressed())
            {
                Debug.Log("This button can be pressed");
                upgradeButtons[i].Select();
                break;
            }
            else
            {
                defaultButton.Select();
            }
        }
    }
}
