using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIndicatorUI : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    public void SetVisual(bool active)
    {
        visual.SetActive(active);
    }
}
