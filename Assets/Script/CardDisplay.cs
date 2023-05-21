using UnityEngine;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    //public TextMeshProUGUI nameText;
    //public TextMeshProUGUI healthText;
    //public TextMeshProUGUI attackText;

    //[HideInInspector]
    public int attack;
    //[HideInInspector]
    public int health;
    //[HideInInspector]
    public string cardname;

    public GameObject targetCard;

    //bool updateCard = false;

    // Start is called before the first frame update
    void Start()
    {
        //nameText.text = card.name;
        //healthText.text = card.health.ToString();
        //attackText.text = card.attack.ToString();

        cardname = card.name;
        attack = card.attack;
        health = card.health;
    }

	private void Update()
	{
  //      if (updateCard == false)
		//{
  //          changeLaunchStats();
  //          updateCard = true;
  //      }
    }

    public void changeLaunchStats()
	{
        // Assuming your own card's CardUIDisplay component is attached to the same GameObject
        CardUIDisplay targetCardUIDisplay = targetCard.GetComponent<CardUIDisplay>();

        cardname = targetCardUIDisplay.card.name;
        attack = targetCardUIDisplay.card.attack;
        health = targetCardUIDisplay.card.health;

        // Assuming the CardUIDisplay component is attached to the targetCard game object
        CardDisplay cardDisplay = GetComponent<CardDisplay>();

        // Access the Renderer component of the card's game object
        Renderer renderer = gameObject.GetComponent<Renderer>();

        // Access the material assigned to the renderer
        Material material = renderer.material;

        // Set the new albedo texture
        material.mainTexture = targetCardUIDisplay.card.artwork.texture;

        
    }

	public void changeStats()
	{
        cardname = card.name;
        attack = card.attack;
        health = card.health;

        // Assuming the CardUIDisplay component is attached to the targetCard game object
        CardDisplay cardDisplay = GetComponent<CardDisplay>();

        // Access the Renderer component of the card's game object
        Renderer renderer = gameObject.GetComponent<Renderer>();

        // Access the material assigned to the renderer
        Material material = renderer.material;

        // Set the new albedo texture
        material.mainTexture = cardDisplay.card.artwork.texture;

        
    }

 //   public void changeBool()
	//{
 //       updateCard = false;
 //   }
}
