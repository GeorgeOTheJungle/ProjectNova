using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBit : MonoBehaviour
{
    [Header("Ammo Bit: "), Space(10)]
    [SerializeField] private float frameRate = 0.1f;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private Image visual;

    public void SetToStarting()
    {
        visual.sprite = sprites[sprites.Length - 1];
    }

    public void UseAnimation()
    {
        StartCoroutine(Animation(false));
    }

    public void RestoreAnimation()
    {
        StartCoroutine(Animation(true));
    }

    private IEnumerator Animation(bool isReverse)
    {

        if (isReverse)
        {
            visual.sprite = sprites[sprites.Length - 1];
            for (int i = sprites.Length - 1; i > 0; i--)
            {
                visual.sprite = sprites[i];
                yield return new WaitForSeconds(frameRate);
            }
            visual.sprite = sprites[0];
        } else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                visual.sprite = sprites[i];
                yield return new WaitForSeconds(frameRate);
            }
            visual.sprite = sprites[sprites.Length - 1];
        }

    }
}
