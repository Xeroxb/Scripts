using UnityEngine;
using System.Collections;

public class GUIcontroller : MonoBehaviour {

	public Texture guiBg;
	public Texture powerBg;
	public Texture power;
	public Texture lastPower;

	public float lastBarForce = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.DrawTexture (new Rect(0, Screen.height*0.85f, Screen.width, Screen.height*0.15f), guiBg);

		GUI.Label(new Rect(Screen.width*0.1f, Screen.height*0.9f, Screen.width*0.1f, Screen.height*0.1f),  GameObject.Find("myPlayer").GetComponent<PlayerBehaviour>().angle+ "°");
	
		GUI.Label (new Rect (Screen.width*0.2f, Screen.height*0.86f, Screen.width*0.1f, Screen.height*0.1f), "0");
		GUI.Label (new Rect (Screen.width*0.35f, Screen.height*0.86f, Screen.width*0.1f, Screen.height*0.1f), "25");
		GUI.Label (new Rect (Screen.width*0.5f, Screen.height*0.86f, Screen.width*0.1f, Screen.height*0.1f), "50");
		GUI.Label (new Rect (Screen.width*0.65f, Screen.height*0.86f, Screen.width*0.1f, Screen.height*0.1f), "75");
		GUI.Label (new Rect (Screen.width*0.77f, Screen.height*0.86f, Screen.width*0.1f, Screen.height*0.1f), "100");
		GUI.DrawTexture (new Rect(Screen.width*0.2f, Screen.height*0.9f, Screen.width * lastBarForce/30, Screen.height*0.05f), lastPower);
		GUI.DrawTexture (new Rect(Screen.width*0.2f, Screen.height*0.9f, Screen.width * GameObject.Find("myPlayer").GetComponent<PlayerBehaviour>().force/30, Screen.height*0.05f), power);
		GUI.DrawTexture (new Rect(Screen.width*0.2f, Screen.height*0.9f, Screen.width*0.6f, Screen.height*0.05f), powerBg);

		//GUI.Label(new Rect(Screen.width*0.9f, Screen.height*0.9f, Screen.width*0.1f, Screen.height*0.1f), GameObject.Find("myPlayer").GetComponent<PlayerBehaviour>().force.ToString());
	}

	public void UpdateLastBar(float newLastBarForce){
		lastBarForce = newLastBarForce;
	}
}
