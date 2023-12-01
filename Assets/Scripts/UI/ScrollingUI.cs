using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingUI : MonoBehaviour
{
    [Header("Skills"), Space(10)]
    [SerializeField] private List<PlayerSkill> avaliablePlayerSkills = new List<PlayerSkill>();
    [Header("References: "), Space(10)]
    [SerializeField] private Image middleSkillDisplay;
    [SerializeField] private Image leftSkillDisplay;
    [SerializeField] private Image rightSkillDisplay;

    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Button skillButton;

    [SerializeField] private GameObject[] unavaliableIcons;

    private PlayerEntity entity;
    private bool canSelectSkill = false;
    private int lastSkill;
    private int currentSkill;
    private int nextSkill;

    private int playerSkillLenght;

    private void Awake()
    {
        entity = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerEntity>();
        avaliablePlayerSkills = new List<PlayerSkill>(0);
    }

    private void Start()
    {
        //avaliablePlayerSkills.Clear();
        avaliablePlayerSkills = SkillManager.Instance.GetAvaliableSkills();
        playerSkillLenght = avaliablePlayerSkills.Count - 1;
        Scroll(0);
    }

    private void OnEnable()
    {
       // avaliablePlayerSkills.Clear();
         avaliablePlayerSkills = SkillManager.Instance.GetAvaliableSkills();
        playerSkillLenght = avaliablePlayerSkills.Count - 1;
        Scroll(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Scroll(1);
        } else if(Input.GetKeyDown(KeyCode.A))
        {
            Scroll(-1);
        }
    }

    private void Scroll(int dir)
    {
        currentSkill += dir;

        foreach(var icon in unavaliableIcons)
        {
            icon.SetActive(false);
        }

        // Calculate sprites
        if (currentSkill < 0) currentSkill = playerSkillLenght;
        else if (currentSkill > playerSkillLenght) currentSkill = 0;

        lastSkill = currentSkill - 1;
        if (lastSkill < 0) lastSkill = playerSkillLenght;

        nextSkill = currentSkill + 1;
        if(nextSkill > playerSkillLenght) nextSkill = 0;


        //Update text and UI
        actionText.SetText(avaliablePlayerSkills[currentSkill].skillName);

        // Assign sprites
        leftSkillDisplay.sprite = avaliablePlayerSkills[lastSkill].icon;
        middleSkillDisplay.sprite = avaliablePlayerSkills[currentSkill].icon;
        rightSkillDisplay.sprite = avaliablePlayerSkills[nextSkill].icon;
        canSelectSkill = true;
        switch (avaliablePlayerSkills[currentSkill].resourceType)
        {
            case Enums.ResourceType.ammo:
                if (entity.HasAmmo(avaliablePlayerSkills[currentSkill].resourceAmount) == false)
                {
                    unavaliableIcons[0].SetActive(true);
                    canSelectSkill = false;
                }
                break;
            case Enums.ResourceType.energy:
                if (entity.HasEnergy(avaliablePlayerSkills[currentSkill].resourceAmount) == false)
                {
                    unavaliableIcons[1].SetActive(true);
                    canSelectSkill = false;
                }
                break;
        }
    }

    public void UseSkill()
    {
        if (canSelectSkill == false) return;

        if (avaliablePlayerSkills[currentSkill].targetingStyle == Enums.TargetingStyle.self)
        {
            entity.PerformAction(avaliablePlayerSkills[currentSkill]);
            CombatNavigation.Instance.HideAllWindows();
        }
        else
        {
            entity.PreSelectSkill(avaliablePlayerSkills[currentSkill]);
            CombatNavigation.Instance.HideAllWindows();
            CombatManager.Instance.OpenTargetWindow(avaliablePlayerSkills[currentSkill].targetingStyle);
        }

        //CombatPlayer.Instance.PerformAction(avaliablePlayerSkills[currentSkill]);
    }
}
