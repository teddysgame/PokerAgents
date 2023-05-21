using UnityEngine;

public class BalloonController : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public float floatHeight = 2f; // Height above the player's head
    public float smoothSpeed = 5f; // Speed of the balloon's movement

    private Vector3 targetPosition; // Target position of the balloon

    private void Start()
    {
        // Set the initial target position above the player's head
        targetPosition = playerTransform.position + Vector3.up * floatHeight;
    }

    private void Update()
    {
        // Set the target position above the player's head
        targetPosition = playerTransform.position + Vector3.up * floatHeight;

        // Smoothly move the balloon towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Rotate the bottom of the balloon to face towards the player
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.down);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
