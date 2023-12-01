using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource m_AudioSource;
    
    [SerializeField] private AudioClip[] stageSongs;
    [SerializeField] private AudioClip combatSong;
    [SerializeField] private AudioClip bossSong;
    [SerializeField] private AudioClip victorySong;
    private int currentFloor;
    private void Awake()
    {
        Instance = this;
        m_AudioSource = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        PlaySong(Constants.EXPLORATION_SONG);
    }

    public void ChangeFloorSong(int floor)
    {
        currentFloor = floor;
        StartCoroutine(FadeOutSong(m_AudioSource, Constants.EXPLORATION_SONG));
    }

    public void PlaySong(string songID)
    {
        StartCoroutine(FadeOutSong(m_AudioSource,songID));
    }

    private void ChangeMusic(string song)
    {
        switch (song)
        {
            case Constants.EXPLORATION_SONG:
                m_AudioSource.clip = GetCurrentFloorSong(currentFloor);
                break;
            case Constants.COMBAT_SONG:
                m_AudioSource.clip = combatSong;
                break;
            case Constants.BOSS_SONG:
                m_AudioSource.clip = bossSong;

                break;
            case Constants.VICTORY_SONG:
                m_AudioSource.clip = victorySong;
                break;
        }
        StartCoroutine(FadeInSong(m_AudioSource));
    }

    private IEnumerator FadeOutSong(AudioSource audio, string nextSong)
    {
        float volume = audio.volume;

        while (volume > 0.0f)
        {
            volume -= Time.deltaTime;
            audio.volume = volume;
            yield return new WaitForEndOfFrame();
        }
        audio.Stop();
        ChangeMusic(nextSong);
    }

    private IEnumerator FadeInSong(AudioSource audio)
    {
        float volume = audio.volume;
        audio.volume = 0.0f;
        audio.Play();
        while (volume < 1.0f)
        {
            volume += Time.deltaTime;
            audio.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }

    private AudioClip GetCurrentFloorSong(int id) => stageSongs[id];
}
