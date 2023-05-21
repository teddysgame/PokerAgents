using System.Collections;
using UnityEngine;
using Niantic.ARDKExamples.Pong;

public class GameplayController : MonoBehaviour
{
    private bool isPlayerTurn = false;
    public bool isConditionMet = false;

    public int p1_score = 0;
    public int p2_score = 0;

    public PhysicsController physicsController;

    public string startingPlayer;
    public string playerturn;

    public GameObject p1;
    public GameObject p2;

    private void Awake()
    {
        // Start the game with a coin flip to determine the starting player
        bool isHeads = CoinFlip();
        isPlayerTurn = isHeads;

        if (isPlayerTurn)
        {
            startingPlayer = "Player 1";
            //Debug.Log("Player 1 starts first.");
            StartCoroutine(PlayerTurn());
        }
        else
        {
            startingPlayer = "Player 2";
            //Debug.Log("Player 2 starts first.");
            //reset condition so player's turn not skipped.
            isConditionMet = true;
            StartCoroutine(OpponentTurn());
        }
    }

    private IEnumerator PlayerTurn()
    {
        Debug.Log("Player 1's turn");
        // Perform player's turn logic here

        playerturn = "Player 1";

        // Wait until the condition is met
        yield return new WaitUntil(() => isConditionMet);

        // The condition is met, execute the coroutine logic
        //Debug.Log("Condition is met!");

        // Player's turn ends, switch to opponent's turn
        isPlayerTurn = false;
        StartCoroutine(OpponentTurn());
    }

    private IEnumerator OpponentTurn()
    {
        Debug.Log("Player 2's turn");
        // Perform opponent's turn logic here

        playerturn = "Player 2";

        // Wait until the condition is met
        yield return new WaitUntil(() => isConditionMet == false);

        // The condition is met, execute the coroutine logic
        //Debug.Log("Condition is met!");

        // Opponent's turn ends, switch to player's turn
        isPlayerTurn = true;
        StartCoroutine(PlayerTurn());
    }

    private bool CoinFlip()
    {
        // Simulating a coin flip, returns true for heads and false for tails
        var coinflip = Random.Range(0, 2) == 0;

        return (coinflip);
    }

    private void Update()
    {
        // Call the function to trigger the next part of the coroutine logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //end turn of player manually
            EndTurn();
        }
    }

    public void EndTurn()
	{
        //Debug.Log("End Turn");
        //this is use to end turn
		if (isConditionMet == true)
		{
            isConditionMet = false;
        }
		else
		{
            isConditionMet = true;
        }
	}

    public void GameOver()
	{
        Debug.Log("Game Over");
    }
}
