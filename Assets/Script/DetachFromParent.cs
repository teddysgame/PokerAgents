using UnityEngine;

public class DetachFromParent : MonoBehaviour
{
	private void Start()
	{
        // Remove from parent
        transform.parent = null;

        // Optional: Disable any specific behaviors or components on the detached object
        // For example, you might disable any scripts related to parenting or attachment

        // Optional: Modify any other properties of the detached object as needed

        // Optional: Add any additional logic or functionality here
    }
}
