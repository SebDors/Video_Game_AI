using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Utility")] // Nous allons la mettre dans une catégorie "Utility"
[TaskDescription("Returns Success immediately. Useful as a leaf node to ensure a branch succeeds.")]
public class AlwaysSuccess : Action
{
    public override TaskStatus OnUpdate()
    {
         return TaskStatus.Success;
    }
}