using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIDisplay : MonoBehaviour
{
    public Card card;
    //public TextMeshProUGUI nameText;
    //public TextMeshProUGUI healthText;
    //public TextMeshProUGUI attackText;

    //public Image targetImage;

    //[HideInInspector]
    public int attack;
    //[HideInInspector]
    public int health;
    //[HideInInspector]
    public string cardname;

    // Start is called before the first frame update
    void Start()
    {
        //nameText.text = card.name;
        //healthText.text = card.health.ToString();
        //attackText.text = card.attack.ToString();
        Image imageComponent = gameObject.GetComponent<Image>();

        // Assign the new sprite to the Image component
        imageComponent.sprite = card.artwork;

        cardname = card.name;
        attack = card.attack;
        health = card.health;
    }

    public void changeImage()
	{
        Image imageComponent = gameObject.GetComponent<Image>();
        // Assign the new sprite to the Image component
        imageComponent.sprite = card.artwork;

        cardname = card.name;
        attack = card.attack;
        health = card.health;
    }
}
