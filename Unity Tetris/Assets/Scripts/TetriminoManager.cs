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
		Round ();
	}

	public void Right() {
		transform.position = new Vector3 (transform.position.x + 1f, transform.position.y);
		Round ();
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

	// Somehow there are errors in the position even though I am only moving +- 1, so I round it to the nearest 1.
	void Round() {
		float x = transform.position.x;
		transform.position = new Vector3 (Mathf.Round (x), transform.position.y);
	}

	/* TODO: 
	 * 1. Access children (sub-squares)
	 * 2. Find group of squares with lowest y-value and store in list
	 * 3. Raycast downward from the list of lowest-y-value squares
	 * 4. Remaining y-distance left is the lowest distance value
	 */
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

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Wall") {
			GameObject manager = GameObject.FindGameObjectWithTag ("ModeManager");
			manager.GetComponent<ClassicModeManager> ().Stop ();
		}
	}

}
