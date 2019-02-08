using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	private GameObject TetriminoActive;
    private ClassicModeManager manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("ModeManager").GetComponent<ClassicModeManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!manager.isPaused) {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                TetriminoActive.GetComponent<TetriminoManager>().Rotate();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TetriminoActive.GetComponent<TetriminoManager>().Drop();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TetriminoActive.GetComponent<TetriminoManager>().Left();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                TetriminoActive.GetComponent<TetriminoManager>().Right();
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                TetriminoActive.GetComponent<TetriminoManager>().Accelerate();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                TetriminoActive.GetComponent<TetriminoManager>().UnAccelerate();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                manager.Hold();
            }
	    }
    }

	public void GetNewActiveTetrimino() {
		TetriminoActive = GameObject.FindGameObjectWithTag ("TetriminoActive");
	}

}
