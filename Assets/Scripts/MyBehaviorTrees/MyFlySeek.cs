using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class MyFlySeek : Action//Movement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedTransform m_Target;
		[Tooltip("The ground layers to raycast in order to compute the offset height above the ground")]
		public LayerMask m_GroundLayers;
		[Tooltip("The max speed of the agent")]
		public float m_TranslationMaxSpeed = 10;
		[Tooltip("The acceleration of the agent")]
		public float m_LinearAcceleration = 10;
		[Tooltip("The angular speed of the agent")]
		public float m_AngularSpeed = 120;

		public float m_ArriveDistance = 0.2f;
		public float m_ArriveAngle = 1;

		float m_TranslationSpeed = 0;
		float m_InitHeightFromGround;


		Rigidbody m_Rigidbody;
		Transform m_Transform;

		public override void OnAwake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Transform = transform;
			m_TranslationSpeed = 0;

			Vector3 posOnTerrain = Vector3.zero;
			Vector3 normalOnTerrain = Vector3.zero;
			if (TerrainManager.Instance.GetVerticallyAlignedPositionOnTerrain(m_Transform.position, ref posOnTerrain, ref normalOnTerrain))
				m_InitHeightFromGround = Vector3.Distance(posOnTerrain, m_Transform.position);
		}

		bool HasArrivedTranslation()
		{
			if (m_Target.Value == null) return false;
			Vector3 vect = Vector3.ProjectOnPlane(m_Target.Value.position - transform.position, Vector3.up);

			return (vect.sqrMagnitude <= m_ArriveDistance * m_ArriveDistance);
		}

		bool HasArrivedRotation()
		{
			// Always return true, as rotation is no longer a factor for task completion.
			return true;
		}

		bool HasArrived()
		{
			return HasArrivedTranslation() && HasArrivedRotation();
		}

		public override TaskStatus OnUpdate()
        {
            if (m_Target.Value == null) return TaskStatus.Failure;

            if (HasArrived()) return TaskStatus.Success;

            return TaskStatus.Running;
        }

		public override void OnFixedUpdate()
		{
			if (m_Target.Value == null) return;

			// --- Rotation --- (Removed as per user request)
            // The drone will no longer rotate to face the target.

			// --- Translation --- (Modified to move towards target XZ and maintain height)
			if (!HasArrivedTranslation())
			{
				m_TranslationSpeed = Mathf.Min(m_TranslationMaxSpeed, m_TranslationSpeed + m_LinearAcceleration * Time.fixedDeltaTime);
				float dist = m_TranslationSpeed * Time.fixedDeltaTime;

				Vector3 currentPosition = m_Rigidbody.position;
				Vector3 targetPosition = m_Target.Value.position;

				// Calculate XZ movement towards target
				Vector3 directionToTargetXZ = Vector3.ProjectOnPlane(targetPosition - currentPosition, Vector3.up).normalized;
				Vector3 newPosition = currentPosition + directionToTargetXZ * dist;

				// Adjust Y position to maintain height above terrain
				Vector3 posOnTerrain = Vector3.zero;
				Vector3 normalOnTerrain = Vector3.zero;
				if (TerrainManager.Instance.GetVerticallyAlignedPositionOnTerrain(newPosition, ref posOnTerrain, ref normalOnTerrain))
				{
					newPosition.y = posOnTerrain.y + m_InitHeightFromGround;
				}
				// If terrain not found, it will just move in XZ plane and keep current Y.

				m_Rigidbody.MovePosition(newPosition);
			}
			else
			{
				m_TranslationSpeed = 0;
			}
		}

		public override void OnReset()
        {
            m_Target = null; 
        }
    }
}
