using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Minion")]
public class Card : ScriptableObject
{
	public new string name;
	public string description;
	public Sprite artwork;

	public int manacost;
	public int attack;
	public int health;
}
