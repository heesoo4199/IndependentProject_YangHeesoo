using UnityEngine;
using System.Collections;

public class Start : MonoBehaviour {

    public void onClick(int room)
    {
        Application.LoadLevel(room);
    }
}
