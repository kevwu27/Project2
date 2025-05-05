using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public int checkpointIndex;
    private ArrowPointer arrowPointer;

    void Start()
    {
        arrowPointer = FindObjectOfType<ArrowPointer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && arrowPointer != null)
        {
            // Only advance if this is the correct checkpoint in sequence
            if (arrowPointer.currentCheckpoint == checkpointIndex)
            {
                arrowPointer.AdvanceToNextCheckpoint();
            }
        }
    }
}
