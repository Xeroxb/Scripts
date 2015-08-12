using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour {

	public float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2 (transform.position.x, transform.position.y + 0.02f);

		if (Time.time > startTime + 2f)
			Destroy (gameObject);
	}
}
