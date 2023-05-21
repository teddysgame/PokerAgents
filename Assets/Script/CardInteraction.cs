using UnityEngine;

public class CardInteraction : MonoBehaviour
{
    public float scaleIncreaseAmount = 0.1f;
    public float flickThreshold = 500f;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private bool isTouching;
    private Vector2 touchStartPosition;
    private float touchStartTime;

    private void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Touched Down");
            OnTouchBegin(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Touched Up");
            OnTouchEnd(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            //Debug.Log("Touched Move");
            OnTouchMove(Input.mousePosition);
        }
    }

    private void OnTouchBegin(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isTouching = true;
                touchStartPosition = touchPosition;
                touchStartTime = Time.time;
                transform.localScale = initialScale + Vector3.one * scaleIncreaseAmount;
            }
        }
    }

    private void OnTouchMove(Vector2 touchPosition)
    {
        if (isTouching)
        {
            // Perform any card dragging or movement logic here if needed
        }
    }

    private void OnTouchEnd(Vector2 touchPosition)
    {
        if (isTouching)
        {
            isTouching = false;
            transform.localScale = initialScale;

            float touchDuration = Time.time - touchStartTime;
            Vector2 flickDirection = touchPosition - touchStartPosition;
            float flickVelocity = flickDirection.magnitude / touchDuration;

            if (flickVelocity > flickThreshold && flickDirection.y > 0)
            {
                // Flicked upwards, activate the function
                ActivateFunction();
            }
        }
    }

    private void ActivateFunction()
    {
        // Implement the logic for the function you want to activate when flicked upwards
        Debug.Log("Flicked upwards, activating function!");
    }
}
