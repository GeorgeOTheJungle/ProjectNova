using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [SerializeField] private GameObject transitionScreen; // TODO MAKE A COOL ANIMATION
    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        transitionScreen.SetActive(false);
        yield return new WaitForEndOfFrame();
        CombatManager.Instance.onCombatStart += CombatTransition;
    }

    private void OnEnable()
    {
        if (CombatManager.Instance == null) return;
        CombatManager.Instance.onCombatStart += CombatTransition;
    }

    private void OnDisable()
    {
        CombatManager.Instance.onCombatStart -= CombatTransition;

    }

    public void CombatTransition()
    {
        StartCoroutine(CombatTransitionAnimation());
    }

    private IEnumerator CombatTransitionAnimation()
    {
        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        transitionScreen.SetActive(false);
        GameManager.Instance.ChangeGameState(Enums.GameState.combatReady);
    }
}
