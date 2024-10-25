using UnityEngine;

public class FixedIso : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Vector3 cameraDirection = new Vector3(1, 1, -1); // Default direction
    public float distance = 10f;  // Distance from the player

    void LateUpdate()
    {
        // Normalize the direction to maintain a consistent direction
        Vector3 direction = cameraDirection.normalized;

        // Set the camera position relative to the player
        transform.position = player.position + direction * distance;

        // Make the camera look at the player
        transform.LookAt(player.position);
    }
}
