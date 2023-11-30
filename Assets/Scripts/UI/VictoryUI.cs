using Enums;
using System.Collections;
using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [Header("Victory Window: "),Space(10)]
    [SerializeField] private GameObject victoryWindow;
    [SerializeField] private GameObject xpText;
    [SerializeField] private TextMeshProUGUI xpGainedText;
    [SerializeField] private GameObject pressButtonPrompt;

    private bool promptDisplayed = false;
    private IEnumerator Start()
    {
        promptDisplayed = false;
        UIReset();
        yield return new WaitForEndOfFrame();
        CombatManager.Instance.onCombatFinish += HandleCombatVictoryResult;
       //CombatManager.Instance.onCombatCleanup += HandleEndTransition;
        InputComponent.Instance.interactTrigger += HandleEndTransition;

    }

    private void OnEnable()
    {
        if(CombatManager.Instance) CombatManager.Instance.onCombatFinish += HandleCombatVictoryResult;
        if (InputComponent.Instance) InputComponent.Instance.interactTrigger += HandleEndTransition;
    }

    private void OnDisable()
    {
        if (CombatManager.Instance) CombatManager.Instance.onCombatFinish -= HandleCombatVictoryResult;
        if (InputComponent.Instance) InputComponent.Instance.interactTrigger -= HandleEndTransition;
    }

    private void HandleCombatVictoryResult(CombatResult result,int id)
    {
        if (result != CombatResult.victory) return;
        StartCoroutine(VictoryWindowAnimation());
    }

    private void HandleEndTransition()
    {
        if (!promptDisplayed) return;
        promptDisplayed = false;
        CombatManager.Instance.VictoryEnd();
        Invoke(nameof(UIReset), 1.0f);
        // Reset things here after cleanup
    }

    private void UIReset()
    {
        victoryWindow.SetActive(false);
        xpGainedText.gameObject.SetActive(false);
        pressButtonPrompt.SetActive(false);
    }

    private IEnumerator VictoryWindowAnimation()
    {
        victoryWindow.SetActive(true);
        int xpGained = CombatManager.Instance.GetXPStored();
        int xpShow = 0;
        yield return new WaitForSeconds(0.25f);
        xpText.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        xpGainedText.gameObject.SetActive(true);
        while (xpShow < xpGained)
        {
            xpShow += Mathf.CeilToInt(Time.deltaTime * 1.0f);
            xpGainedText.SetText(xpShow.ToString());
            yield return new WaitForEndOfFrame();
        }
        xpGainedText.SetText(xpGained.ToString());
        yield return new WaitForSeconds(1.0f);
        pressButtonPrompt.SetActive(true);
        promptDisplayed = true;
    }
}
