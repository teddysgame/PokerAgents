using UnityEngine;
using Niantic.LightshipHub.Templates;

public class OpponentLauncher : MonoBehaviour
{
    public GameObject opponentCard;
    private Transform opponentCardPos;

    public GameplayController gameplayController;
    //bool ignoreStart = false;
    bool ignored = false;

    // Start is called before the first frame update
    void Start()
    {
        opponentCardPos = opponentCard.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Call the function to trigger the next part of the coroutine logic
        if (Input.GetKeyDown(KeyCode.Return))
        {
            opponentCard.GetComponent<Rigidbody>().useGravity = true;
            //end turn of player manually
            //opponentCard.GetComponent<PhysicsController>().Launch(5);

            //ignoreStart = false;

            //         if (gameplayController.startingPlayer == "Player 2")
            //{
            //             //ignoreStart = true;
            //             //ignored = true;
            //             //enable opponent card to receive damage
            //             opponentCard.GetComponentInChildren<AboveAttack>().resetAttackBool();
            //         }
            //else if (ignoreStart)
            //{
            //             //enable opponent card to receive damage
            //             
            //         }
            opponentCard.GetComponentInChildren<AboveAttack>().resetAttackBool();

            //end turn immediately after throwing if opponent is first player.
			if (gameplayController.startingPlayer == "Player 2" && !ignored)
			{
                opponentCard.GetComponentInChildren<AboveAttack>().EndTurn();
                ignored = true;
            }
        }

        //reset position prior launch
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetPos();
        }
    }

    public void ResetPos()
	{
        opponentCard.GetComponent<Rigidbody>().useGravity = false;
        //reset card to the beginning
        opponentCard.transform.position = opponentCardPos.position;
    }
}
