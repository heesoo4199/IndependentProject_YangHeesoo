using UnityEngine;
using System.Collections;

public class backgroundMovement : MonoBehaviour {

    public float speed = 0.5f;
    public Renderer rend;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 offSet = new Vector2(Time.time * speed, Time.time * speed);

         rend.material.mainTextureOffset = offSet;
	}
}
