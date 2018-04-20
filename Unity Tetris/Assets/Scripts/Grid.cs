using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	// True if space is occupied. 20Hx10W
	public Cube[][] grid = new Cube[20][10];

	class Cube : MonoBehaviour {

		Color col;
		bool active;

		public Cube () {
			col = Random.ColorHSV;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
