using UnityEngine;
using TMPro; // Make sure to include this
using System.Collections.Generic;

public class CheckpointSpawner : MonoBehaviour
{
    public TextAsset file;
    public GameObject checkpointPrefab;
    public GameObject xrOrigin;
    public float scaleFactor = 1.0f / 39.37f;
    public float spawnOffset = 0.5f;
    public ArrowPointer arrowPointer;
    void Start()
    {
        List<Vector3> positions = ParseFile();
        List<Transform> checkpointTransforms = new List<Transform>();
        for (int i = 0; i < positions.Count; i++)
        {
            GameObject checkpoint = Instantiate(checkpointPrefab, positions[i], Quaternion.identity);
            checkpointTransforms.Add(checkpoint.transform);

            // Assign checkpoint index to the trigger script
            CheckpointTrigger trigger = checkpoint.AddComponent<CheckpointTrigger>();
            trigger.checkpointIndex = i;

            // Find the TextMeshPro component in the child
            TextMeshPro text = checkpoint.GetComponentInChildren<TextMeshPro>();
            if (text != null)
            {
                text.text = "Checkpoint " + (i + 1);
            }
        }

        if (positions.Count >= 2 && xrOrigin != null)
        {
            PositionXROrigin(positions[0], positions[1]);
        }

        if (arrowPointer != null)
        {
            arrowPointer.checkpoints = checkpointTransforms;
        }
    }

    void PositionXROrigin(Vector3 checkpoint1, Vector3 checkpoint2)
    {
        Vector3 direction = checkpoint2 - checkpoint1;
        direction.y = 0; // Flatten the direction to avoid pitch

        if (direction.sqrMagnitude < 0.001f) return; // Avoid zero-length direction

        direction.Normalize();
        Vector3 spawnPosition = checkpoint1 - direction * spawnOffset;

        xrOrigin.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(direction, Vector3.up));
    }

    List<Vector3> ParseFile()
    {
        List<Vector3> positions = new List<Vector3>();
        string content = file.text;
        string[] lines = content.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] coords = line.Trim().Split(' ');
            Vector3 pos = new Vector3(
                float.Parse(coords[0]),
                float.Parse(coords[1]),
                float.Parse(coords[2])
            );
            positions.Add(pos * scaleFactor);
        }
        return positions;
    }
}
