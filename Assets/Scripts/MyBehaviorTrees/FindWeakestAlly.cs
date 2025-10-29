using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Finds a random weak ally using the ArmyManager.")]
[TaskCategory("My Behavior Trees")]
public class FindWeakestAlly : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The found ally will be stored in this variable")]
    public SharedGameObject returnedObject;

    private ArmyManager armyManager;
    private ArmyElement self;

    public override void OnStart()
    {
        self = GetComponent<ArmyElement>();
        // ArmyManager is assigned at runtime by the manager itself, so we get it from our own ArmyElement component.
        if (self != null && self.ArmyManager != null)
        {
            armyManager = self.ArmyManager;
        }
    }

    public override TaskStatus OnUpdate()
    {
        // If the manager wasn't found on start, try again.
        if (armyManager == null && self != null)
        {
            armyManager = self.ArmyManager;
        }

        if (armyManager == null)
        {
            Debug.LogWarning("ArmyManager not found on this agent. Cannot find weak ally.");
            return TaskStatus.Failure;
        }

        GameObject weakAlly = armyManager.GetRandomWeakAlly(self);

        if (weakAlly != null)
        {
            returnedObject.Value = weakAlly;
            return TaskStatus.Success;
        }

        // No weak ally found
        returnedObject.Value = null;
        return TaskStatus.Failure;
    }
}
