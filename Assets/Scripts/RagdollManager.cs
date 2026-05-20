using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    Rigidbody[] rbs;

    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs) rb.isKinematic = true;
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rbs) rb.isKinematic = false;
    }
}
