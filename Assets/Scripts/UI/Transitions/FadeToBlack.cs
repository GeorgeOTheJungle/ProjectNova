using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

public class FadeToBlack : Transition
{
    [SerializeField] private RectTransform blackSquare;


    public override void StartTransition()
    {
        
        GameManager.Instance.ChangeGameState(Enums.GameState.explorationTransition);
  
        LeanTween.alpha(blackSquare,1.0f, Constants.FADE_TIME);
 
        float waitTime = Constants.FADE_TIME + Constants.WAIT_TIME;
        Invoke(nameof(RestartState), waitTime);
        LeanTween.alpha(blackSquare, 0.0f, Constants.FADE_TIME).setDelay(waitTime);
        
    }


}
