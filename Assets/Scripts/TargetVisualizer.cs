using UnityEngine;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
[RequireComponent(typeof(LineRenderer))]
public class TargetVisualizer : MonoBehaviour
{
    private BehaviorTree behaviorTree;
    private LineRenderer lineRenderer;
    private SharedTransform target;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        behaviorTree = GetComponent<BehaviorTree>();

        // Find the "Target" variable in the Behavior Tree
        target = behaviorTree.GetVariable("Target") as SharedTransform;

        if (target == null)
        {
            Debug.LogWarning("TargetVisualizer: Could not find SharedTransform variable named 'Target' in the Behavior Tree.", this);
            enabled = false;
            return;
        }

        // Configure the LineRenderer
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (target != null && target.Value != null)
        {
            // If there is a target, draw the line
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.Value.position);
        }
        else
        {
            // If there is no target, disable the line
            lineRenderer.enabled = false;
        }
    }
}
