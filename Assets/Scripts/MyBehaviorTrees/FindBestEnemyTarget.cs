using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Linq;
using System.Collections.Generic;

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

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Number of top candidates to consider for random selection. Set to 1 for always choosing the best.")]
    public int topN = 3;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Enable to draw debug lines to the top N targets.")]
    public bool debugLines = false;

    private ArmyManager armyManager;
    private ArmyElement self;

    // --- Variables for Gizmo drawing ---
    private List<Transform> topCandidateTransforms;
    private Transform chosenTargetTransform;

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
        topCandidateTransforms = new List<Transform>();
    }

    public override TaskStatus OnUpdate()
    {
        // Clear previous debug info
        topCandidateTransforms.Clear();
        chosenTargetTransform = null;

        if (self == null) return TaskStatus.Failure;

        if (armyManager == null)
        {
            armyManager = self.ArmyManager;
            if (armyManager == null) return TaskStatus.Failure;
        }

        var enemies = armyManager.GetAllEnemies(false)
            .Select(enemy => new { enemy, health = enemy.GetComponentInChildren<Health>() })
            .Where(x => x.health != null)
            .ToList();

        if (enemies.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        var scoredEnemies = enemies.Select(potentialTarget => {
            float healthPercentage = potentialTarget.health.HealthPercentage;
            float distance = Vector3.Distance(self.transform.position, potentialTarget.enemy.transform.position);
            float healthScore = 1.0f - healthPercentage;
            float distanceScore = (distance > 0.1f) ? 1.0f / distance : float.MaxValue;
            float finalScore = (healthScore * healthWeight) + (distanceScore * distanceWeight);
            return new { Target = potentialTarget.enemy, Score = finalScore };
        })
        .OrderByDescending(x => x.Score)
        .ToList();

        var topCandidates = scoredEnemies.Take(topN).ToList();

        if (topCandidates.Count == 0)
        {
            returnedObject.Value = null;
            return TaskStatus.Failure;
        }

        var selectedCandidate = topCandidates[Random.Range(0, topCandidates.Count)];
        returnedObject.Value = selectedCandidate.Target.transform;

        // Store info for Gizmos
        if (debugLines)
        {
            chosenTargetTransform = selectedCandidate.Target.transform;
            topCandidateTransforms = topCandidates.Select(c => c.Target.transform).ToList();
        }

        return TaskStatus.Success;
    }

    public override void OnDrawGizmos()
    {
        if (!debugLines || topCandidateTransforms == null || self == null)
        {
            return;
        }

        foreach (var candidateTransform in topCandidateTransforms)
        {
            if (candidateTransform == null) continue;

            if (candidateTransform == chosenTargetTransform)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(self.transform.position, candidateTransform.position);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(self.transform.position, candidateTransform.position);
            }
        }
    }
}