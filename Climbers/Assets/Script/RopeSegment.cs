using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class RopeSegment : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }


    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" 
            && (other.gameObject.GetComponent<RopeClimbing>() == null
            || other.gameObject.GetComponent<RopeClimbing>().enabled))
        {
            other.gameObject.transform.parent = this.transform;
            other.GetComponent<Rigidbody2D>().isKinematic = true;
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //other.GetComponent<Rigidbody2D>().gravityScale = 0;

            RopeClimbing r = other.gameObject.GetComponent<RopeClimbing>();
            if (other.gameObject.GetComponent<RopeClimbing>() == null)
                r = other.gameObject.AddComponent<RopeClimbing>();
            else
                r.enabled = true;

            r.current_segment = this.transform;
            r.above_segment = this.GetComponent<HingeJoint2D>().connectedBody.transform;
            other.GetComponent<PlatformerCharacter2D>().enabled = false;
        }
    }
}
