using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorBase : MonoBehaviour,IMechanism
{
    [SerializeField] protected bool doorOpen = false;
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject impactVisual;
    private Collider doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
        if (doorCollider == null) Debug.LogWarning($"No collider was found in {gameObject.name}", gameObject);

        visual.SetActive(true);
        impactVisual.SetActive(false);
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
        int totalFrames = 4;
        float initialX = visual.transform.localPosition.x;
        float moveOffset = 0.1f;
        visual.SetActive(false);
        impactVisual.SetActive(true);
        yield return new WaitForSeconds(frameRate);
        visual.SetActive(true);
        impactVisual.SetActive(false);

        int x = 1;
        int curve = 0;
        float offset = initialX - moveOffset;
        for(int i = 0;i < totalFrames; i++)
        {
            LeanTween.moveLocalX(visual, offset, 0.0f);
            yield return new WaitForSeconds(frameRate);
            x++;
            curve += 1;
            offset = initialX - moveOffset * (x + curve); 
        }

        //LeanTween.moveX(visual, initialX - moveOffset, 0.0f);
        //LeanTween.moveX(visual, initialX - (moveOffset * 3), 0.0f).setDelay(frameRate);
        //LeanTween.moveX(visual, initialX - (moveOffset * 6), 0.0f).setDelay(frameRate * 2);
        //LeanTween.moveX(visual, initialX - (moveOffset * 8), 0.0f).setDelay(frameRate * 3);
        //LeanTween.moveX(visual, initialX - (moveOffset * 9), 0.0f).setDelay(frameRate * 4);
        //visual.SetActive(false);
    }
}
