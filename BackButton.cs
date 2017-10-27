using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour {

    public GameObject global;
	// Use this for initialization
	public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
