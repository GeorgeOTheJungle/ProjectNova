using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDisplay : MonoBehaviour
{
    [SerializeField] private PlayerSkill m_Skill;
    [SerializeField] private int m_id;
    private SkillUpgradeUI skillUpgrade;

    private void Awake()
    {
        skillUpgrade = GetComponentInParent<SkillUpgradeUI>();
    }

    public void SetDisplay(PlayerSkill skill,int id)
    {
        m_Skill = skill;
        m_id = id;
    }

    public void SelectSkill()
    {
        skillUpgrade.OnSkillSelected(m_Skill, m_id);
    }
}
