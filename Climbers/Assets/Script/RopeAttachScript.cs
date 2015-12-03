using UnityEngine;
using System.Collections;

public class RopeAttachScript : MonoBehaviour {
    public int layerToHit = 11;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == layerToHit)
        {
            HingeJoint2D newHinge = gameObject.AddComponent<HingeJoint2D>();
            newHinge.connectedAnchor = collision.contacts[0].point;
            newHinge.anchor = new Vector2(0,0);
        }
    }
}
