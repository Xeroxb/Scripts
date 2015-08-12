using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerBehaviour : NetworkBehaviour {

	private Rigidbody2D rigid;

	public GameObject bullet;
	public GameObject aim;

	public float speed = 1.0f;
	public float life = 100;

	public float facingRight = 1f;

	public float force = 1f;
	public float angle = 0f;

	public float xDir;
	public float yDir;

	public GameObject myPlayer;
	public GameObject lifeBar;

	public bool canMove = false;
	public bool canShoot = false;
	public bool powerReturn = false;

	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody2D> ();
		if (!isLocalPlayer) { // se nao eh local
			aim.SetActive (false);
		}
		else { // se eh local (meu player)
			myPlayer = gameObject;
			name = "myPlayer";
			GameObject.Find("Main Camera").GetComponent<CameraBehaviour>().changeTarget(gameObject);
			//GameObject.Find("Main Camera").SetActive(true);

			if(isServer){
				StartCoroutine(WaitLoad());
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (!isLocalPlayer) { // se nao for meu player, sair do codigo, pois se eu entrar no codigo vou fazer o player de outro andar
			return;
		}

		if (canMove) {
			float moveHorizontal = Input.GetAxis ("Horizontal"); // recebendo teclas horizontais para definir movimento

			if (Input.GetKeyDown ("left") && facingRight == 1 && !Input.GetKey("right")) { // fazendo o player girar pra esquerda
				CmdFlip (3); // mandando um comando na rede para o player girar para a esquerda, apenas se ele estiver olhando para a direita
			}
			else if (Input.GetKeyDown ("right") && facingRight == -1 && !Input.GetKey("left")) { // fazendo o player girar para a direita
				CmdFlip (-3);
			}

			// PEGANDO MOVIMENTO DO PLAYER
			Vector3 movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
			rigid.velocity = movement * speed; // calculando velocidade
		}

		// MUDANDO DE ANGULO
		if (Input.GetKey ("up")) { // mudando angulo para cima
			if(angle < 90) {
				aim.transform.Rotate(Vector3.forward * -0.5f);
				angle += 0.5f;
				//GameObject.Find("Angle").GetComponent<GUIText>().text = angle.ToString() + "°";
			}
		}
		if (Input.GetKey ("down")) { // mudando angulo para baixo
			if(angle > 0){
				aim.transform.Rotate(Vector3.forward * 0.5f);
				angle -= 0.5f;
				//GameObject.Find("Angle").GetComponent<GUIText>().text = angle.ToString() + "°";
			}
		}

		// ATIRANDO
		if (canShoot) {
			if (Input.GetKeyDown ("space")) {// player apertou o botao de atirar
				canMove = false;
			}
			else if (Input.GetKey ("space")) { // enquanto o player estiver com a tecla espaco pressionada, a variavel forca eh incrementada
				if (force < 18 && !powerReturn) // se a barra nao chegou no limite e nao esta voltando
					force += 0.1f;
				else if (force > 0 && force < 19 && powerReturn) // barra de forca retornando
					force -= 0.1f;
				else if (force < 0 && powerReturn) // caso esteja retornando e seja menor que zero
					powerReturn = false;
				else if (force >= 18) // se barra chegou no limite, tem que voltar
					powerReturn = true;
			} else if (Input.GetKeyUp ("space")) { // quando solta a barra de espaco

				// SETANDO uma barra para indicar ate onde foi a ultima
				GameObject.Find("GUI").GetComponent<GUIcontroller>().UpdateLastBar(force);

				// SENTANDO A DIRECAO DO TIRO
				if (angle >= 0 && angle <= 45) {
					xDir = 1;
					yDir = angle / 45;
				} else if (angle > 45 && angle <= 90) {
					xDir = 1 - ((angle - 45) * 0.0222f);
					yDir = 1;
				}

				if (facingRight > 0) {
					if (xDir < 0)
						xDir = xDir * -1f;
				} else if (facingRight < 0) {
					if (xDir > 0)
						xDir = xDir * -1f;
				}

				if (isServer) {
					Shoot (facingRight, xDir, yDir, force);
				}

				CmdShoot (facingRight, xDir, yDir, force); // fazer a bala ser instanciada na rede

				force = 0; // resetar a barra de forca
				//canMove = true;
				canShoot = false;
				powerReturn = false;
				if(GameObject.FindGameObjectWithTag("Bullet")) // se ainda tem algum projetil no jogo, esperar ate ele ser destruido
					StartCoroutine(WaitBullet());
				else // se nao tem projetil no jogo, fazer a camera seguir o player da vez
					EndTurn();
			}
		}
	}


	IEnumerator WaitLoad() { // funcao que espera os players serem carregados
		GameObject[] lobbyPlayers = GameObject.FindGameObjectsWithTag ("LobbyPlayer");
		GameObject.Find("GameController").GetComponent<GameController>().players = GameObject.FindGameObjectsWithTag ("Player");

		while (lobbyPlayers.Length != GameObject.Find("GameController").GetComponent<GameController>().players.Length) {
			GameObject.Find("GameController").GetComponent<GameController>().players = GameObject.FindGameObjectsWithTag ("Player");
			yield return new WaitForSeconds(0.5f);
		}

		Debug.Log ("todos os players foram carregados!");
		CmdUpdatePlayers (GameObject.Find("GameController").GetComponent<GameController>().players); // atualizando a lista dos players para os players na rede
		CmdTurnSystem (0); // comecando do player zero da lista
	}

	[Command]
	void CmdUpdatePlayers(GameObject[] newList){
		RpcUpdatePlayers (newList);
	}

	[ClientRpc]
	void RpcUpdatePlayers(GameObject[] newList){
		gameObject.name = "Host"; // recebi uma mensagem do host, aproveito para marcar ele como o host
		GameObject.Find("GameController").GetComponent<GameController>().players = newList; // atualizar a lista de players
	}

	IEnumerator WaitBullet() {
		while (GameObject.FindGameObjectWithTag("Bullet")) {
			yield return new WaitForSeconds(1);
		}

		EndTurn ();
	}

	public void EndTurn(){
		GameObject.Find ("GameController").GetComponent<GameController> ().currentPlayer++;
		CmdTurnSystem (GameObject.Find("GameController").GetComponent<GameController>().currentPlayer);
	}

	[Command]
	public void CmdTurnSystem(int newCurrentPlayer){
		GameObject.Find("GameController").GetComponent<GameController>().setTurn (newCurrentPlayer);
		RpcSetTurn (newCurrentPlayer);
	}

	[ClientRpc]
	void RpcSetTurn(int newCurrentPlayer){ // fazendo o player da vez poder andar, atirar, setar a camera nele e comecar a contar o seu tempo
		GameObject.Find("GameController").GetComponent<GameController>().setTurn (newCurrentPlayer);
	}

	public void takeDamage(float damage){
		life -= damage;

		if (life < 0) {
			life = 0;
			Destroy(gameObject);
		}

		if (life <= 25) {
			lifeBar.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("lowLife");
			lifeBar.transform.localScale = new Vector2((facingRight * -life)/100, lifeBar.transform.localScale.y);
		} else if (life < 100) {
			lifeBar.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("life");
			lifeBar.transform.localScale = new Vector2((facingRight * -life)/100, lifeBar.transform.localScale.y);
		}
	}

	// flipando o player, virando pra esquerda ou direita
	[Command]
	void CmdFlip(float x){

		facingRight = -facingRight;
		transform.localScale = new Vector2(x, transform.localScale.y);

		if (x == -3) {
			lifeBar.transform.localScale = new Vector2(-lifeBar.transform.localScale.x, lifeBar.transform.localScale.y);
			lifeBar.transform.position = new Vector2 (transform.position.x - 0.91f, lifeBar.transform.position.y);
		} else if (x == 3) {
			lifeBar.transform.localScale = new Vector2(-lifeBar.transform.localScale.x, lifeBar.transform.localScale.y);
			lifeBar.transform.position = new Vector2 (transform.position.x - 0.91f, lifeBar.transform.position.y);
		}

		RpcFlip (x);
	}

	[ClientRpc]
	void RpcFlip(float x){

		if (!isServer) { // para nao flipar o personagem duas vezes
			facingRight = -facingRight;

			transform.localScale = new Vector2 (x, transform.localScale.y);

			if (x == -3) {
				lifeBar.transform.localScale = new Vector2 (-lifeBar.transform.localScale.x, lifeBar.transform.localScale.y);
				lifeBar.transform.position = new Vector2 (transform.position.x - 0.91f, lifeBar.transform.position.y);
			} else if (x == 3) {
				lifeBar.transform.localScale = new Vector2 (-lifeBar.transform.localScale.x, lifeBar.transform.localScale.y);
				lifeBar.transform.position = new Vector2 (transform.position.x - 0.91f, lifeBar.transform.position.y);
			}
		}
	}



	// spawnando a bala na rede
	[Command]
	void CmdShoot(float direction, float x, float y, float z){
		RpcShoot (direction, x, y, z);
	}

	[ClientRpc]
	void RpcShoot(float direction, float x, float y, float z){
		Shoot (direction, x, y, z);
	}

	void Shoot(float direction, float x, float y, float z){
		GameObject clone = Instantiate(bullet, transform.position, transform.rotation) as GameObject;
		clone.transform.localScale = new Vector2(-direction, 1); // fazendo a bala "olhar" pro lugar certo
		clone.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(new Vector2(x, y) * z);
		clone.GetComponent<BulletBehaviour> ().damage = z; // passando a forca com que o projetil foi jogado para ser adicionado no dano final

		Debug.Log ("chamando funcao target da camera");
		Camera.main.GetComponent<CameraBehaviour>().changeTarget(clone);
	}
}
