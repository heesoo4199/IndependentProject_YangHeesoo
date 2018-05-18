using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassicModeManager : MonoBehaviour {

	public float speed;
	public float acceleration;
	public bool isPaused;
	public int score;

	public GameObject[] tetriminos = new GameObject[7];
	public Text scoreText;

	// Use this for initialization
	void Start () {
		GenerateTetrimino ();
	}
	
	// Update is called once per frame
	void Update () {
		speed = (speed + acceleration * Time.deltaTime);
	}

	//hi
	public void GenerateTetrimino() {
		if (!isPaused) {
			int chooser = (int)Random.Range (0f, 7f);
			// Prep tetromino
			GameObject tetrimino = (GameObject) Instantiate (tetriminos [chooser]);
			TetriminoManager manager = tetrimino.GetComponent<TetriminoManager> ();
			manager.velocity = speed;
			// Prep copy
			GameObject copy = (GameObject) Instantiate (tetriminos [chooser]);
			copy.tag = "TetriminoCopy";
			Destroy (copy.GetComponent<Rigidbody2D> ());
			Destroy (copy.GetComponent<TetriminoManager> ());
			for (int i = 0; i < 4; i++) {
				GameObject child = copy.transform.GetChild (i).gameObject;
				Destroy (child.GetComponent<BoxCollider2D> ());
				child.GetComponent<SpriteRenderer> ().color = new Color(1f, 1f, 1f, 0.3f);
			}
			manager.copy = copy;
			manager.MoveCopy ();
			// Prep input
			GameObject inputManager = GameObject.FindGameObjectWithTag ("InputManager");
			inputManager.GetComponent<InputManager> ().GetNewActiveTetrimino ();
		}
		scoreText.text = "Score: " + score;
	}

	public void ClearLines() {
		int count = 0;
		for (int i = 0; i < 20; i++) {
			List<Transform> line = GetLine (i);
			if (line.Count == 10) {
				Clear (line, i);
				i--;
				count++;
			}
		}
		score += count;
	}

	// 0 is bottom, 19 is top.
	void Clear(List<Transform> line, int row) {
		// Remove squares in line
		foreach (Transform square in line) {
			Destroy (square.gameObject);
		}
		// Shift all lines above down
		for (int i = row + 1; i < 20; i++) {
			List<Transform> l = GetLine (i);
			foreach (Transform s in l) {
				s.position = new Vector3 (s.position.x, s.position.y - 1f);
			}
		}
	}

	// If piece is detected going out of bounds (sideways), then move it back in.
	void Clamp() {
		Transform l = transform.GetChild (20);
		Transform r = transform.GetChild (21);
		// raycasting
	}

	//
	List<Transform> GetLine(int row) {
		Transform child = transform.GetChild (row);
		RaycastHit2D[] hit = Physics2D.RaycastAll (child.position, Vector2.right, Mathf.Infinity);
		List<Transform> ret = new List<Transform>();
		foreach (RaycastHit2D h in hit) {
			ret.Add (h.collider.transform);
		}
		ret.RemoveAt (0);
		ret.RemoveAt (ret.Count - 1);
		return ret;
	}

	public void Stop() {
		isPaused = true;
	}

}
