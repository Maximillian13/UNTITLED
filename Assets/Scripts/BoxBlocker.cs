using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBlocker : MonoBehaviour
{
	public float speed; // How fast/slow the box can move through
	public bool blockFast; // What it should block (Slow/Fast)

    private MeshRenderer mr;
    private Material mat;
    private float alpha;
    private bool aInc;

    void Start()
    {
        mr = this.GetComponent<MeshRenderer>();
        mat = mr.material;
        alpha = mat.color.a;
        aInc = false;
    }

    void FixedUpdate()
    {
        if (aInc == false)
        {
            // have the alpha fade out
            if (blockFast == false)
                alpha -= .01f;
            else
                alpha -= .002f;
        }
        else
        {
            // have the alpha fade in 
            if (blockFast == false)
                alpha += .01f;
            else
                alpha += .002f;
        }

        if (alpha >= .6f)
            aInc = false;
        if (alpha <= .3f)
            aInc = true;

        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }

	void OnTriggerEnter(Collider other)
	{
		// Ignore sphere collider on black box 
		if (other.tag == "Box" && other.GetType() == typeof(BoxCollider)) 
		{
			// Get the speed of the box and determine if it should be destroyed or not
			Rigidbody rig = other.GetComponent<Rigidbody>();
			IBoxProperties bp = rig.GetComponent<IBoxProperties>();
			if (blockFast == true)
			{
				if (Mathf.Abs(rig.velocity.magnitude) > speed)
					bp.DestroyBox(true);
			}
			else
			{
				if (Mathf.Abs(rig.velocity.magnitude) < speed)
					bp.DestroyBox(true);
			}

		}
	}
	
}
