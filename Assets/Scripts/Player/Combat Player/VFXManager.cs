using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] vfxParticles;


    public void PlayVFX(int id)
    {
        vfxParticles[id].Play();
        // Call sound here!
    }
}
