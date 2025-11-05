using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;

[TaskDescription("Finds the best enemy to attack based on a weighted score of health and distance.")]
[TaskCategory("MyTasks")]
public class FindBestEnemyTarget : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The found enemy will be stored in this variable")]
    public SharedTransform returnedObject;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the enemy's health (lower health = higher priority)")]
    public float healthWeight = 1.0f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the distance to the enemy (closer = higher priority)")]
    public float distanceWeight = 1.0f;

    private ArmyManager armyManager;
    private ArmyElement self;

    public override void OnStart()
    {
        self = GetComponent<ArmyElement>();
        if (self == null)
        {
            Debug.LogWarning("FindBestEnemyTarget: No ArmyElement found on this agent. Task will fail.");
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
                Debug.LogWarning("FindBestEnemyTarget: ArmyManager not found on this agent. Cannot find an enemy.");
                return TaskStatus.Failure;
            }
        }

        var enemies = armyManager.GetAllEnemies(false)
            .Select(enemy => new {
                enemy,
                health = enemy.GetComponentInChildren<Health>()
            })
            .Where(x => x.health != null) // Ensure the enemy has a health component
            .ToList();

        if (enemies.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        ArmyElement bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var potentialTarget in enemies)
        {
            float healthPercentage = potentialTarget.health.HealthPercentage;
            float distance = Vector3.Distance(self.transform.position, potentialTarget.enemy.transform.position);

            // Score for health (0 to 1, higher is better for lower health)
            float healthScore = 1.0f - healthPercentage;

            // Score for distance (higher is better for closer distance)
            // Avoid division by zero
            float distanceScore = (distance > 0.1f) ? 1.0f / distance : float.MaxValue;

            float finalScore = (healthScore * healthWeight) + (distanceScore * distanceWeight);

            if (finalScore > bestScore)
            {
                bestScore = finalScore;
                bestTarget = potentialTarget.enemy;
            }
        }

        if (bestTarget != null)
        {
            returnedObject.Value = bestTarget.transform;
            return TaskStatus.Success;
        }

        returnedObject.Value = null;
        return TaskStatus.Failure;
    }
}
