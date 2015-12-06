using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class RopeClimbing : MonoBehaviour
{
    public Transform current_segment;
    public Transform above_segment;
    public Transform below_segment;
    float climbing_speed = 0.3f;
    float distance_between_segments = 0;

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
            this.enabled = false;
        }
	}


    void FixedUpdate()
    {
        // If at the next segment, find the next segment
        if (distance_between_segments >= 1)
        {
            below_segment = current_segment;
            current_segment = current_segment.GetComponent<Link>().above.transform;
            above_segment = current_segment.GetComponent<Link>().above.transform;
            distance_between_segments = 0;
        }
        else if (distance_between_segments <= -1)
        {
            above_segment = current_segment;
            current_segment = current_segment.GetComponent<Link>().below.transform;
            below_segment = current_segment.GetComponent<Link>().below.transform;
            distance_between_segments = 0;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        /*float h = 0;
        float v = 0;
        */
        // Keep track of how far between the two segments we are
        distance_between_segments = Mathf.Clamp(distance_between_segments + v / 2f, -1, 1);

        if (distance_between_segments >= 0)
        {
            this.transform.position = Vector2.Lerp(current_segment.transform.position, above_segment.transform.position, distance_between_segments);
        }
        else
        {
            this.transform.position = Vector2.Lerp(current_segment.transform.position, below_segment.transform.position, Mathf.Abs(distance_between_segments));
        }

        // Swing rope left and right
        if (h != 0)
        {
            current_segment.GetComponent<Rigidbody2D>().AddForce(new Vector2(h * -100, 0));
        }
    }
}
