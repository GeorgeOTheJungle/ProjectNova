using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingUI : MonoBehaviour
{
    [Header("Skills"), Space(10)]
    [SerializeField] private List<Skill> avaliablePlayerSkills = new List<Skill>();
    [Header("References: "), Space(10)]
    [SerializeField] private Image middleSkillDisplay;
    [SerializeField] private Image leftSkillDisplay;
    [SerializeField] private Image rightSkillDisplay;

    private int lastSkill;
    private int currentSkill;
    private int nextSkill;

    private int playerSkillLenght;

    private void Start()
    {
        avaliablePlayerSkills.Clear();
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

        // Calculate sprites
        if (currentSkill < 0) currentSkill = playerSkillLenght;
        else if (currentSkill > playerSkillLenght) currentSkill = 0;

        lastSkill = currentSkill - 1;
        if (lastSkill < 0) lastSkill = playerSkillLenght;

        nextSkill = currentSkill + 1;
        if(nextSkill > playerSkillLenght) nextSkill = 0;

        // Assign sprites
        leftSkillDisplay.sprite = avaliablePlayerSkills[lastSkill].smallIcon;
        middleSkillDisplay.sprite = avaliablePlayerSkills[currentSkill].largeIcon;
        rightSkillDisplay.sprite = avaliablePlayerSkills[nextSkill].smallIcon;
    }
}
