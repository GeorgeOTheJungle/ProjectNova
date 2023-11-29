using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
public class CommandUI : MonoBehaviour,ISelectHandler
{
    [SerializeField] private string defaultName;
    [SerializeField] private PlayerSkill assignedAction;

    private PlayerEntity entity;

    private void Awake()
    {
        entity = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerEntity>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (assignedAction && assignedAction.skillName != string.Empty) CombatNavigation.Instance.UpdateActionText(assignedAction.skillName);
        else CombatNavigation.Instance.UpdateActionText(defaultName);
    }

    public void OpenTargetWindow()
    {
        //CombatPlayer.Instance.PerformAction(assignedAction);
        // Close player UI here.
        entity.PreSelectSkill(assignedAction);
        CombatNavigation.Instance.HideAllWindows();
        CombatManager.Instance.OpenTargetWindow(assignedAction.targetingStyle);
    }

    public void DoAction()
    {
        if(assignedAction == null)
        {
            Debug.LogWarning("There is no skill assigned!");
            return;
        }
        //CombatPlayer.Instance.PerformAction(assignedAction);
        entity.PerformAction(assignedAction);
        //CombatManager.Instance.ActivateTargets();
    }

    
}
