using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControl : MonoBehaviour
{
	// The receptacles in the level
	public Recepticle[] recepticles;
	public bool multiRecepticle;
	private bool multiRecDone;
	private float timer;
	private bool fadingWhite;
	private float fadingWhiteTimer;
	private EyeFadeControl eyeFade;

	public bool specail;
	private int forceLevel = -1;

	void Start()
	{
		// Update the player pref if the player is further than they have been
		int currentLevel = SceneManager.GetActiveScene().buildIndex;
		int ppLevel = PlayerPrefs.GetInt("Level");

		if (currentLevel > ppLevel)
			PlayerPrefs.SetInt("Level", currentLevel);
	}

	// Update is called once per frame
	void Update()
	{
		if(specail == true)
		{
			if (forceLevel != -1)
			{
				// Get and start fade
				if (eyeFade == null)
				{
					eyeFade = GameObject.Find("[CameraRig]").transform.Find("Camera").Find("EyeCover").GetComponent<EyeFadeControl>();
					eyeFade.FadeWhite();
				}

				// After fade load level
				fadingWhiteTimer += Time.deltaTime;
				if (fadingWhiteTimer > .6f)
					SceneManager.LoadScene(forceLevel);
			}
			return;
		}
		// Check the receptacles and if they are not all complete return out of the method
		for (int x = 0; x < recepticles.Length; x++)
		{
			if (multiRecepticle == false)
			{
				if (recepticles[x].Complete == false)
					return;
			}
			else
			{
				if (recepticles[x].InstanceIsDoneAndWaiting == false)
					return;
			}
		}

		multiRecDone = true;

		if (multiRecepticle == true)
		{
			timer += Time.deltaTime;
			if (timer < 2)
				return;
		}

		// Get and start fade
		if (eyeFade == null)
		{
			eyeFade = GameObject.Find("[CameraRig]").transform.Find("Camera").Find("EyeCover").GetComponent<EyeFadeControl>();
			eyeFade.FadeWhite();
		}

		// After fade load level
		fadingWhiteTimer += Time.deltaTime;
		if (fadingWhiteTimer > .6f)
		{
			// If we are here then everything is done so load the next level if there is one
			int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
			if (SceneManager.sceneCountInBuildSettings > nextScene)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			else
				SceneManager.LoadScene(nextScene - 1);
		}
	}


	public bool GetMultiRec()
	{
		return multiRecepticle;
	}

	public bool GetMultiRecDone()
	{
		return multiRecDone;
	}

	public void ForceLoadLevel(int levelToLoad, bool killMusic = false)
	{
		if(killMusic == true)
		{
			GameObject music = GameObject.Find("Music");
			if (music != null)
				Destroy(music.gameObject);
		}
		forceLevel = levelToLoad;
	}
}
