using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveAttackController : MonoBehaviour
{
    public string PlayerRole;
    public GameplayController gameplayController;
    //public AboveAttack aboveAttack;
    public BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = transform.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
		if (gameplayController.playerturn == "Player 1")
		{
			if (PlayerRole == "Player 1")
			{
                boxCollider.enabled = false;
            }
			else
			{
                boxCollider.enabled = true;
            }
		}
		else
		{
            if (PlayerRole == "Player 2")
            {
                boxCollider.enabled = false;
            }
            else
            {
                boxCollider.enabled = true;
            }
        }
    }
}
