using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBehaviour : MonoBehaviour {

	public float startTime;
	
	public float damage;
	public float baseDamage = 15f;

	public GameObject damageText;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > startTime + 4f)
			destroy (gameObject);
	}

	void destroy(GameObject toDestroy){
		//GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets.RemoveAt (0); // removendo o primeiro projetil da lista

		if (GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets.Count > 1) { // se ainda tem algo na lista para seguir

			List<GameObject> newTargets = new List<GameObject>();

			for(int i = 1; i < GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets.Count; i++){
				newTargets.Add(GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets[i]);
			}

			GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets = newTargets; // setando a nova lista de targets
			GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().target = GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets [0]; // fazendo a camera seguir o proximo projetil
		}
		else { // nao tem mais nenhum target na lista, seguir o player entao
			GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().targets = new List<GameObject>(); // resetando a lista
			GameObject.Find ("Main Camera").GetComponent<CameraBehaviour> ().target = GameObject.Find ("myPlayer"); // fazendo a camera seguir o player
		}

		Destroy (toDestroy);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Player") { // se o projetil atingir um player
			Debug.Log ("acertou um player!");
			destroy (gameObject);

			coll.gameObject.GetComponent<PlayerBehaviour>().takeDamage(baseDamage+damage);

			// instanciar um texto com o dano causado
			GameObject clone =  Instantiate (damageText, gameObject.transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<TextMesh> ().text = (baseDamage+damage).ToString();
		}
		else if (coll.gameObject.tag == "Floor") // se o projetil atingir o chao
			destroy (gameObject);

		// tocar alguma animacao de explosao e som
		// chamar uma funcao para aplicar o dano
		
	}

	void OnTriggerExit2D(Collider2D other) { // quando a bala sair do player (de quem atirou a bala)
		GetComponent<CircleCollider2D> ().isTrigger = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		//characterInQuicksand = true;
	}
}
