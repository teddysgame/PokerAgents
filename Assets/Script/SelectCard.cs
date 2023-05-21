using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{
    //create a list
    //public List<GameObject> childList = new List<GameObject>();
    public GameObject targetCard;
    public CardDisplay launchTarget;
    //public GameObject myCard;

    // Start is called before the first frame update
    void Start()
    {
        //// Add all children to the list
        //foreach (Transform child in transform)
        //{
        //    childList.Add(child.gameObject);
        //}
    }

	public void selectCard()
	{
		CardUIDisplay cardUIDisplay = targetCard.GetComponent<CardUIDisplay>();

		// Assuming your own card's CardUIDisplay component is attached to the same GameObject
		CardUIDisplay myCardUIDisplay = GetComponent<CardUIDisplay>();

		// Assign your own card's Card component to the targetCard's CardUIDisplay component
		cardUIDisplay.card = myCardUIDisplay.card;

        //call function
        cardUIDisplay.changeImage();

        //change 3D model stats
        launchTarget.GetComponent<CardDisplay>().card = myCardUIDisplay.card;

        //call function
        launchTarget.changeStats();

    }

}
