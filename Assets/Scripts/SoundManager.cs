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
    [SerializeField] private AudioClip combatSong;
    [SerializeField] private AudioClip bossSong;
    private int currentFloor;
    private GameState lastGameState;
    private void Awake()
    {
       Instance = this;
       currentFloor = 0;
       currentCombatSong.volume = 0;
        currentStageSong.clip = stageSongs[currentFloor];
        currentStageSong.Play();
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
                StartCoroutine(FadeSongOut(currentStageSong,true));
                break;
            case GameState.combatReady:
                StartCoroutine(FadeSongOut(currentCombatSong, false));
                break;
            case GameState.exploration:
                ReturnToExplorationMusic();
                break;
        }
    }
    // Do fade out
    // Change to x Music.
    // Do victory music.
    // Do fade out.
    // Change to x music.

    private IEnumerator FadeSongOut(AudioSource audio, bool fadeOut)
    {
        float volume = audio.volume;

        if (fadeOut)
        {
            while (volume > 0.0f)
            {
                volume -= Time.deltaTime;
                audio.volume = volume;
                yield return new WaitForEndOfFrame();
            }
            audio.Stop();
        } else
        {
            audio.volume = 0.0f;
            audio.Play();
            while (volume < 1.0f)
            {
                volume += Time.deltaTime;
                audio.volume = volume;
                yield return new WaitForEndOfFrame();
            }

        }


    }

    public void ChangeStageMusic()
    {

    }

    private void ChangeToStateMusic(string song)
    {
        switch(song)
        {
            case Constants.EXPLORATION_SONG:
                currentStageSong.Play();
                break;
            case Constants.COMBAT_SONG:
                break;
            case Constants.BOSS_SONG:
                break;
            case Constants.VICTORY_SONG:
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

    private IEnumerator ChangeToExplorationMusicFade()
    {
        float volume = 0.0f;
        float combatVolume = 1.0f;
        while (volume < 1.0f)
        {
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
