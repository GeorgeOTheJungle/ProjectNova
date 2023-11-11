using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyEntity : Entity
{
    [Header("Enemy Specifics: "), Space(10)]
    [SerializeField] private GameObject a;
    public override void AttackEntity()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAwake()
    {
        
    }

    public override void OnCombatStart(GameState gameState)
    {
        currentSkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                Debug.Log("Entity Ready for combat!");
                StartCoroutine(DelayEntrance());
                break;
        }
    }

    public override void OnEntityTurn()
    {
        Debug.Log($"Its my turn: {transform.name}");
    }

    public override void OnStart()
    {
     
    }



    public override void PerformAction(Skill skill)
    {
        throw new System.NotImplementedException();
    }




    protected override void UpdateEntityUI()
    {
        throw new System.NotImplementedException();
    }

    public override void TargetEntity(int entitySlot)
    {

    }

    #region Corutines
    private IEnumerator DelayEntrance()
    {
        yield return new WaitForSeconds(0.25f);
        PlayAnimation(ENTRANCE_ANIMATION);
    }

    #endregion
}
