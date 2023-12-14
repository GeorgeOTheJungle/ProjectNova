using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorsManager : MonoBehaviour
{
    public static FloorsManager Instance;

    [SerializeField] private GameObject[] floors;
    [SerializeField] private Transform[] floorsSpawnPoints;
    [SerializeField] private GameObject finalImage;
    private void Awake()
    {
        Instance = this;
    }
    public void ActivateFloor(int id)
    {
        //foreach(GameObject go in floors)
        //{
        //    go.SetActive(false);
        //}
        //if(id >= floorsSpawnPoints.Length)
        //{
        //    finalImage.SetActive(true);
        //    GameManager.Instance.ChangeGameState(Enums.GameState.messagePrompt);
        //    return;
        //}
        //floors[id].SetActive(true);
        SceneManager.LoadSceneAsync(id);
        // Set active all visuals.
       // CharacterMovement.Instance.MovePlayerToSpawn(floorsSpawnPoints[id]);
        // Move player to spawnpoint.

    }
}
