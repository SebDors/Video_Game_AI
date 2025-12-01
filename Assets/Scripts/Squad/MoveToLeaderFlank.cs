using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Squad")]
[TaskDescription("Moves the drone to a flanking position relative to the squad leader.")]
public class MoveToLeaderFlank : Action
{
    [UnityEngine.Tooltip("The GameObject of the squad leader.")]
    public SharedGameObject squadLeader;

    [UnityEngine.Tooltip("The assigned flank position index for this drone (0, 1, or 2). This is set by the SquadManager.")]
    public SharedInt flankPositionIndex;

    [UnityEngine.Tooltip("Offset for the first flank position relative to the leader.")]
    public Vector3 flankOffset0 = new Vector3(-5f, 0f, -5f); // Example offset for index 0

    [UnityEngine.Tooltip("Offset for the second flank position relative to the leader.")]
    public Vector3 flankOffset1 = new Vector3(5f, 0f, -5f); // Example offset for index 1

    [UnityEngine.Tooltip("Offset for the third flank position relative to the leader.")]
    public Vector3 flankOffset2 = new Vector3(0f, 0f, -10f); // Example offset for index 2 (can be used for leader if needed, or another follower)

    [UnityEngine.Tooltip("The speed at which the drone moves.")]
    public SharedFloat moveSpeed = 5f;

    [UnityEngine.Tooltip("The stopping distance for the NavMeshAgent.")]
    public SharedFloat stoppingDistance = 1f;

    private NavMeshAgent navMeshAgent;
    private bool hasSetDestination = false;

    public override void OnAwake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("MoveToLeaderFlank: NavMeshAgent component not found on this GameObject.");
        }
    }

    public override void OnStart()
    {
        hasSetDestination = false;
        if (navMeshAgent != null) navMeshAgent.stoppingDistance = stoppingDistance.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (navMeshAgent == null || squadLeader.Value == null)
        {
            return TaskStatus.Failure;
        }

        // This task should ideally be guarded by a conditional in the BT: "Is NOT Leader".
        // However, if it runs on the leader, it will just succeed without moving.
        if (squadLeader.Value == this.gameObject)
        {
            return TaskStatus.Success; // Leader doesn't move to a flank position via this task.
        }

        Vector3 targetFlankOffset;
        switch (flankPositionIndex.Value)
        {
            case 0:
                targetFlankOffset = flankOffset0;
                break;
            case 1:
                targetFlankOffset = flankOffset1;
                break;
            case 2:
                targetFlankOffset = flankOffset2;
                break;
            default:
                Debug.LogWarning($"MoveToLeaderFlank: Invalid flankPositionIndex {flankPositionIndex.Value} for {gameObject.name}. Defaulting to offset 0.");
                targetFlankOffset = flankOffset0;
                break;
        }

        // Calculate the target position relative to the leader's orientation
        Vector3 targetPosition = squadLeader.Value.transform.position + squadLeader.Value.transform.TransformDirection(targetFlankOffset);

        if (!hasSetDestination)
        {
            navMeshAgent.speed = moveSpeed.Value;
            navMeshAgent.SetDestination(targetPosition);
            hasSetDestination = true;
        }

        // Check if the drone has reached its destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                Debug.Log($"{gameObject.name} reached flank position.");
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Running; // Still moving
    }

    public override void OnReset()
    {
        squadLeader = null;
        flankPositionIndex = 0;
        flankOffset0 = new Vector3(-5f, 0f, -5f);
        flankOffset1 = new Vector3(5f, 0f, -5f);
        flankOffset2 = new Vector3(0f, 0f, -10f);
        moveSpeed = 5f;
        stoppingDistance = 1f;
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
        }
    }
}
