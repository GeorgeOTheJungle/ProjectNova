using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource currentStageSong;
    [SerializeField] private AudioSource currentCombatSong;
    [SerializeField] private AudioSource victoryMusic;
    [SerializeField] private AudioClip[] stageSongs;
    [SerializeField] private AudioClip[] combatSongs;
    private int currentFloor;
    private GameState lastGameState;
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
        CombatManager.Instance.onCombatFinish += HandleVictoryMusic;
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= HandleMusicMix;
        CombatManager.Instance.onCombatFinish -= HandleVictoryMusic;
    }

    private void HandleMusicMix(GameState gameState)
    {
        if (gameState == GameState.messagePrompt) return;
        if (gameState == GameState.paused) return;
        if (gameState == GameState.explorationTransition) return;

        if (lastGameState == gameState) return;
        lastGameState = gameState;
        switch (gameState)
        {
            case GameState.combatPreparation:
                ChangeToCombatMusic();
                break;
            case GameState.exploration:
                ReturnToExplorationMusic();
                break;
        }
    }

    private void HandleVictoryMusic(CombatResult restult, int id)
    {
        if (restult == CombatResult.victory) ChangeToVictoryMusic();
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

    private void ChangeToVictoryMusic()
    {
        Debug.Log("Victory song");
        currentCombatSong.Stop();
        victoryMusic.Play();
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
           // currentCombatSong.volume = combatVolume;
            yield return null;
        }
        currentStageSong.volume = 0f;
        currentStageSong.Pause();
        currentCombatSong.volume = 1.0f;
        currentCombatSong.Play();

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
            victoryMusic.volume = combatVolume;
            yield return null;
        }
        currentStageSong.volume = 1.0f;
        currentCombatSong.volume = 0.0f;
        currentStageSong.Play();
        victoryMusic.Stop();
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
        if (id <= stageSongs.Length)
        {
            currentStageSong.clip = stageSongs[id];
            //currentCombatSong.clip = combatSongs[id];

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

}
