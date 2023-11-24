using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    private const string ON_KEY_OBTAINED = "You got a key!";
    public void OnInteraction()
    {
        gameObject.SetActive(false);
        PlayerStatsManager.Instance.GetKey();
        MessageManager.Instance.OpenMessagePrompt(ON_KEY_OBTAINED);
    }

    public void OnPlayerEnter()
    {
    }

    public void OnPlayerExit()
    {

    }
}
