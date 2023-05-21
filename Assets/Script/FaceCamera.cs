using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Update()
    {
        // Get the position of the main camera or the player
        Vector3 cameraPosition = Camera.main.transform.position;

        // Make the object face towards the camera/player
        transform.LookAt(cameraPosition);
    }
}
