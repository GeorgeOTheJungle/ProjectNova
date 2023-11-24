using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    protected void RestartState()
    {
        GameManager.Instance.ChangeGameState(Enums.GameState.exploration);
    }
    public abstract void StartTransition();

}
