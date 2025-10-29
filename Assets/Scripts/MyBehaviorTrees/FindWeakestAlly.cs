using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Finds a random weak ally using the ArmyManager.")]
[TaskCategory("MyTasks")]
public class FindWeakestAlly : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The found ally will be stored in this variable")]
    public SharedTransform returnedObject;

    private ArmyManager armyManager;
    private ArmyElement self;

    public override void OnStart()
    {
        self = GetComponent<ArmyElement>();
        if (self == null)
        {
            Debug.LogWarning("FindWeakestAlly: No ArmyElement found on this agent. Task will fail.");
            return;
        }
        // ArmyManager is assigned at runtime by the manager itself, so we get it from our own ArmyElement component.
        if (self.ArmyManager != null)
        {
            armyManager = self.ArmyManager;
        }

        // // Add a null check for returnedObject here
        // if (returnedObject == null)
        // {
        //     Debug.LogWarning("FindWeakestAlly: returnedObject is null in OnStart. Attempting to initialize.");
        //     returnedObject = new SharedGameObject(); // Initialize it to prevent NRE later
        // }
    }

    public override TaskStatus OnUpdate()
    {
        if (self == null)
        {
            Debug.LogWarning("FindWeakestAlly: ArmyElement is null. Task cannot proceed.");
            return TaskStatus.Failure;
        }

        // If the manager wasn't found on start, try again.
        if (armyManager == null)
        {
            armyManager = self.ArmyManager;
        }

        if (armyManager == null)
        {
            Debug.LogWarning("FindWeakestAlly: ArmyManager not found on this agent. Cannot find weak ally.");
            return TaskStatus.Failure;
        }

        if (returnedObject == null)
        {
            Debug.LogWarning("FindWeakestAlly: returnedObject is not assigned in the Behavior Designer editor. Task will fail.");
            return TaskStatus.Failure;
        }

        GameObject weakAlly = armyManager.GetRandomWeakAlly(self);

        if (weakAlly != null)
        {
            returnedObject.Value = weakAlly.transform;
            return TaskStatus.Success;
        }

        // No weak ally found
        returnedObject.Value = null;
        return TaskStatus.Failure;
    }
}
