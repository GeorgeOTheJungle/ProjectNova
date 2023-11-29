using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource currentStageSong;
    [SerializeField] private AudioSource currentCombatSong;

    [SerializeField] private AudioClip[] stageSongs;
    [SerializeField] private AudioClip[] combatSongs;
    private int currentFloor;
    private void Awake()
    {
        Instance = this;
       currentFloor = 0;
        currentCombatSong.volume = 0;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += HandleMusicMix;
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= HandleMusicMix;
    }

    private void HandleMusicMix(GameState gameState)
    {
        if (gameState == GameState.messagePrompt) return;
        switch(gameState)
        {
            case GameState.combatPreparation:
                ChangeToCombatMusic();
                break;
            case GameState.exploration:
                Debug.Log("Reached exploration");
                ReturnToExplorationMusic();
                break;
        }
    }

    public void ChangeFloorMusic(int id)
    {
        // Fade and then change
        currentFloor = id;
        StartCoroutine(ChangeFloorSongFade(id));
    }

    public void ChangeToCombatMusic()
    {
        StartCoroutine(ChangeToCombatMusicFade());
    }

    public void ReturnToExplorationMusic()
    {
        StartCoroutine(ChangeToExplorationMusicFade());
    }

    private IEnumerator ChangeToCombatMusicFade()
    {
        float volume = currentStageSong.volume;
        float combatVolume = currentCombatSong.volume;
        while (volume > 0)
        {
            volume -= Time.deltaTime;
            combatVolume += Time.deltaTime;
            currentStageSong.volume = volume;
            currentCombatSong.volume = combatVolume;
            yield return null;
        }
        currentStageSong.volume = 0f;
        currentCombatSong.volume = 1.0f;

    }

    private IEnumerator ChangeToExplorationMusicFade()
    {
        Debug.Log("Starting Corutine");
        float volume = 0.0f;
        float combatVolume = 1.0f;
        while (volume < 1.0f)
        {
            Debug.Log("doing fade to exploration");
            volume += Time.deltaTime;
            combatVolume -= Time.deltaTime;
            currentStageSong.volume = volume;
            currentCombatSong.volume = combatVolume;
            yield return null;
        }
        currentStageSong.volume = 1.0f;
        currentCombatSong.volume = 0.0f;

    }

    private IEnumerator ChangeFloorSongFade(int id)
    {
        float volume = currentStageSong.volume;

        while (volume > 0.0f)
        {
            volume -= Time.deltaTime;
            currentStageSong.volume = volume;
            yield return null;
        }
        currentStageSong.Stop();
        currentStageSong.clip = stageSongs[id];
        currentCombatSong.clip = combatSongs[id];
        yield return new WaitForSeconds(0.2f);
        currentStageSong.Play();
        while (volume < 1.0f)
        {
            volume += Time.deltaTime;
            currentStageSong.volume = volume;
            yield return null;
        }
    }

}
