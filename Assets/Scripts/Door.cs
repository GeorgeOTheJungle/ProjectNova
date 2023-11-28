using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour,IInteractable
{
    [SerializeField] private bool teleportToNextStage;
    [SerializeField] private int nextFloor;
    [SerializeField] private bool isOpen;

    private SpriteRenderer doorSpriteRenderer;

    [SerializeField] private GameObject[] doorGameObjects;

    private void Awake()
    {
        doorSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
       // doorSpriteRenderer.sprite = isOpen ? openDoor : closedDoor;
       foreach(GameObject go in doorGameObjects)
        {
            go.SetActive(false);   
        }

        doorGameObjects[0].SetActive(true);
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
        if (teleportToNextStage == false) return;
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
       // doorSpriteRenderer.sprite = isOpen ? openDoor : closedDoor;
        StartCoroutine(OpeningAnimation());
    }

    private IEnumerator OpeningAnimation()
    {
        int f = 0;
        int lenght = doorGameObjects.Length - 1;
        while (f < lenght)
        {
            doorGameObjects[f].SetActive(true);
            yield return new WaitForSeconds(.1f);
            doorGameObjects[f].SetActive(false);
            f++;
        }
        if (teleportToNextStage == false) gameObject.SetActive(false); // TODO CHANGE THIS ONCE WE HAVE THE ART
        else doorGameObjects[lenght - 1].SetActive(true);
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
