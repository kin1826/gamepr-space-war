using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public ObjectiveMarkerSystem objectiveSystem;

    public bool nextObjective = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 🚀 objective tiếp theo
        if (nextObjective)
        {
            objectiveSystem.NextObjective();
        }

        // 🚀 hoặc tắt objective
        else
        {
            objectiveSystem.EndObjective();
        }
    }
}