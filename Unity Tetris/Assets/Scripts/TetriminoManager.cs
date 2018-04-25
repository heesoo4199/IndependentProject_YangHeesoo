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
		float dist = FloorMeasure ();
		transform.position = new Vector3 (transform.position.x, transform.position.y - dist + 1f);
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

	// returns smallest distance from the current piece to the floor.
	float FloorMeasure() {
		List<Transform> list = new List<Transform> ();
		float min = float.MaxValue;
		for (int i = 0; i < 4; i++) {
			if (transform.GetChild(i).position.y < min) {
				min = transform.GetChild(i).position.y;
			}
		}
		for (int i = 0; i < 4; i++) {
			Transform child = transform.GetChild (i);
			if (CloseEnough(child.position.y, min, 0.1f)) {
				list.Add (child);
			}
		}
		float minDist = float.MaxValue;
		foreach (Transform child in list) {
			RaycastHit2D[] hit = Physics2D.RaycastAll (child.position, child.TransformDirection (Vector2.down), Mathf.Infinity);
			// Does the ray intersect any objects excluding the player layer
			if (hit [1])
			{
				if (hit [1].distance < minDist) {
					minDist = hit [1].distance;
				}
			}
			else
			{
				Debug.Log("Did not Hit");
			}
		}
		return minDist;
	}

	static bool CloseEnough(float a, float b, float tolerance)
	{
		return Mathf.Abs(a - b) <= tolerance; 
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
