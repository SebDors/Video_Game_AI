using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

[TaskCategory("Squad")]
[TaskDescription("The leader waits for all follower drones to reach their designated flank positions.")]
public class WaitForFollowers : Action
{
    [UnityEngine.Tooltip("The list of all drones in the squad.")]
    public SharedGameObjectList allSquadDrones;

    [UnityEngine.Tooltip("The GameObject of the squad leader.")]
    public SharedGameObject squadLeader;

    [UnityEngine.Tooltip("The distance within which a follower is considered to be in position.")]
    public SharedFloat positionTolerance = 1.5f;

    // The flank offsets should match those in MoveToLeaderFlank for consistency
    [UnityEngine.Tooltip("Offset for the first flank position relative to the leader.")]
    public Vector3 flankOffset0 = new Vector3(-5f, 0f, -5f);

    [UnityEngine.Tooltip("Offset for the second flank position relative to the leader.")]
    public Vector3 flankOffset1 = new Vector3(5f, 0f, -5f);

    [UnityEngine.Tooltip("Offset for the third flank position relative to the leader.")]
    public Vector3 flankOffset2 = new Vector3(0f, 0f, -10f);

    public override TaskStatus OnUpdate()
    {
        if (squadLeader.Value == null || allSquadDrones.Value.Count != 3)
        {
            return TaskStatus.Failure;
        }

        // This task should only be run by the leader.
        if (squadLeader.Value != this.gameObject)
        {
            // If a follower somehow runs this task, just succeed immediately.
            return TaskStatus.Success;
        }

        bool allFollowersInPosition = true;

        foreach (var drone in allSquadDrones.Value)
        {
            if (drone == squadLeader.Value) continue; // Skip the leader

            // Find the flank index for this follower drone
            var bt = drone.GetComponent<BehaviorTree>();
            if (bt == null) continue;

            int flankIndex = (bt.GetVariable("FlankPositionIndex") as SharedInt).Value;
            
            Vector3 targetFlankOffset;
            switch (flankIndex)
            {
                case 0: targetFlankOffset = flankOffset0; break;
                case 1: targetFlankOffset = flankOffset1; break;
                case 2: targetFlankOffset = flankOffset2; break;
                default: targetFlankOffset = flankOffset0; break; // Fallback
            }

            // Calculate the follower's target position
            Vector3 targetPosition = squadLeader.Value.transform.position + squadLeader.Value.transform.TransformDirection(targetFlankOffset);

            // Check the distance
            float distanceToTarget = Vector3.Distance(drone.transform.position, targetPosition);

            if (distanceToTarget > positionTolerance.Value)
            {
                allFollowersInPosition = false;
                break; // No need to check others, we have to wait
            }
        }

        if (allFollowersInPosition)
        {
            Debug.Log("Leader: All followers are in position. Proceeding with attack.");
            return TaskStatus.Success;
        }
        else
        {
            Debug.Log("Leader: Waiting for followers to get into position...");
            return TaskStatus.Running;
        }
    }
}
