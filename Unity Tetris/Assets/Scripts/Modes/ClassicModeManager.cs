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
			GameObject tetrimino = (GameObject) Instantiate (tetriminos [(int) Random.Range (0f, 6f)]);
			tetrimino.GetComponent<TetriminoManager> ().velocity = speed;
			GameObject inputManager = GameObject.FindGameObjectWithTag ("InputManager");
			inputManager.GetComponent<InputManager> ().GetNewActiveTetrimino ();
		}
	}

	public void Stop() {
		isPaused = true;
	}

}
