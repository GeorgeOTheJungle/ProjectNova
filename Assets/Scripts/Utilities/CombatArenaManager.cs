using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArenaManager : MonoBehaviour
{
    public static CombatArenaManager Instance;
    [Header("Avaliable Arenas: "), Space(10)]
    [SerializeField] private GameObject[] arenas;

    private void Awake()
    {
        Instance = this;
    }

    public void SetArena(int id)
    {
        foreach (var arena in arenas)
        {
            arena.SetActive(false);
        }

        arenas[id].SetActive(true);
    }
}
