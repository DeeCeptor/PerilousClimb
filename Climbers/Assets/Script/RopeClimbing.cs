using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class RopeClimbing : MonoBehaviour
{
    public Transform current_segment;
    public Transform above_segment;
    float climbing_speed = 0.3f;

	void Start ()
    {
	
	}
	
	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.Space))
        {
            // Let go of the rope
            this.GetComponent<PlatformerCharacter2D>().enabled = true;
            this.GetComponent<Rigidbody2D>().isKinematic = false;
            this.transform.parent = null;
        }
	}


    void FixedUpdate()
    {
        // If at the next segment, find the next segment
        if (((Vector2) this.transform.position - (Vector2) above_segment.transform.position).magnitude < 0.1f)
        {
            Debug.Log("at same segment");
            this.current_segment = above_segment;
            above_segment = current_segment.GetComponent<HingeJoint2D>().connectedBody.transform;
            this.transform.parent = this.current_segment.transform;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Moving up
        if (v > 0)
        {
            this.transform.position = Vector2.Lerp(this.transform.position, above_segment.transform.position, climbing_speed);
        }

        // Swing rope left and right
        if (h != 0)
        {
            current_segment.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(h * -500, 0));
        }
    }
}
