using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    [SerializeField] private int nextFloor;
    [SerializeField] private bool isOpen;

    [SerializeField] private Sprite closedDoor;
    [SerializeField] private Sprite openDoor;

    private SpriteRenderer doorSpriteRenderer;

    private void Awake()
    {
        doorSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        doorSpriteRenderer.sprite = isOpen ? openDoor : closedDoor;
    }
    private void FloorManagerCall()
    {
        FloorsManager.Instance.ActivateFloor(nextFloor);

    }

    public void OnInteraction()
    {
        
        if (!isOpen)
        {
            // Check if player has a key here!
            KeyCheck();
            return;
        }
        if (GameManager.Instance.CurrentGameState() != Enums.GameState.exploration) return;
        Invoke(nameof(FloorManagerCall), Constants.TRANSITION_TIME);
        TransitionManager.Instance.DoTransition(Constants.FADE_TO_BLACK);
    }

    public void KeyCheck()
    {
        // If player has a key, open the door.
        if(PlayerStatsManager.Instance.GetCurrentKeys() > 0)
        {
            PlayerStatsManager.Instance.UseKey();
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
        doorSpriteRenderer.sprite = isOpen ? openDoor : closedDoor;
    }

    public void OnPlayerEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerExit()
    {
        throw new System.NotImplementedException();
    }
}
