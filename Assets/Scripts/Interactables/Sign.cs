using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{
    public void OnInteraction()
    {
        Debug.Log("I was interacted! cool!!!");
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
