using UnityEngine;

public class FlagRotation : MonoBehaviour
{
    void Update()
    {
        // Get the current rotation
        Quaternion currentRotation = transform.rotation;

        // Set the Y rotation to 0
        Quaternion targetRotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0);

        // Apply the new rotation
        transform.rotation = targetRotation;
    }
}
