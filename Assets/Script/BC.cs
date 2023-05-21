using UnityEngine;

public class BC : MonoBehaviour
{
    public Transform targetObject; // Reference to the object the balloon is tied to
    public float floatForce = 5f; // Force applied to make the balloon float
    public float distanceScale = 1f; // Scale to adjust force based on distance
    public float desiredHeight = 2f; // Desired height above the object

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Set initial position above the target object
        transform.position = new Vector3(targetObject.position.x, targetObject.position.y + desiredHeight, targetObject.position.z);
    }

    private void FixedUpdate()
    {
        // Calculate the force to make the balloon float
        Vector3 force = Vector3.up * floatForce;

        // Calculate the distance between the balloon and the target object's center
        float distance = Vector3.Distance(transform.position, targetObject.position);

        // Calculate the desired height difference
        float heightDifference = desiredHeight - (transform.position.y - targetObject.position.y);

        // Scale down the force based on distance and desired height difference
        force *= 1f / (distance * distanceScale) * heightDifference;

        // Apply the force to the balloon
        rb.AddForce(force, ForceMode.Force);
    }
}
