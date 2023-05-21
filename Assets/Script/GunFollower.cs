using UnityEngine;

public class GunFollower : MonoBehaviour
{
    public Transform playerBody; // Reference to the player's body transform

    private void Update()
    {
        // Update the gun's rotation to match the player's body rotation
        transform.rotation = playerBody.rotation;
    }
}
