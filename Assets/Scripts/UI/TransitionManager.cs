using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [SerializeField] private GameObject transitionScreen; // TODO MAKE A COOL ANIMATION
    [SerializeField] private CharacterMovement characterMovement;
    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        transitionScreen.SetActive(false);
        yield return new WaitForEndOfFrame();
        CombatManager.Instance.onCombatStart += HandleStartingCombatTransition;
        CombatManager.Instance.onCombatFinish += HandleCombatEndingTransition;
    }

    private void OnEnable()
    {
        if (CombatManager.Instance == null) return;
        CombatManager.Instance.onCombatStart += HandleStartingCombatTransition;
        CombatManager.Instance.onCombatFinish += HandleCombatEndingTransition;
    }

    private void OnDisable()
    {
        CombatManager.Instance.onCombatStart -= HandleStartingCombatTransition;
        CombatManager.Instance.onCombatFinish += HandleCombatEndingTransition;

    }

    public void HandleStartingCombatTransition()
    {
        StartCoroutine(TransitionToCombat());
    }

    public void HandleCombatEndingTransition(CombatResult result, int id)
    {
        if (result == CombatResult.victory) return;
        StartCoroutine(TransitionToExploration(result == CombatResult.defeat));
    }

    //private void HandleDefeatCombatTransition(CombatResult result,int id)
    //{
    //    if (result != CombatResult.defeat) return;
    //    StartCoroutine(EndingCombatTransitionAnimation(2.5f));
    //}

    private IEnumerator TransitionToCombat()
    {
        // This will be replaced later with actual animation
        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        transitionScreen.SetActive(false);
        GameManager.Instance.ChangeGameState(Enums.GameState.combatReady);
    }

    private IEnumerator TransitionToExploration(bool playerDefeat)
    {

        transitionScreen.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        transitionScreen.SetActive(false);
        GameManager.Instance.ChangeGameState(Enums.GameState.exploration);
        if(playerDefeat) characterMovement.HandleRespawn();
    }

    //private IEnumerator CombatTransitionAnimation()
    //{
    //    transitionScreen.SetActive(true);
    //    yield return new WaitForSeconds(2.0f);
    //    transitionScreen.SetActive(false);
    //    GameManager.Instance.ChangeGameState(Enums.GameState.combatReady);
    //}

    //private IEnumerator EndingCombatTransitionAnimation(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    transitionScreen.SetActive(true);
    //    yield return new WaitForSeconds(2.0f);
    //    transitionScreen.SetActive(false);
    //    GameManager.Instance.ChangeGameState(Enums.GameState.exploration);
    //    characterMovement.HandleRespawn();
    //}

    
}
