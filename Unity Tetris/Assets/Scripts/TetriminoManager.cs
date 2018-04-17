using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoManager : MonoBehaviour {

	public float velocity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, transform.position.y - (velocity * Time.deltaTime));
	}

	public void Rotate() {
		transform.Rotate (new Vector3 (0, 0, 90));
	}

	public void Left() {
		transform.position = new Vector3 (transform.position.x - 2, transform.position.y);
	}

	public void Right() {
		transform.position = new Vector3 (transform.position.x + 2, transform.position.y);
	}

	public void Accelerate() {

	}

}
