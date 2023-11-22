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

        foreach(Skill skill in allPlayerSkills)
        {
            skill.Initialize();
        }
    }

    public void GetXP(int total)
    {
        currentXP += total;
        totalXPStored += total;

        if(currentXP < 0)currentXP = 0;
        if(totalXPStored < 0)totalXPStored = 0;
    }

    public void ResetXP()
    {
        currentXP = totalXPStored;
        foreach(Skill skill in allPlayerSkills)
        {
            skill.ResetToLevel1();
        }
    }

    public bool SkillCanBeUpgraded(Skill skill)
    {
        if (skill.unlocked == false) return false;
        else if (skill.level > 3) return false; 
        else if (skill.RequiredXp() <= currentXP) return true;
        else return false;
    }

    public bool SkillCanBeUnlocked(Skill skill)
    {
        return skill.initialUnlockCost <= currentXP;
    }

    public bool HaveEnoughXP(int xpNeed)
    {
        return currentXP >= xpNeed;
    }

    public void UpgradeSkill(Skill skillToUpgrade)
    {
        currentXP -= skillToUpgrade.RequiredXp();
        skillToUpgrade.UpdgradeSkill();
    }

    public void UnlockSkill(Skill skillToUnlock)
    {
        currentXP -= skillToUnlock.initialUnlockCost;
        skillToUnlock.unlocked = true;
        skillToUnlock.level = 1;
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

    public List<Skill> GetAllSkills() => allPlayerSkills;

    public string GetCurrentXP() => currentXP.ToString();

    public int GetCurXP() => currentXP;

}
