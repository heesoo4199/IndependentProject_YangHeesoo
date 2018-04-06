using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			Rotate ();
		}	
	}

	void Rotate() {
	}

	void Drop() {
	}

	void Hold() {

	}

	void Accelerate() {

	}
}
