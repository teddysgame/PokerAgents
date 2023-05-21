// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Niantic.ARDK.Utilities.Input.Legacy;

namespace Niantic.ARDKExamples.Pong
{
    public class PhysicsSceneController : MonoBehaviour
    {
        private List<GameObject> objectHolders = new List<GameObject>();

        //original script has a max count to spawn
        //private int objCount, maxObjects = 5;

        private bool isCooling = false;
        public int cooltime = 3;

        Vector2 startPoint;
        Vector2 endPoint;

        public GameObject cardHolding;

        private bool isTouchingButton = false;

        public GameplayController gameplayController;

        bool ignored = false;

        private void Start()
        {
            SetObjectHolders();
        }

        private void Update()
        {
            if (PlatformAgnosticInput.touchCount <= 0) return;

            var touch = PlatformAgnosticInput.GetTouch(0);

            //if (objCount <= maxObjects && !isCooling)
            if (!isCooling)
            {
                if ((Input.GetMouseButtonDown(0) || touch.phase == TouchPhase.Began))
                {
                    if (IsTouchOverUI(touch.position))
                    {
                        // Touch is hitting UI elements (e.g., button)
                        isTouchingButton = true;
                    }
                    else
                    {
                        // Button press or touch began and not hitting UI elements
                        isTouchingButton = false;
                        StoreScreenPoint(touch.position);
                    }
                }
                else if ((Input.GetMouseButtonUp(0) || touch.phase == TouchPhase.Ended))
                {
                    if (!isTouchingButton)
                    {
                        // Button release or touch ended and not hitting UI elements
                        CalculateLaunchSpeed(touch.position);
                        //Debug.Log("hit");
                    }
                    else
                    {
                        // Touch was over a button, handle button press
                        HandleButtonPress(touch.position);
                    }
                }
            }
        }

        public void print()
        {
            Debug.Log("test button");
        }

        private void HandleButtonPress(Vector2 touchPosition)
        {
            // Handle button press logic
            // You can use the EventSystem to find the UI element that was pressed
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touchPosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                // Get the button that was pressed
                var button = results[0].gameObject.GetComponent<Button>();

                if (button != null)
                {
                    // Handle button press
                    button.onClick.Invoke();
                }
            }
        }

        private bool IsTouchOverUI(Vector2 touchPosition)
        {
            // Check if the touch position is over any UI elements
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touchPosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        void StoreScreenPoint(Vector2 point)
        {
            startPoint = point;
        }

        void CalculateLaunchSpeed(Vector2 point)
        {
            endPoint = point;

            // Calculate the distance between the start and end points
            float swipeDistance = Mathf.Abs(endPoint.y - startPoint.y);

            // Calculate the horizontal distance between the start and end points
            float horizontalDistance = endPoint.x - startPoint.x;

            // Use the horizontal distance as the movement value
            //float movementValue = horizontalDistance;

            // Use the swipe distance as the launch speed
            //divide by 100 cuz of the speed
            float launchSpeed = swipeDistance / 100;

            //LaunchBegan(launchSpeed, movementValue);
            LaunchBegan(launchSpeed);
            //Debug.Log("Launch Speed: " + launchSpeed);
            //Debug.Log("Launch spin: " + movementValue);
        }

        //private void LaunchBegan(float launchSpeed, float movementValue)
        private void LaunchBegan(float launchSpeed)
        {
            GameObject objectHolder = ActivateOneOH();
            LaunchObjectHolder(objectHolder, launchSpeed);

            //for limiting spawn
            //objCount++;

            //this is to enable cool down for launching so players do not spam launch
            isCooling = true;
            StartCoroutine(CoolDownSpawn());


            //StartCoroutine(DeactivateObject(objectHolder));
        }

        private void SetObjectHolders()
        {
            //for (int i = 0; i < maxObjects / 3; i++)
            for (int i = 0; i < 1; i++)
            {
                foreach (Transform child in gameObject.transform)
                {
                    if (child.gameObject.name == "PreloadManager") continue;
                    if (child.gameObject.activeSelf) child.gameObject.SetActive(false);

                    GameObject newObj = Instantiate(child.gameObject);
                    var cursor = newObj.transform.Find("cursor");
                    if (cursor != null) Destroy(cursor.gameObject);
                    newObj.SetActive(false);
                    objectHolders.Add(newObj.gameObject);
                }
            }
        }

        private GameObject ActivateOneOH()
        {
            for (int i = 0; i < objectHolders.Count; i++)
            {
                int rd = 0;
                if (!objectHolders[rd].activeSelf) return objectHolders[rd];
            }
            return objectHolders[0];
        }

        //private void LaunchObjectHolder(GameObject obj, float launchSpeed, float movementValue)
        private void LaunchObjectHolder(GameObject obj, float launchSpeed)
        {
            cardHolding.SetActive(false);
            obj.TryGetComponent<PhysicsController>(out PhysicsController physicsController);
            //physicsController.Launch(launchSpeed, movementValue);
            //physicsController.Launch(launchSpeed);

            //if (gameplayController.startingPlayer == "Player 1")
            //{
            //    //enable to take damage
            //    physicsController.GetComponentInChildren<AboveAttack>().resetAttackBool();
            //}
            physicsController.GetComponentInChildren<AboveAttack>().resetAttackBool();
            if (gameplayController.startingPlayer == "Player 1" && !ignored)
            {
                physicsController.GetComponentInChildren<AboveAttack>().EndTurn();
                ignored = true;
            }

        }
        private IEnumerator DeactivateObject(GameObject obj)
        {
            yield return new WaitForSeconds(cooltime);
            //reset the bool
            //obj.GetComponentInChildren<CardDisplay>().changeBool();

            //setactive the object
            obj.SetActive(false);

            //limit object is disabled
            //objCount--;

            //activate the card on the canvas to show the player card is ready to throw
            cardHolding.SetActive(true);
        }

        private IEnumerator CoolDownSpawn()
        {
            yield return new WaitForSeconds(cooltime);
            isCooling = false;
        }
    }
}