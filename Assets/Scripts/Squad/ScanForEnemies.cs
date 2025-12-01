using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

[TaskCategory("Squad")]
[TaskDescription("Scans for enemies within a specified radius and stores the count.")]
public class ScanForEnemies : Action
{
    [UnityEngine.Tooltip("The radius within which to scan for enemies.")]
    public SharedFloat scanRadius = 10f;

    [UnityEngine.Tooltip("The LayerMask for enemy objects.")]
    public LayerMask enemyLayer;

    [UnityEngine.Tooltip("The shared variable to store the count of enemies found.")]
    public SharedInt enemyCount;

    [UnityEngine.Tooltip("The shared variable to store the closest enemy found (optional).")]
    public SharedGameObject closestEnemy;

    public override TaskStatus OnUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, scanRadius.Value, enemyLayer);
        int currentEnemyCount = hitColliders.Length;
        enemyCount.Value = currentEnemyCount;

        GameObject foundClosestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                foundClosestEnemy = hitCollider.gameObject;
            }
        }
        closestEnemy.Value = foundClosestEnemy;

        Debug.Log($"{gameObject.name} scanned and found {currentEnemyCount} enemies. Closest: {foundClosestEnemy?.name ?? "None"}");

        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        scanRadius = 10f;
        enemyLayer = 0; // Default to nothing, user must set this.
        enemyCount = 0;
        closestEnemy = null;
    }
}
