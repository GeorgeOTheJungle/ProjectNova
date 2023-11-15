using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [SerializeField] private int currentXP;
    [SerializeField] private int totalXPStored;
    [SerializeField] private List<Skill> allPlayerSkills;

    private void Awake()
    {
        Instance = this;
    }

    public void GetXP(int total)
    {
        currentXP += total;
        totalXPStored += total;
    }

    public List<Skill> GetAvaliableSkills()
    {
        List<Skill> package = new List<Skill>();

        foreach(Skill skill in allPlayerSkills)
        {
            if(skill.unlocked == true) package.Add(skill);
        }

        return package;
    }


}
