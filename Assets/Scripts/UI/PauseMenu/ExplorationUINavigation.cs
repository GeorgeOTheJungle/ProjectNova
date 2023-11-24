using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationUINavigation : MonoBehaviour
{
    private bool uiOpen = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject characterWindow;
    [SerializeField] private GameObject skillUpgradeMenu;
    [SerializeField] private Button mainMenuFirstButton;


    private IEnumerator Start()
    {
        pauseMenu.SetActive(false);
        characterWindow.SetActive(false);
        skillUpgradeMenu.SetActive(false);
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.pauseTrigger += HandlePauseMenu;
    }

    private void OnDisable()
    {
        InputComponent.Instance.pauseTrigger -= HandlePauseMenu;
    }
    private void HandlePauseMenu()
    {
        if (GameManager.Instance.CurrentGameState() == Enums.GameState.messagePrompt) return;
        if (GameManager.Instance.CurrentGameState() != Enums.GameState.exploration &&
            GameManager.Instance.CurrentGameState() != Enums.GameState.paused) return;

        uiOpen = !uiOpen;
        if(GameManager.Instance.CurrentGameState() == Enums.GameState.paused)
        {
            GameManager.Instance.ChangeGameState(Enums.GameState.exploration);
        } else
        {
            GameManager.Instance.ChangeGameState(Enums.GameState.paused);
        }

        pauseMenu.SetActive(uiOpen);
        mainMenu.SetActive(uiOpen);
        characterWindow.SetActive(false);
        skillUpgradeMenu.SetActive(false);
        mainMenuFirstButton.Select();
    }

   
}
