using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRespawner : MonoBehaviour
{
	public bool dontPlaySound;
	public bool green;
	public bool blue;
	public bool red;
	public bool orange;
	public bool pink;
	public bool yellow;
	public bool lightBlue;
	public bool black;
	public bool white;
	public float maxDistance;
    public Vector3 startForce;
	private float somethingInsideSpawnAreaTimer;
	private string boxName;
	private Transform boxTrans;
	private IBoxProperties boxProperties;

	private bool spawningIn;
	private GameObject inst;
	private MeshRenderer instmr;
	private float instA;

	// Use this for initialization
	void Start ()
	{
		somethingInsideSpawnAreaTimer = float.MinValue;

        // What cube to spawn
		if (green == true)
			boxName = "Cube Green";
		else if (blue == true)
			boxName = "Cube Blue";
		else if(red == true)
			boxName = "Cube Red";
		else if(orange == true)
			boxName = "Cube Orange";
		else if(pink == true)
			boxName = "Cube Pink";
		else if(yellow == true)
			boxName = "Cube Yellow";
		else if(lightBlue == true)
			boxName = "Cube LightBlue";
		else if(black == true)
			boxName = "Cube Black";
		else if(white == true)
			boxName = "Cube White";

		this.CreateNewCube();
	}

	// Update is called once per frame
	void Update ()
	{
		if (boxTrans != null)
		{
			float curDist = Vector3.Distance(this.transform.position, boxTrans.position);
			if (curDist > maxDistance)
			{
				boxProperties.DestroyBox(false);
				this.CreateNewCube();
			}
		}
		else
			this.CreateNewCube();

		if(spawningIn == true)
		{
			instmr.materials[0].color = new Color(instmr.materials[0].color.r, instmr.materials[0].color.g, instmr.materials[0].color.b, instA);
			instmr.materials[1].color = new Color(instmr.materials[1].color.r, instmr.materials[1].color.g, instmr.materials[1].color.b, instA);
			instA += Time.deltaTime * 6;
			if (instA >= 1)
			{
				inst.GetComponent<Rigidbody>().isKinematic = false;

				Material[] ms = new Material[2];
				ms[0] = Resources.Load<Material>("Materials/Normal/Cube Border");
				ms[1] = Resources.Load<Material>("Materials/Normal/" + boxName);
				instmr.materials = ms;

				instmr.materials[0].color = new Color(instmr.materials[0].color.r, instmr.materials[0].color.g, instmr.materials[0].color.b, 1);
				instmr.materials[1].color = new Color(instmr.materials[1].color.r, instmr.materials[1].color.g, instmr.materials[1].color.b, 1);

				inst.GetComponent<Rigidbody>().AddForce(startForce * 100);
				boxProperties = inst.GetComponent<IBoxProperties>();
				boxProperties.ActivateProperties(dontPlaySound);
				Transform colGO = inst.transform.Find("CollisionChecker");
				if (colGO != null && yellow == false)
					Destroy(colGO.gameObject);
				instA = 0;
				spawningIn = false;
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		// Only sphere Collider is the grav cubes which is trigger
		if (other.GetType() == typeof(SphereCollider))
			return;
		somethingInsideSpawnAreaTimer = Time.time + .25f;
	}

    // Creates the cube 
	private void CreateNewCube()
	{
        // Spawn the cube if there is nothing in there
		if (Time.time > somethingInsideSpawnAreaTimer)
		{
			inst = Instantiate(Resources.Load(boxName), this.transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
			boxTrans = inst.transform;
			inst.transform.rotation = this.transform.rotation;
			Material[] ms = new Material[2];
			ms[0] = Resources.Load<Material>("Materials/Cube Rim");
			ms[1] = Resources.Load<Material>("Materials/" + boxName);
			ms[0].color = new Color(ms[0].color.r, ms[0].color.g, ms[0].color.b, 0);
			ms[1].color = new Color(ms[1].color.r, ms[1].color.g, ms[1].color.b, 0);
			instmr = inst.GetComponent<MeshRenderer>();
			instmr.materials = ms;
			inst.GetComponent<Rigidbody>().isKinematic = true;
			spawningIn = true;
		}
	}
}
