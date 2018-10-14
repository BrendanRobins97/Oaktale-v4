using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Decisions/WaitForTime")]
public class WaitForTime : Decision {

    [SerializeField] private float time = 1f;
    [SerializeField] AnimationClip[] animationsToWaitFor;

    private float totalTime;

    public override bool Decide(Enemy controller)
    {
        totalTime = time;
        foreach (AnimationClip clip in animationsToWaitFor)
        {
            totalTime += clip.length;
        }
        Debug.Log(totalTime);
        Debug.Log(controller.CheckIfCountDownElapsed(totalTime));

        return !controller.CheckIfCountDownElapsed(totalTime);
    }
}
