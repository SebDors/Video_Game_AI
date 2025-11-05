using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;
using System.Collections.Generic;

[TaskDescription("Finds the best enemy Turret to attack based on a weighted score of health and distance.")]
[TaskCategory("MyTasks")]
public class FindBestTurretTarget : Action
{
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The found turret will be stored in this variable")]
    public SharedTransform returnedObject;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the turret's health (lower health = higher priority)")]
    public float healthWeight = 1.0f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Weight for the distance to the turret (closer = higher priority)")]
    public float distanceWeight = 1.0f;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Number of top candidates to consider for random selection. Set to 1 for always choosing the best.")]
    public int topN = 3;

    private ArmyManager armyManager;
    private ArmyElement self;

    public override void OnStart()
    {
        self = GetComponent<ArmyElement>();
        if (self == null)
        {
            Debug.LogWarning("FindBestTurretTarget: No ArmyElement found on this agent. Task will fail.");
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
                Debug.LogWarning("FindBestTurretTarget: ArmyManager not found on this agent. Cannot find an enemy turret.");
                return TaskStatus.Failure;
            }
        }

        // Specifically get enemies of type Turret
        var turrets = armyManager.GetAllEnemiesOfType<Turret>(false)
            .Select(turret => new {
                enemy = turret,
                health = turret.GetComponentInChildren<Health>()
            })
            .Where(x => x.health != null)
            .ToList();

        if (turrets.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        // Calculate scores for all turrets and order them
        var scoredTurrets = turrets.Select(potentialTarget => {
            float healthPercentage = potentialTarget.health.HealthPercentage;
            float distance = Vector3.Distance(self.transform.position, potentialTarget.enemy.transform.position);
            float healthScore = 1.0f - healthPercentage;
            float distanceScore = (distance > 0.1f) ? 1.0f / distance : float.MaxValue;
            float finalScore = (healthScore * healthWeight) + (distanceScore * distanceWeight);
            return new { Target = potentialTarget.enemy, Score = finalScore };
        })
        .OrderByDescending(x => x.Score)
        .ToList();

        // Take the top N candidates
        var topCandidates = scoredTurrets.Take(topN).ToList();

        if (topCandidates.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        // Select a random target from the top candidates
        var selectedCandidate = topCandidates[Random.Range(0, topCandidates.Count)];

        returnedObject.Value = selectedCandidate.Target.transform;
        return TaskStatus.Success;
    }
}
