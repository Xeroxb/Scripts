using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject[] players;
	public int currentPlayer = 0;
	public int turnCount = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setTurn(int newCurrentPlayer)
	{
		Debug.Log ("turno do player: " + newCurrentPlayer);

		currentPlayer = newCurrentPlayer;
		//currentPlayer++;
		
		if (currentPlayer+1 > players.Length) { // turno acabou
			Debug.Log("fim do turno: "+turnCount);
			currentPlayer = 0;
			turnCount++;
		}
		
		players [currentPlayer].GetComponent<PlayerBehaviour> ().canMove = true;
		players [currentPlayer].GetComponent<PlayerBehaviour> ().canShoot = true;
		GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().changeTarget (players [currentPlayer]);
		// comecar a contar o tempo
	}
}
