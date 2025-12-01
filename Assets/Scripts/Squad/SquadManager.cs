using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

/// <summary>
/// Manages a squad of 3 drones and activates their special triangle formation behavior.
/// </summary>
public class SquadManager : MonoBehaviour
{
    // Assign the 3 drones of the squad in the Unity Inspector.
    public List<GameObject> squadDrones = new List<GameObject>(3);

    // The name of the custom variable on the drones' Behavior Tree to start the behavior.
    public string startBehaviorVariableName = "StartTriangleFormation";

    // The name of the SharedGameObjectList variable on each drone's Behavior Tree that holds all squad members.
    public string allSquadDronesVariableName = "AllSquadDrones";

    // The name of the SharedGameObject variable on each drone's Behavior Tree that will store the leader.
    public string squadLeaderVariableName = "SquadLeader";

    // The name of the SharedInt variable on each drone's Behavior Tree that will store its assigned flank position index.
    public string flankPositionIndexVariableName = "FlankPositionIndex";

    /// <summary>
    /// Call this method to start the squad's special behavior.
    /// </summary>
    public void ActivateTriangleFormation()
    {
        Debug.Log("SquadManager: ActivateTriangleFormation() called.");
        Debug.Log($"SquadManager: Assigning {squadDrones.Count} drones to the squad.");

        if (squadDrones.Count != 3)
        {
            Debug.LogError("SquadManager: Exactly 3 drones must be assigned to the squad.");
            return;
        }

        SharedGameObjectList sharedSquadList = new SharedGameObjectList();
        sharedSquadList.Value = new List<GameObject>(squadDrones);

        // Assign flank indices and other variables
        for (int i = 0; i < squadDrones.Count; i++)
        {
            GameObject drone = squadDrones[i];
            if (drone == null) continue;

            var behaviorTree = drone.GetComponent<BehaviorTree>();
            if (behaviorTree != null)
            {
                behaviorTree.SetVariableValue(startBehaviorVariableName, true);
                behaviorTree.SetVariableValue(allSquadDronesVariableName, sharedSquadList);
                behaviorTree.SetVariableValue(squadLeaderVariableName, (GameObject)null);
                behaviorTree.SetVariableValue(flankPositionIndexVariableName, i); // Assign 0, 1, 2
            }
            else
            {
                Debug.LogWarning($"SquadManager: Drone {drone.name} does not have a BehaviorTree component.");
            }
        }
    }

    // Automatically activate the squad formation when the game starts.
    void Start()
    {
        ActivateTriangleFormation();
    }
}
