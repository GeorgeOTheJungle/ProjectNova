using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState _gameState;

    public delegate void GameStateChange();

    public GameStateChange onGameStateChangeTrigger;
    // Enemy interaction triggers combat manager
    // Combat manager registers the enemy count of the interacted object (A list of enemies?)
    // On the combat scenario, place player and all the enemies that are in that list. (Maybe scriptables?)
    // On combat start check who will start.
    // Player selects an attack from its UI.
    // 
    private void Awake()
    {
        Instance = this;
        _gameState = GameState.exploration;
    }

    public void EnterCombatState()
    {
        _gameState = GameState.combat;
    }

    public void EnterExplorationState()
    {
        _gameState = GameState.exploration;
    }
}

