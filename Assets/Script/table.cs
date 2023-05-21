using UnityEngine;
using Niantic.ARDKExamples.Pong;

public class table : MonoBehaviour
{
    public GameObject gun;
	bool guntaken = false;

	//gun to reappear on the table
	public void GunAppear()
	{
		gun.SetActive(true);
		guntaken = false;
	}

	public void GunDisppear()
	{
		gun.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !guntaken)
		{
			Debug.Log(other.name);
			//gun already taken
			guntaken = true;

			//disable the gun at the table as a pick up anim
			GunDisppear();

			//allow the gun to appear on player's screen
			other.GetComponentInChildren<Gun>().GunAppear();

			//holding gun is now true
			if (other.name == "Player 1 Body")
			{
				GameObject.Find("GameManager").GetComponent<GameController>().holdingGun = true;
			}
		}
	}
}
