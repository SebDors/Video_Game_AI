using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Heals a target GameObject with a Health component.")]
[TaskCategory("MyTasks")]
public class HealTarget : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The target to heal.")]
    public SharedGameObject target;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The amount of health to restore.")]
    public SharedFloat healAmount = 10f;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }

        var healthComponent = target.Value.GetComponent<Health>();
        if (healthComponent == null)
        {
            // This shouldn't happen if FindWeakestAlly is used correctly.
            return TaskStatus.Failure;
        }

        // Stop healing if the target is at full health.
        if (healthComponent.HealthPercentage >= 1.0f)
        {
            return TaskStatus.Success;
        }

        healthComponent.Heal(healAmount.Value);

        // Return success to indicate the heal was applied.
        return TaskStatus.Success;
    }
}
