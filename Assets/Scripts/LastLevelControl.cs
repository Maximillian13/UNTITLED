using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastLevelControl : MonoBehaviour
{
	public bool lastLevel; // if its the last or second to last level or the last level
	private float timer; 
	private float endTimer;

	// Use this for initialization
	void Start () {
		timer = float.MaxValue;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.time > timer)  // load the next level
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (lastLevel == true && endTimer >= 20) // Quit the game after 20 seconds
            Application.Quit();

		endTimer += Time.deltaTime;
	}

	public void CubeKilled() // If the cube gets pulled out of the table 
	{
		timer = Time.time + 8;
	}
}
