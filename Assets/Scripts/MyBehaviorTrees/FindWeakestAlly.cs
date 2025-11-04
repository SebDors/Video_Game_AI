using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskDescription("Finds the best ally to heal based on a weighted score of health and distance.")]
[TaskCategory("MyTasks")]
public class FindWeakestAlly : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The found ally will be stored in this variable")]
    public SharedTransform returnedObject;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the ally's health (lower health = higher priority)")]
    public float healthWeight = 1.0f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the distance to the ally (closer = higher priority)")]
    public float distanceWeight = 1.0f;

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

        if (self.ArmyManager != null)
        {
            armyManager = self.ArmyManager;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (self == null)
        {
            return TaskStatus.Failure;
        }

        if (armyManager == null)
        {
            armyManager = self.ArmyManager;
            if (armyManager == null)
            {
                Debug.LogWarning("FindWeakestAlly: ArmyManager not found on this agent. Cannot find weak ally.");
                return TaskStatus.Failure;
            }
        }

        var alliesToHeal = armyManager.GetAllAllies(false, self)
            .Select(ally => new {
                ally,
                health = ally.GetComponentInChildren<Health>()
            })
            .Where(x => x.health != null && x.health.HealthPercentage < 1.0f)
            .ToList();

        if (alliesToHeal.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        ArmyElement bestAlly = null;
        float bestScore = float.MinValue;

        foreach (var potentialTarget in alliesToHeal)
        {
            float healthPercentage = potentialTarget.health.HealthPercentage;
            float distance = Vector3.Distance(self.transform.position, potentialTarget.ally.transform.position);

            // Score for health (0 to 1, higher is better)
            float healthScore = 1.0f - healthPercentage;

            // Score for distance (higher is better)
            float distanceScore = 0f;
            if (distance > 0.1f)
            {
                distanceScore = 1.0f / distance;
            }

            float finalScore = (healthScore * healthWeight) + (distanceScore * distanceWeight);

            if (finalScore > bestScore)
            {
                bestScore = finalScore;
                bestAlly = potentialTarget.ally;
            }
        }

        if (bestAlly != null)
        {
            returnedObject.Value = bestAlly.transform;
            return TaskStatus.Success;
        }

        returnedObject.Value = null;
        return TaskStatus.Failure;
    }
}