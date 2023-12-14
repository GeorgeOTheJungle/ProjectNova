using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MechanismButton : MonoBehaviour
{
    [SerializeField] private bool canMultipleToggle = true;
    private bool isPressed = false;
    [SerializeField] private GameObject linkedMechanism;
    [Space(10)]
    [SerializeField] private GameObject[] pressAnimation;
    [SerializeField] private GameObject notPressedVisual;
    [SerializeField] private GameObject pressedVisual;
    private IMechanism mechanism;

    private Collider m_collider;

    private void OnDrawGizmosSelected()
    {
        if (linkedMechanism == null) return;

        Gizmos.DrawWireCube(transform.position,Vector3.one);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, linkedMechanism.transform.position);

        Gizmos.color = Color.green;
        Vector3 target = linkedMechanism.transform.position;
        target += new Vector3(0.5f,0.5f,0.5f);
        Gizmos.DrawWireCube(target, Vector3.one);
    }

    private void Awake()
    {
        if(linkedMechanism == null)
        {
            Debug.LogWarning("No mechanism linked to this button!", gameObject);
            return;
        }

        mechanism = linkedMechanism.GetComponent<IMechanism>();
        m_collider = GetComponent<Collider>();
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = !isPressed;
            mechanism.ToggleMechanism(isPressed);
            //notPressedVisual.SetActive(!isPressed);
            //pressedVisual.SetActive(isPressed);
            StartCoroutine(PressAnimation());
            if (canMultipleToggle == false) m_collider.enabled = false;
        }
    }

    private IEnumerator PressAnimation()
    {
        float frameRate = 0.1f;
        for(int i = 0;i < pressAnimation.Length;i++)
        {
            pressAnimation[i].SetActive(true);
            yield return new WaitForSeconds(frameRate);
            pressAnimation[i].SetActive(false);
        }
        pressAnimation[pressAnimation.Length - 1].SetActive(true);
    }
}
