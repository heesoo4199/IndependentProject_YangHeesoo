using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartPressed() {
		SceneManager.LoadScene("Main"); // SceneManager.LoadScene("OtherSceneName", LoadSceneMode.Additive);
	}

}
