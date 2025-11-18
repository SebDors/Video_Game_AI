using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seeks the target and simultaneously rotates to face it. Succeeds when the agent is within distance and facing the target.")]
    [TaskCategory("MyTasks")]
    public class SeekAndFaceTarget : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedTransform target;
        [Tooltip("The angle difference at which the agent is considered to be facing the target")]
        public SharedFloat arrivalAngle = 5f;

        public override void OnStart()
        {
            base.OnStart();
            m_NavMeshAgent.stoppingDistance = m_ArriveDistance.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
            {
                return TaskStatus.Failure;
            }

            // Update the destination of the NavMeshAgent to handle moving targets.
            SetDestination(target.Value.position);

            // Calculate the direction to the target on the XZ plane.
            Vector3 direction = target.Value.position - transform.position;
            direction.y = 0;

            // Rotate to face the target using the agent's angular speed for smooth turning.
            if (direction.sqrMagnitude > 0.01f)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_NavMeshAgent.angularSpeed * Time.deltaTime);
            }

            // Check for success condition.
            // HasArrived() checks if we are within stopping distance.
            // We also check if we are facing the target.
            float angle = Vector3.Angle(transform.forward, direction);
            if (HasArrived() && angle < arrivalAngle.Value)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            arrivalAngle = 5f;
        }
    }
}
