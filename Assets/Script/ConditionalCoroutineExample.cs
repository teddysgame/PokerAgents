using System.Collections;
using UnityEngine;

public class ConditionalCoroutineExample : MonoBehaviour
{
    private bool isConditionMet = false;

    private void Start()
    {
        StartCoroutine(ConditionalCoroutine());
    }

    private IEnumerator ConditionalCoroutine()
    {
        // Wait until the condition is met
        yield return new WaitUntil(() => isConditionMet);

        // The condition is met, execute the coroutine logic
        Debug.Log("Condition is met!");

        // Wait for a specific function to be called
        yield return StartCoroutine(WaitForFunctionCall());

        // The function is called, execute the next part of the coroutine logic
        Debug.Log("Function is called!");

        // Continue with the rest of the coroutine logic
        Debug.Log("Coroutine completed!");
    }

    private IEnumerator WaitForFunctionCall()
    {
        // Wait until a specific function is called
        while (true)
        {
            yield return null; // Yield once per frame

            // Check if the function is called
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // The function is called, break out of the loop
                break;
            }
        }
    }

    private void Update()
    {
        // Set the condition to true when a certain condition is met
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isConditionMet = true;
        }

        // Call the function to trigger the next part of the coroutine logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(WaitForFunctionCall());
        }
    }
}
