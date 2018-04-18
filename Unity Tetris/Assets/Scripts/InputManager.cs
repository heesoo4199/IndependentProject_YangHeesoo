using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	private GameObject TetriminoActive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			TetriminoActive.GetComponent<TetriminoManager>().Rotate ();
		}	

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			TetriminoActive.GetComponent<TetriminoManager>().Left ();
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			TetriminoActive.GetComponent<TetriminoManager>().Right ();		
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			TetriminoActive.GetComponent<TetriminoManager> ().Accelerate ();
		} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			TetriminoActive.GetComponent<TetriminoManager> ().UnAccelerate ();
		}
	}

	public void GetNewActiveTetrimino() {
		TetriminoActive = GameObject.FindGameObjectWithTag ("TetriminoActive");
	}

}
