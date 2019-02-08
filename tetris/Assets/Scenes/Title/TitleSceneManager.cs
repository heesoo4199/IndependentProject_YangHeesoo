using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour {

    public Text hs;

	// Use this for initialization
	void Start () {
        hs.text = "High Score - " + PlayerPrefs.GetInt("score");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartPressed() {
        Gamestrap.GSAppExampleControl.Instance.LoadScene(Gamestrap.ESceneNames.Main);
		//SceneManager.LoadScene("Main"); // SceneManager.LoadScene("OtherSceneName", LoadSceneMode.Additive);
	}

}
