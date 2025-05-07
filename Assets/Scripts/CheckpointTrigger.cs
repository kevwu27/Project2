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
        Debug.Log("Entered: " + checkpointIndex + "");
        if (other.CompareTag("Player") && arrowPointer != null)
        {
            Debug.Log("Current Checkpoint: " + arrowPointer.currentCheckpoint + "");
            // Only advance if this is the correct checkpoint in sequence
            if (arrowPointer.currentCheckpoint <= checkpointIndex)
            {   
                
                arrowPointer.AdvanceToNextCheckpoint();
            }
        }
    }
}
