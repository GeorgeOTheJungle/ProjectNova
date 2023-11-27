using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public static BossUI Instance { get; private set; }

    [SerializeField] private GameObject visuals;
    [SerializeField] private Image healthBar;

    private float maxHealth;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CloseBossUI();
    }

    public void OpenBossUI()
    {
        visuals.SetActive(true);
    }

    public void CloseBossUI()
    {
        visuals.SetActive(false);
    }

    public void SetBossUI(float _maxHealth)
    {
        maxHealth = _maxHealth;
    }

    public void UpdateBossUI(float currentHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
