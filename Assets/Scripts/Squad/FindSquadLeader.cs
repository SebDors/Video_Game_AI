using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

[TaskCategory("Squad")]
[TaskDescription("Determines which drone in the squad has found the most enemies and sets it as the leader.")]
public class FindSquadLeader : Action
{
    [UnityEngine.Tooltip("The list of all drones in the squad (including this one).")]
    public SharedGameObjectList allSquadDrones;

    [UnityEngine.Tooltip("The name of the SharedInt variable on each drone's Behavior Tree that holds its enemy count.")]
    public SharedString enemyCountVariableName = "MyEnemyCount";

    [UnityEngine.Tooltip("The shared variable to store the GameObject of the identified squad leader.")]
    public SharedGameObject squadLeader;

    public override TaskStatus OnUpdate()
    {
        if (allSquadDrones == null || allSquadDrones.Value.Count == 0)
        {
            Debug.LogError("FindSquadLeader: allSquadDrones list is not set or is empty.");
            return TaskStatus.Failure;
        }

        GameObject currentLeader = null;
        int maxEnemies = -1;

        // Iterate through all drones in the squad to find the one with the most enemies.
        foreach (GameObject drone in allSquadDrones.Value)
        {
            if (drone == null) continue;

            BehaviorTree droneBT = drone.GetComponent<BehaviorTree>();
            if (droneBT == null)
            {
                Debug.LogWarning($"FindSquadLeader: Drone {drone.name} does not have a BehaviorTree component.");
                continue;
            }

            // Safely get the SharedInt variable from the drone's Behavior Tree.
            SharedInt droneEnemyCount = droneBT.GetVariable(enemyCountVariableName.Value) as SharedInt;
            if (droneEnemyCount == null)
            {
                Debug.LogWarning($"FindSquadLeader: Drone {drone.name} does not have a SharedInt variable named {enemyCountVariableName.Value}. Make sure it's set in the ScanForEnemies task.");
                continue;
            }

            if (droneEnemyCount.Value > maxEnemies)
            {
                maxEnemies = droneEnemyCount.Value;
                currentLeader = drone;
            }
            // If tied, the first one encountered remains the leader (arbitrary but consistent).
        }

        if (currentLeader != null)
        {
            squadLeader.Value = currentLeader; // Set the leader for this drone's BT

            // Also, set the leader variable on ALL squad drones' Behavior Trees
            foreach (GameObject drone in allSquadDrones.Value)
            {
                if (drone == null) continue;

                BehaviorTree droneBT = drone.GetComponent<BehaviorTree>();
                if (droneBT != null)
                {
                    droneBT.SetVariableValue(squadLeader.Name, squadLeader.Value); // Use the name of the shared variable from this task
                }
            }

            Debug.Log($"Squad Leader identified: {currentLeader.name} with {maxEnemies} enemies.");
            return TaskStatus.Success;
        }
        else
        {
            Debug.LogWarning("FindSquadLeader: Could not identify a squad leader (perhaps no drones or no enemies found by any drone).");
            return TaskStatus.Failure;
        }
    }

    public override void OnReset()
    {
        // Reset values for editor consistency.
        allSquadDrones = null;
        enemyCountVariableName = "MyEnemyCount";
        squadLeader = null;
    }
}
