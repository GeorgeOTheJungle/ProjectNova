using Enums;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ScriptedAnimator : MonoBehaviour
{
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector3 offset;
    [Tooltip("If left as -1 then its not going to do damage")]
    [SerializeField] private int[] eventFrame;
    [SerializeField] private FrameEvent[] frameEvents;
    [SerializeField] private ParticleSystem particles;
    private int damagingFramesLenght;
    private int spritesLenght;
    private SpriteRenderer spriteRenderer;
    protected Entity m_entity;

    private Vector3 originalPosition;
    private void Awake()
    {
        originalPosition = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesLenght = sprites.Length;
        damagingFramesLenght = eventFrame.Length;
        m_entity = GetComponentInParent<Entity>();
    }
    public void PlayAnimation()
    {
        StartCoroutine(AnimationCorutine());
    }

    private IEnumerator AnimationCorutine()
    {
        Vector3 position = transform.position;
        transform.position = position + offset;
        int frame = 0;
        int currentDamagingFrame = 0;
        while (frame < spritesLenght)
        {
            spriteRenderer.sprite = sprites[frame];
            if (damagingFramesLenght >= 0)
            {
                if (frame == eventFrame[currentDamagingFrame])
                {
                    switch (frameEvents[currentDamagingFrame])
                    {
                        case FrameEvent.damagePlayer:
                            Debug.Log("Dealing damage in frame:" + frame);
                            m_entity.AttackEntity();
                            break;
                        case FrameEvent.playParticles:
                            Debug.Log("Player particles in frame:" + frame);
                            if (particles) particles.Play();
                            break;
                        case FrameEvent.stopParticles:
                            if (particles) particles.Stop();
                            break;
                    }
  
                    if (currentDamagingFrame < damagingFramesLenght - 1) currentDamagingFrame++;
                    
                }
            }
            frame++;
            // if (frame == dealDamageOnFrame) m_entity.AttackEntity();
            yield return new WaitForSeconds(frameRate);
        }
        spriteRenderer.sprite = null ;
        if (particles) particles.Stop();
        transform.position = position;
    }
}
