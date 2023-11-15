using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
public class CommandUI : MonoBehaviour,ISelectHandler
{
    [SerializeField] private string defaultName;
    [SerializeField] private Skill assignedAction;

    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (assignedAction) CombatNavigation.Instance.UpdateActionText(assignedAction.skillName);
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
