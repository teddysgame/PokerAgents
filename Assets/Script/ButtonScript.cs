using UnityEngine;
using UnityEngine.UI;
using Niantic.ARDKExamples.Pong;

public class ButtonScript : MonoBehaviour
{
    public GameObject targetObject;

    private void Start()
    {
        targetObject = GameObject.Find("GameManager");

        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (targetObject != null)
        {
            // Attach the script to the target object and call its function
            targetObject.GetComponent<GameController>().OnUIButtonPressed();

            // Attach the script to the target object and call its function
            targetObject.GetComponent<GameController>().dropPlayerGun();
        }
    }
}
