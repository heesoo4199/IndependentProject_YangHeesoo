using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : MonoBehaviour {

	public float velocity;
	public GameObject copy;
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
		transform.Rotate (new Vector3 (0, 0, -90));
		//Clamp ();
		RoundX ();
		MoveCopy ();
	}

	public void Left() {
		float leftClearance = SideMeasure (-1);
		if (leftClearance > 1f) {
			transform.position = new Vector3 (transform.position.x - 1f, transform.position.y);
			RoundX ();
			MoveCopy ();
		}
	}

	public void Right() {
		float rightClearance = SideMeasure (1);
		if (rightClearance > 1f) {
			transform.position = new Vector3 (transform.position.x + 1f, transform.position.y);
			RoundX ();
			MoveCopy ();
		}
	}

	public void Accelerate() {
		velocity = velocity_original * 2f;
	}

	public void UnAccelerate() {
		velocity = velocity_original / 2f;
	}

	public void Drop() {
		velocity = 0f;
		RoundX ();
		float dist = FloorMeasure ();
		transform.position = new Vector3 (transform.position.x, transform.position.y - dist + 0.5f);
	}

	// Somehow there are errors in the position even though I am only moving +- 1, so I round it to the nearest 1.
	void RoundX() {
		float newX = transform.position.x;
		transform.position = new Vector3 (Mathf.Round (newX), transform.position.y);
	}

	void RoundY() {
		float y = transform.position.y;
		print (Mathf.Round (y * 10f) / 10f);
		transform.position = new Vector3 (transform.position.x, Mathf.Round (y * 100f) / 100f);
	}

	void RoundChildren() {
		for (int i = 0; i < 4; i++) {
			Transform child = transform.GetChild (i);
			child.position = new Vector3 (Mathf.Round (child.position.x), Mathf.Round (child.position.y));
		}
	}//

	// returns true if current piece is intersecting the wall or another piece.
	bool Intersects() {
		//Physics.com
		// need to update unity.
	}

	// returns smallest distance from the current piece to the floor.
	float FloorMeasure() {
		List<Transform> list = new List<Transform> ();
		for (int i = 0; i < 4; i++) {
			list.Add (transform.GetChild (i));
		}
		float minDist = float.MaxValue;
		foreach (Transform child in list) {
			RaycastHit2D[] hit = Physics2D.RaycastAll (child.position, Vector2.down, Mathf.Infinity);
			if (hit [1].collider.transform.parent.gameObject.tag == "TetriminoActive") {
				continue;
			}
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

	// left = -1, right = 1;
	float SideMeasure(int dir) {
		List<Transform> list = new List<Transform> ();
		for (int i = 0; i < 4; i++) {
			list.Add (transform.GetChild (i));
		}
		float minDist = float.MaxValue;
		foreach (Transform child in list) {
			RaycastHit2D[] hit = Physics2D.RaycastAll (child.position, dir * Vector2.right, Mathf.Infinity);
			if (hit [1].collider.transform.parent.gameObject.tag == "TetriminoActive") {
				continue;
			}
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
			GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeAll;
			//RoundY ();
			Destroy (copy);
			gameObject.tag = "TetriminoInactive";
			if (transform.position.y >= 10) {
				Die ();
			}
			GameObject manager = GameObject.FindGameObjectWithTag ("ModeManager");
			manager.GetComponent<ClassicModeManager> ().ClearLines ();
			manager.GetComponent<ClassicModeManager> ().GenerateTetrimino ();
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Wall") {
			Die ();
		}
	}

	void Die() {
		GameObject manager = GameObject.FindGameObjectWithTag ("ModeManager");
		manager.GetComponent<ClassicModeManager> ().Stop ();
	}

	public void MoveCopy() {
		float dist = FloorMeasure ();
		copy.transform.rotation = transform.rotation;
		copy.transform.position = new Vector3 (transform.position.x, transform.position.y - dist + 0.5f);
	}

}
