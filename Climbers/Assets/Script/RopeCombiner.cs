using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Placed on the first piece of a rope, if this contacts an end piece of a rope it will attach itself to the other rope
public class RopeCombiner : MonoBehaviour
{
    Link this_link;
    RopeGenerator this_rope;
    Rigidbody2D this_rigidbody;

    void Start()
    {
        this_link = this.GetComponentInParent<Link>();
        this_rope = this_link.rope;
        this_rigidbody = this_link.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Rope" 
            && other.gameObject.GetComponentInParent<Link>().rope != this_rope
            && !this_rigidbody.isKinematic)
        {
            Debug.Log("Connecting ropes");
            other.gameObject.name = "RopePiece";
            this_link.GetComponent<AttachToTerrain>().enabled = false;

            // Connect this rope to the rope that collided with us
            Link other_link = other.GetComponentInParent<Link>();
            other_link.GetComponent<HingeJoint2D>().connectedBody = this_link.GetComponent<Rigidbody2D>();
            other_link.GetComponent<HingeJoint2D>().enabled = true;
            RopeGenerator new_rope = other_link.rope;

            // Go through all the links, setting the new information
           // Link cur_link = other_link.all_segments[0].GetComponent<Link>();
            other_link.all_segments.AddRange(this_link.all_segments);
            List<GameObject> new_all_links = other_link.all_segments;
            Debug.Log(new_all_links.Count);
            
            //while (cur_link != null)
            for (int x = 0; x < new_all_links.Count; x++)
            {
                Link cur_link = new_all_links[x].GetComponent<Link>();
                cur_link.position_from_top_in_rope = x;
                cur_link.position_from_bottom_in_rope = new_all_links.Count - x;
                cur_link.all_segments = new_all_links;
                cur_link.top_most = new_all_links[0];
                cur_link.bottom_most = new_all_links[new_all_links.Count - 1];
                cur_link.transform.parent = new_rope.transform;
                cur_link.rope = new_rope;

                if (x > 0)
                    cur_link.below = new_all_links[x - 1];
                if (x < new_all_links.Count - 1)
                    cur_link.above = new_all_links[x + 1];

                //cur_link = cur_link.below.GetComponent<Link>();
                //pos++;
            }

            new_rope.number_of_segments = new_all_links.Count;

            GameObject.Destroy(this_rope.gameObject);
            GameObject.Destroy(this.gameObject);
            GameObject.Destroy(other.gameObject);
        }
    }
}
