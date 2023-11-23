using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ScriptedAnimator : MonoBehaviour
{
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private Sprite[] sprites;

    [Tooltip("If left as -1 then its not going to do damage")]
    [SerializeField] private int dealDamageOnFrame = -1;

    private int spritesLenght;
    private SpriteRenderer spriteRenderer;
    protected Entity m_entity;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesLenght = sprites.Length;
        m_entity = GetComponentInParent<Entity>();
    }
    public void PlayAnimation()
    {
        StartCoroutine(AnimationCorutine());
    }

    private IEnumerator AnimationCorutine()
    {
        int frame = 0;
        while (frame < spritesLenght)
        {
            spriteRenderer.sprite = sprites[frame];
            frame++;
            if (frame == dealDamageOnFrame) m_entity.AttackEntity();
            yield return new WaitForSeconds(frameRate);
        }
        spriteRenderer.sprite = null ;
    }
}
