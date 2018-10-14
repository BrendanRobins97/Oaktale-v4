using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : ScriptableObject
{
    public virtual void Act(Enemy controller) { }
    public virtual void InitialAction(Enemy controller) { }
}