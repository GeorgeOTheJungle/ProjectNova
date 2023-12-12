using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioLibrary;
    [SerializeField] private AudioSource[] hitSounds;

    [SerializeField] private AudioSource deathSound;
    public void PlayAudio(int id)
    {
        audioLibrary[id].Play();
    }

    public void PlayHitSound()
    {
        int r = Random.Range(0, hitSounds.Length);
        hitSounds[r].Play();
    }

    public void PlayDeathSound() => deathSound.Play();
}
