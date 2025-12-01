using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

[TaskCategory("Squad")]
[TaskDescription("Destroys all enemies within the triangle formed by the squad members.")]
public class TriangleAttack : Action
{
    [UnityEngine.Tooltip("The list of all drones in the squad.")]
    public SharedGameObjectList allSquadDrones;

    [UnityEngine.Tooltip("The tag of the enemies to target.")]
    public SharedString enemyTag;

    private List<Health> enemiesHealth;

    public override void OnStart()
    {
        // Find all potential targets at the start of the action
        var enemyObjects = GameObject.FindGameObjectsWithTag(enemyTag.Value);
        enemiesHealth = new List<Health>();
        foreach (var enemy in enemyObjects)
        {
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                enemiesHealth.Add(health);
            }
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (allSquadDrones.Value.Count != 3)
        {
            return TaskStatus.Failure;
        }

        // Get the 2D positions (XZ plane) of the three squad drones
        Vector2 p1 = new Vector2(allSquadDrones.Value[0].transform.position.x, allSquadDrones.Value[0].transform.position.z);
        Vector2 p2 = new Vector2(allSquadDrones.Value[1].transform.position.x, allSquadDrones.Value[1].transform.position.z);
        Vector2 p3 = new Vector2(allSquadDrones.Value[2].transform.position.x, allSquadDrones.Value[2].transform.position.z);

        int enemiesDestroyed = 0;

        foreach (var enemyHealth in enemiesHealth)
        {
            if (enemyHealth == null) continue; // Enemy might have been destroyed by other means

            Vector2 enemyPos = new Vector2(enemyHealth.transform.position.x, enemyHealth.transform.position.z);

            if (IsPointInTriangle(enemyPos, p1, p2, p3))
            {
                var drone = enemyHealth.gameObject.GetComponent<Drone>();
                if (drone != null)
                {
                    drone.Die(); // Assuming enemies have a Die() method
                    enemiesDestroyed++;
                }
            }
        }

        if (enemiesDestroyed > 0)
        {
            Debug.Log($"Triangle Attack: Destroyed {enemiesDestroyed} enemies.");
        }

        // After the attack, we can consider the behavior complete for this cycle.
        // You might want to reset the "StartTriangleFormation" variable here.
        foreach (var drone in allSquadDrones.Value)
        {
            if (drone != null)
            {
                var bt = drone.GetComponent<BehaviorTree>();
                bt.SetVariableValue("StartTriangleFormation", false);
            }
        }

        return TaskStatus.Success;
    }

    /// <summary>
    /// Checks if a 2D point is inside a 2D triangle using barycentric coordinates.
    /// </summary>
    private bool IsPointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = sign(pt, v1, v2);
        d2 = sign(pt, v2, v3);
        d3 = sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    private float sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }
}
