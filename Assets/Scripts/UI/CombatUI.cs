using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject visual;

    [Header("Stats texts references: "), Space(10)]
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject hitFeedbackVisual;
    [SerializeField] private Image energyBar;
    [SerializeField] private GameObject energyHitFeedbackVisual;
    [SerializeField] private AmmoBit[] ammoContainer;

    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Entity player;

    float maxHealth;
    float maxEnergy;
    int ammo;

    private void Awake()
    {
        player = GameObject.Find("PlayerEntity").GetComponent<Entity>();    
    }

    private IEnumerator Start()
    {
        visual.SetActive(false);
        actionText.gameObject.SetActive(false);
        actionText.SetText(string.Empty);

        yield return new WaitForEndOfFrame();
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatUpdate;
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += HandleCombatUpdate;
    }

    private void OnDisable()
    {
        GameManager.Instance.onGameStateChangeTrigger -= HandleCombatUpdate;
    }

    private void HandleCombatUpdate(GameState state)
    {
        switch (state)
        {
            case GameState.combatPreparation:
                visual.SetActive(true);
                actionText.gameObject.SetActive(true);
                // Initialize stats on texts
                maxHealth = player.entityStats.health;
                maxEnergy = player.entityStats.energy;
                ammo = player.entityStats.ammo;
                UpdateCombatStats(false,false);
                break;
            case GameState.combatReady:
                foreach (var item in ammoContainer)
                {
                    item.SetToStarting();
                }

                StartCoroutine(DoAnimatedAmmoFill(0.5f));
                break;
            default:
                visual.SetActive(false);
                actionText.gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator DoAnimatedAmmoFill(float delay)
    {
        yield return new WaitForSeconds(delay);
        for(int i = 0;i < ammoContainer.Length;i++) 
        {
            ammoContainer[i].RestoreAnimation();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateCombatStats(bool healthFeedback,bool energyFeedback)
    {
        if (healthFeedback)
        {
            StartCoroutine(HitFeedback(hitFeedbackVisual));
        } else if(energyFeedback)
        {
            StartCoroutine(HitFeedback(energyHitFeedbackVisual));
        }
        healthBar.fillAmount = player.entityStats.health / maxHealth;
        energyBar.fillAmount = player.entityStats.energy / maxEnergy;
    }

    private IEnumerator HitFeedback(GameObject visual)
    {
        visual.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        visual.SetActive(false);
    }

    public void OnShot()
    {
        ammo--;
        Invoke(nameof(DelayedAnimation), 0.5f);
    }

    public void OnReload()
    {
        ammo = player.entityStats.ammo;
        StartCoroutine(DoAnimatedAmmoFill(0.1f));
    }

    private void DelayedAnimation()
    {
        ammoContainer[ammo].UseAnimation();
    }
}
