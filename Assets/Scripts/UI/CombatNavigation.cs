using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatNavigation : MonoBehaviour
{
    public static CombatNavigation Instance;
    [Header("Action Window"), Space(10)]
    private bool onMenu = false;
    [SerializeField] private GameObject actionSelectedButton;
    [SerializeField] private GameObject actionsWindow;
    [SerializeField] private GameObject[] menuWindows;
    [SerializeField] private Button[] firstSelected;

    [Header("Command Window"), Space(10)]
    [SerializeField] private GameObject commandSelectedButton;
    [SerializeField] private GameObject commandsWindow;
    [Space(10)]
    [SerializeField] private GameObject shootCommandGO;
    [SerializeField] private GameObject punchCommandGO;

    [Space(10)]
    public int lastWindow; // READ ONLY

    [SerializeField] private TextMeshProUGUI actionText;

    private PlayerEntity playerEntity;
    private bool canReturn = false;
    private void Awake()
    {
        Instance = this;
        playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEntity>();
    }

    private void Update()
    {
        if (onMenu == false) return;
        if (canReturn == false) return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleReturnToLastWindow();
        }
    }
    int buttonIndex;
    int currentWindow;
    public void OpenWindow(int menuIndex)
    {
        lastWindow = menuIndex;
        currentWindow = menuIndex;
        buttonIndex = menuIndex;

        foreach (GameObject menu in menuWindows)
        {
            menu.SetActive(false);
        }

        menuWindows[menuIndex].SetActive(true);

        // Check if is attack window
        if(menuIndex == 1)
        {
            
            commandsWindow.SetActive(true);
            actionsWindow.SetActive(false);

            //Check player ammo
            if (playerEntity.HasAmmo(1) == false)
            {
                shootCommandGO.SetActive(false);
                punchCommandGO.SetActive(true);
                buttonIndex++;
            }
            else
            {
                shootCommandGO.SetActive(true);
                punchCommandGO.SetActive(false);

            }
        }
        if (menuIndex != 2) firstSelected[buttonIndex].Select();
       
        else firstSelected[3].Select();
    }

    public void HandleReturnToLastWindow()
    {
        //lastWindow = currentWindow;
        currentWindow--;
        
        if (currentWindow < 0) currentWindow = 0;
        OpenWindow(currentWindow);
    }

    public void StartCombatWindows()
    {
        OpenWindow(0);
        EnableMenuNavigation(true);
        actionText.SetText("Act");
        actionText.gameObject.SetActive(true);
    }

    public void OnSkillSelected()
    {
        foreach (GameObject menu in menuWindows)
        {
            menu.SetActive(false);
        }
        actionText.SetText(string.Empty);
        actionText.gameObject.SetActive(false);
        onMenu = false;
        canReturn = false;
    }

    public void UpdateActionText(string text)
    {
        if (actionText.IsActive() == false) return;
        actionText.SetText(text);
    }

    public void HideAllWindows()
    {
        for(int i = 0;i < menuWindows.Length; i++)
        {
            if (menuWindows[i].activeSelf) lastWindow = i;
            menuWindows[i].SetActive(false);  
        }
        onMenu = false;
        canReturn = false;
    }

    public void EnableMenuNavigation(bool active)
    {
        onMenu = active;
        if(active) Invoke(nameof(ReturnDelay), 0.1f);
    }

    private void ReturnDelay() => canReturn = true;
}
