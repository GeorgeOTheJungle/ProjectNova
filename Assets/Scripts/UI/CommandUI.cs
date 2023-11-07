using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
public class CommandUI : MonoBehaviour,ISelectHandler
{
    [SerializeField] private string defaultName;
    [SerializeField] private Skill assignedAction;

    public void OnSelect(BaseEventData eventData)
    {
        if (assignedAction) CombatNavigation.Instance.UpdateActionText(assignedAction.skillName);
        else CombatNavigation.Instance.UpdateActionText(defaultName);


    }

    public void DoAction()
    {
        if(assignedAction == null)
        {
            Debug.LogWarning("There is no skill assigned!");
            return;
        }
        CombatPlayer.Instance.PerformAction(assignedAction);
    }

    
}
