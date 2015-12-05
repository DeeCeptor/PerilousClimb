using UnityEngine;
using System.Collections;

public class AttachToTerrain : MonoBehaviour 
{


	void Start () 
	{
	
	}
	

	void Update () 
	{
	
	}


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            /*
            HingeJoint2D joint = this.GetComponent<HingeJoint2D>();
            if (joint != null)
            {
                joint.enabled = true;
                joint.anchor = Vector2.zero;
                joint.connectedAnchor = Vector2.zero;
            }*/
            this.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }
}
