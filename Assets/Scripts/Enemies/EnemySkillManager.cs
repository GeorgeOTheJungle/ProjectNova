using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillManager : MonoBehaviour
{
    [SerializeField] private EnemySkill[] enemySkillList;

    private void Awake()
    {
        enemySkillList = GetComponentsInChildren<EnemySkill>();
    }

    public EnemySkill GetEnemySkill(int entityID)
    {
        EnemySkill enemySkill = null;

        foreach (EnemySkill skill in enemySkillList)
        {
            if(skill.entityID == entityID)
            {
                enemySkill = skill;
                break;
            }
        }
        return enemySkill;
    }
}
