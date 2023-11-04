using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public void OnInteraction();

    public void OnPlayerEnter();

    public void OnPlayerExit();
}
