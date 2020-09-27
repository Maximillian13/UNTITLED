using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeButton : MonoBehaviour
{
	public SandboxTable table;
	public string boxType;
	public bool press;
	private bool learping;
	private float a;
	private bool movingBack;
	private bool spawnCube;
	private Vector3 forwardPos;
	private Vector3 backPos;

	// Start is called before the first frame update
	void Start()
    {
		forwardPos = this.transform.position;
		backPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - .03f);
    }

    // Update is called once per frame
    void Update()
    {
		if (press == true)
			this.PressButton();

		if (learping == true)
		{
			if (movingBack == true)
				a += Time.deltaTime * 4;
			else
				a -= Time.deltaTime * 4;

			this.transform.position = Vector3.Lerp(forwardPos, backPos, a);

			if (a >= 1)
			{
				if(spawnCube == true)
					table.SpawnInBox(this.boxType);
				movingBack = false;
				a = .999f; // To prevent it getting called multiple times 
			}

			if(a <= 0)
			{
				this.transform.position = forwardPos;
				a = 0;
				learping = false;
			}
		}
    }

	public void PressButton()
	{
		press = false;
		if (boxType == "ResetCubes")
			this.ResetCubes();
		else if (boxType == "MainMenu")
			this.MainMenu();
		else
			spawnCube = true;
		this.BoxPress();
	}

	private void ResetCubes()
	{
		GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
		foreach(GameObject b in boxes)
			b.GetComponent<IBoxProperties>().DestroyBox(true);
		table.ClearAllCubeSpawners();
	}

	private void MainMenu()
	{
		GameObject.Find("LevelControl").GetComponent<LevelControl>().ForceLoadLevel(0);
	}

	private void BoxPress()
	{
		learping = true;
		movingBack = true;
	}
}
