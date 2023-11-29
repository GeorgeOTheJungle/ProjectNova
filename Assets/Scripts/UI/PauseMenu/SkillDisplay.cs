using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDisplay : MonoBehaviour
{
    [SerializeField] private PlayerSkill m_Skill;
    [SerializeField] private int m_id;
    [SerializeField] private Image iconImage;
    private SkillUpgradeUI skillUpgrade;

    private void Awake()
    {
        
        skillUpgrade = GetComponentInParent<SkillUpgradeUI>();
    }

    public void SetDisplay(PlayerSkill skill,int id)
    {
        m_Skill = skill;
        m_id = id;

        iconImage.sprite = m_Skill.icon;
    }

    public void SelectSkill()
    {
        skillUpgrade.OnSkillSelected(m_Skill, m_id);
    }
}
