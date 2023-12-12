using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour,IMechanism
{
    [SerializeField] protected bool doorOpen = false;
    [SerializeField] private GameObject[] visuals;
    private Collider doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
        if (doorCollider == null) Debug.LogWarning($"No collider was found in {gameObject.name}", gameObject);

        for (int i = 0; i < visuals.Length; i++)
        {
            visuals[i].SetActive(false);
        }
        visuals[0].SetActive(true);
    }

    public void SetDoor(bool active)
    {
        doorOpen = !active;
        doorCollider.enabled = doorOpen;
        StartCoroutine(OpeningAnimation());
        // Art call here
    }

    public void ToggleMechanism(bool active)
    {
        SetDoor(active);
    }

    private IEnumerator OpeningAnimation()
    {
        float frameRate = 0.15f;
        for(int i = 0; i < visuals.Length; i++) 
        {
            visuals[i].SetActive(true);
            yield return new WaitForSeconds(frameRate);
            visuals[i].SetActive(false);
        }
        visuals[visuals.Length - 1].SetActive(true);
    }
}
