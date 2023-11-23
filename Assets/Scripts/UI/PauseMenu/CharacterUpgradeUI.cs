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
   // private void currentButtonPressed;
    private bool inCharacterWindow = true;
    [SerializeField] private TextMeshProUGUI xpText;

    [SerializeField] private GameObject defaultSelection;
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
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null) return;

        if(eventSystem.currentSelectedGameObject.activeSelf == false)
        {
            eventSystem.SetSelectedGameObject(defaultSelection);
        }
        //if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject != defaultSelection)
        //    defaultSelection = eventSystem.currentSelectedGameObject;
        //else if (defaultSelection != null && eventSystem.currentSelectedGameObject == null)
        //    eventSystem.SetSelectedGameObject(defaultSelection);
    }

    public void SwapWindows()
    {
        inCharacterWindow = !inCharacterWindow;
       // characterNameText.SetText(inCharacterWindow ? "Catherine" : "Nova Gun");
    }

    public void UpdateXPUI()
    {
        xpText.SetText($"XP: {SkillManager.Instance.GetCurrentXP()}");
    }

    /* 0 - Health;
     * 1 - Energy;
     * 2 - Ammo;
     * 3 - Damage
    */
}
