using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : MonoBehaviour {

	public float velocity;
	private float velocity_original;

	// Use this for initialization
	void Start () {
		velocity_original = velocity;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, transform.position.y - (velocity * Time.deltaTime));
	}

	public void Rotate() {
		transform.Rotate (new Vector3 (0, 0, 90));
	}

	public void Left() {
		transform.position = new Vector3 (transform.position.x - 1f, transform.position.y);
	}

	public void Right() {
		transform.position = new Vector3 (transform.position.x + 1f, transform.position.y);
	}

	public void Accelerate() {
		velocity = velocity_original * 2f;
	}

	public void UnAccelerate() {
		velocity = velocity_original / 2f;
	}

	public void Drop() {
		FloorMeasure ();
	}

	void FloorMeasure() {
		RaycastHit hit;
		Transform child = transform.GetChild (0);
		if (Physics.Linecast(child.position, new Vector3(child.position.x, -10f), out hit)) {
			float dist = hit.distance;

		}
		print (hit.distance);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (gameObject.tag == "TetriminoActive" && (coll.gameObject.tag == "TetriminoInactive" || coll.gameObject.tag == "Wall")) {
			velocity = 0f;
			gameObject.tag = "TetriminoInactive";
			GameObject manager = GameObject.FindGameObjectWithTag ("ModeManager");
			manager.GetComponent<ClassicModeManager> ().GenerateTetrimino ();
		}
	}

}
