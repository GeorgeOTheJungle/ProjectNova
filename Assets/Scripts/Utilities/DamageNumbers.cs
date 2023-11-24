using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    private SpriteRenderer icon;
    private TextMeshPro damageText;

    [SerializeField] private Sprite[] icons;

    private void Awake()
    {
        icon = GetComponentInChildren<SpriteRenderer>();
        damageText = GetComponentInChildren<TextMeshPro>();
    }

    public void SetDamage(int damage, bool isCrit, bool isMagic)
    {
        damageText.SetText(damage.ToString());

        int id = 0;
        if (isMagic)
        {
            if (isCrit) id = 3;
            else id = 2;
        }
        else
        {
            if (isCrit) id = 1;
            else id = 0;
        }

        icon.sprite = icons[id];
        Invoke(nameof(SelfDestroy), 1.25f);

        Vector3 position = transform.position;
        position.y += 1.0f;
        LeanTween.move(gameObject, position, 0.5f);
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
