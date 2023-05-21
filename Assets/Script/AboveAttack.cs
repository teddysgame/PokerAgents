using UnityEngine;
using System.Collections;
using TMPro;

public class AboveAttack : MonoBehaviour
{
	//bool hitOnce = false;
	public GameObject hitEffect;
	public GameObject deathEffect;

	public float launchForce = 10f; // The force to launch the object with
	public float spinForce = 100f;

	private Vector3 initialPosition;

	public CardDisplay cardDisplay;
	private int myHealth;
	private int attack;

	public bool attacked = true;

	//Connect to gameplay controller
	public GameplayController gameplayController;

	public TextMeshProUGUI damageText;

	// Start is called before the first frame update
	private void Awake()
	{
		initialPosition = transform.position;

		//initiate the health and attack
		myHealth = cardDisplay.health;

		
		
	}
	private void OnTriggerStay(Collider other)
	{

		//Debug.Log(cardDisplay.name + "'s health is " + myHealth);
		Rigidbody otherRb = other.GetComponentInParent<Rigidbody>();

		if (otherRb != null && otherRb.velocity.magnitude == 0f)
		{
			if (other.CompareTag("Card"))
			{
				//find the gamemanager GO
				gameplayController = GameObject.Find("GameManager").GetComponent<GameplayController>();

				//hitOnce = true;
				//acccess other card's stats
				CardDisplay otherCardDisplay = other.GetComponent<CardDisplay>();

				//Debug.Log(gameplayController.playerturn);
				//Debug.Log("2 " + other.GetComponent<AboveAttackController>().PlayerRole);

				//if false = can receive damage
				if (gameplayController.playerturn == other.GetComponent<AboveAttackController>().PlayerRole && attacked == false)
				{
					myHealth = myHealth - otherCardDisplay.attack;
					attacked = true;

					//update damage text
					damageText = GameObject.Find("DamageText").GetComponent<TextMeshProUGUI>();
					damageText.text = cardDisplay.name + " receives " + otherCardDisplay.attack + "damages!";

					Debug.Log(cardDisplay.name + " healh is now " + myHealth);
					//play effect if life is more than 0
					if (myHealth > -10)
					{
						//get hit effect
						Instantiate(hitEffect, transform);
					}

					//find the gamemanager GO
					//gameplayController = GameObject.Find("GameManager").GetComponent<GameplayController>();
					gameplayController.EndTurn();
					//Debug.Log("Call for end turn");

					return;
				}

				//evaluate action
				if (myHealth <= 0)
				{
					////Debug.Log(otherCardDisplay.name + " hits " + cardDisplay.name);
					////Debug.Log(otherCardDisplay.attack + " is more than " + cardDisplay.health);

					////ensure it is hit only once
					//hitOnce = true;

					//// Add Rigidbody component to the current game object
					//Rigidbody rb = gameObject.AddComponent<Rigidbody>();

					//rb = GetComponentInParent<Rigidbody>();

					//// Apply a vertical force to the object to launch it into the air
					//rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

					//// Apply spin to the object around the Y-axis
					//rb.AddTorque(Vector3.up * spinForce, ForceMode.Impulse);

					//play death effect
					Instantiate(deathEffect, transform);

					StartCoroutine(destroyGO());
					

					//console print dead
					Debug.Log(gameObject.name + " is dead");

					//find the gamemanager GO to end turn
					gameplayController = GameObject.Find("GameManager").GetComponent<GameplayController>();
					gameplayController.GameOver();

					return;

				}
				//else
				//{
				//	//Debug.Log(cardDisplay.name + "I'm not dead");
				//}

				//StartCoroutine(RefreshBool());
			}
			//else
			//{
			//	Debug.Log(other.gameObject.name);
			//}

			return;
		}
	}

	//private void OnTriggerExit(Collider other)
	//{
	//	Debug.Log("exit");

	//	//enable to attack
	//	attacked = false;
	//}

	public void EndTurn()
	{
		gameplayController.EndTurn();
	}

	public void resetAttackBool()
	{
		//find the gamemanager GO
		gameplayController = GameObject.Find("GameManager").GetComponent<GameplayController>();

		if (gameplayController.playerturn == GetComponent<AboveAttackController>().PlayerRole)
		{
			attacked = false;
		}
	}
	IEnumerator destroyGO()
	{
		yield return new WaitForSeconds(0.1f);
		//Destroy(gameObject);
		//ResetPosition();
		gameObject.SetActive(false);
	}

	private void ResetPosition()
	{
		RemoveComponent<Rigidbody>();
		//Vector3 newPosition = initialPosition;
		//newPosition.y = 0f;
		transform.position = initialPosition;

	}

	private void RemoveComponent<T>() where T : Component
	{
		T component = GetComponent<T>();
		if (component != null)
		{
			Destroy(component);
		}
	}

	//IEnumerator RefreshBool()
	//{
	//	yield return new WaitForSeconds(2);
	//	hitOnce = false;
	//}
}
