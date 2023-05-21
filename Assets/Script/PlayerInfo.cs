using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    // Public int variables
    //public int health;
    public int score;
    public TextMeshProUGUI scoreText;

    public GameObject gunHolder;

    public void EnterScore(int scorefromGC)
	{
        score = scorefromGC;
        scoreText.text = scorefromGC.ToString();
    }

	//private void OnTriggerEnter(Collider other)
	//{
	//	if (other.CompareTag("Gun"))
	//	{
 //           PickupGun();
	//	}
	//}
	//public void PickupGun()
	//{
 //       gunHolder.SetActive(true);
 //   }
}
