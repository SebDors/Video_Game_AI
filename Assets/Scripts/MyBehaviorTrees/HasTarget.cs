using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Checks if the agent has a target that is not null and is still alive.")]
[TaskCategory("MyTasks")]
public class HasTarget : Conditional
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The target to check.")]
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }

        // Check if the target GameObject is still active in the scene.
        if (!target.Value.gameObject.activeInHierarchy)
        {
            // The target might have been destroyed. Clear it so a new one can be found.
            target.Value = null;
            return TaskStatus.Failure;
        }
        
        // Additionally, check health in case the object is not destroyed immediately.
        var healthComponent = target.Value.GetComponentInChildren<Health>();
        if (healthComponent != null && healthComponent.Value <= 0)
        {
            target.Value = null;
            return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}
