using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Heals a target GameObject with a Health component to full health.")]
[TaskCategory("MyTasks")]
public class HealTarget : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The target to heal.")]
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }

        var healthComponent = target.Value.GetComponentInChildren<Health>();
        if (healthComponent == null)
        {
            return TaskStatus.Failure;
        }

        // Stop healing if the target is at full health.
        if (healthComponent.HealthPercentage >= 1.0f)
        {
            return TaskStatus.Success;
        }

        healthComponent.HealToFull();

        // Return success to indicate the heal was applied.
        return TaskStatus.Success;
    }
}
