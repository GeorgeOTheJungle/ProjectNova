using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;
    [SerializeField] private GameObject messageBoard;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject closePrompt;

    private bool canBeClosed = false;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.pauseTrigger += CloseMessagePrompt;
        InputComponent.Instance.interactTrigger += CloseMessagePrompt;

        messageBoard.SetActive(false);
        messageText.SetText(string.Empty);
        canBeClosed = false;
        closePrompt.SetActive(false);
    }

    private void OnDisable()
    {
        InputComponent.Instance.pauseTrigger -= CloseMessagePrompt;
        InputComponent.Instance.interactTrigger -= CloseMessagePrompt;
    }

    public void OpenMessagePrompt(string message)
    {
        GameManager.Instance.ChangeGameState(Enums.GameState.messagePrompt);
        messageBoard.SetActive(true);
        messageText.SetText(message);
        Invoke(nameof(CloseCooldown), 1.0f);
    }

    private void CloseCooldown()
    {
        canBeClosed = true;
        closePrompt.SetActive(true);
    }

    private void CloseMessagePrompt()
    {
        if (GameManager.Instance.CurrentGameState() != Enums.GameState.messagePrompt) return;
        if (canBeClosed == false) return;
        GameManager.Instance.ChangeGameState(Enums.GameState.exploration);
        messageBoard.SetActive(false);
        messageText.SetText(string.Empty);
        canBeClosed = false;
        closePrompt.SetActive(false);
    }
}
