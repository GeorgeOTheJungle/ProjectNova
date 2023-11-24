using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public void OnInteraction()
    {
        gameObject.SetActive(false);
        PlayerStatsManager.Instance.GetKey();
    }

    public void OnPlayerEnter()
    {
    }

    public void OnPlayerExit()
    {

    }
}
