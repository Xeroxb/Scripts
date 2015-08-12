using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraBehaviour : MonoBehaviour {

	public GameObject target;

	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;
	
	public bool canUpdate = true;
	public bool movingCamera = false;

	private Vector3 v3OrgMouse;

	public List<GameObject> targets = new List<GameObject>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		/*if (target == null)
			target = GameObject.Find ("myPlayer");*/

		if(GameObject.Find("GameController").GetComponent<GameController>().players[GameObject.Find("GameController").GetComponent<GameController>().currentPlayer].name != target.name && !GameObject.FindGameObjectWithTag("Bullet"))
			target = GameObject.Find("GameController").GetComponent<GameController>().players[GameObject.Find("GameController").GetComponent<GameController>().currentPlayer].gameObject;


			
		//if (target.name == "myPlayer") { // se a camera esta no meu player, entao tenho livre controle sobre ela
			if (Input.GetMouseButtonDown(0)) { // mover a camera quando o player pressionar o botao esquerdo do mouse
				canUpdate = false; // para parar de atualizar a camera
				Cursor.SetCursor (cursorTexture, hotSpot, cursorMode); // mudando o cursor do mouse

				movingCamera = true;
				v3OrgMouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -25);
				v3OrgMouse = Camera.main.ScreenToWorldPoint (v3OrgMouse);
				//v3OrgMouse.y = transform.position.y;
			}
			else if (Input.GetMouseButton(0)) {
				var v3Pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -25);
				v3Pos = Camera.main.ScreenToWorldPoint (v3Pos);
				//v3Pos.y = transform.position.y; // freeza a camera no Y (nao podendo mexer para cima e para baixo)
				transform.position -= (v3Pos - v3OrgMouse);
			}
			else if (Input.GetMouseButtonUp(0)) { // quando o player soltar o botao de mover a camera
				Cursor.SetCursor (null, Vector2.zero, cursorMode); // mudando o cursor do mouse para o normal
				movingCamera = false;
				canUpdate = true; // voltando a atualizar a camera
			}
		//}

		/*/if (target.transform.position.x < -10 || target.transform.position.x > 9) // se o player chegar perto dos limites da fase, esquerdo ou direito, parar de mover a camera
			canUpdate = false;
		else/*/ if(!movingCamera) // se nao estiver movendo a camera manualmente
			canUpdate = true;

		if (canUpdate) {
			transform.position = new Vector3 (target.transform.position.x, target.transform.position.y, -25); // atualizando a posicao da camera para o respectico target

		}
	}

	public void changeTarget(GameObject newTarget){ // fazendo a camera mudar de target
		Debug.Log ("mudando target da camera para: " + newTarget.name);
		if (newTarget.tag == "Bullet") { // se eh um projetil, adicionar ele numa lista
			targets.Add(newTarget);
			target = targets[0];
		} 
		else if(!GameObject.FindGameObjectWithTag("Bullet")){
			target = newTarget;
		}
	}
}
