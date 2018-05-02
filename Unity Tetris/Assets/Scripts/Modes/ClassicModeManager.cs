using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicModeManager : MonoBehaviour {

	public float speed;
	public bool isPaused;
	public int score;
	public GameObject[] tetriminos = new GameObject[6];

	// Use this for initialization
	void Start () {
		GenerateTetrimino ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateTetrimino() {
		if (!isPaused) {
			int chooser = (int)Random.Range (0f, 6f);
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
	}

	public void Stop() {
		isPaused = true;
	}

}
