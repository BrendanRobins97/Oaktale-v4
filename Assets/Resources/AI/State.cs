using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI/State")]
public class State : ScriptableObject 
{
    public Action[] actions;
    public Transition[] transitions;

    public void OnEnterState(Enemy controller)
    {
        DoInitialActions(controller);
    }

    public void UpdateState(Enemy controller)
    {
        DoActions (controller);
        CheckTransitions (controller);
    }

    private void DoActions(Enemy controller)
    {
        for (int i = 0; i < actions.Length; i++) {
            actions [i].Act (controller);
        }
    }

    private void DoInitialActions(Enemy controller)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].InitialAction(controller);
        }
    }

    private void CheckTransitions(Enemy controller)
    {
        for (int i = 0; i < transitions.Length; i++) 
        {
            bool decisionSucceeded = transitions[i].decision.Decide (controller);
            if (decisionSucceeded) {
                controller.TransitionToState (transitions[i].trueState);
            } else 
            {
                controller.TransitionToState (transitions[i].falseState);
            }
        }
    }


}