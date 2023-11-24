using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorsManager : MonoBehaviour
{
    public static FloorsManager Instance;

    [SerializeField] private GameObject[] floors;
    [SerializeField] private Transform[] floorsSpawnPoints;

    private void Awake()
    {
        Instance = this;
    }
    public void ActivateFloor(int id)
    {
        foreach(GameObject go in floors)
        {
            go.SetActive(false);
        }

        floors[id].SetActive(true);
        // Set active all visuals.
        CharacterMovement.Instance.MovePlayerToSpawn(floorsSpawnPoints[id]);
        // Move player to spawnpoint.

    }
}
