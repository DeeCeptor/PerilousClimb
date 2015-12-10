using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookRope : MonoBehaviour
{
    public GameObject owner;    // Who threw the hook. The hook will attach the springjoint to this object
    public List<GameObject> rope_links;

    LineRenderer line;
    SpringJoint2D spring;
    bool hooked = false;
    float max_hooking_distance = 8f;
    float cur_distance;     // How far along the rope we are
    float climbing_speed = 0.03f;
    int stuck_counter = 0;  // Are we stuck on the geometry for a while?

    void Awake()
    {
        //line = this.GetComponent<lineRenderer>();
        //lineSetVertexCount(2);
        //lineenabled = false;
        spring = this.GetComponent<SpringJoint2D>();
    }


    void FixedUpdate()
    {
        //lineSetPosition(0, this.gameObject.transform.position);
        //lineSetPosition(1, this.owner.transform.position);

        if (hooked && Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
        }

        if (hooked)
        {
            float h = Input.GetAxis("Horizontal");
            owner.GetComponent<Rigidbody2D>().AddForce(new Vector2(h * 100, 0));

            float v = Input.GetAxis("Vertical");
            cur_distance = Mathf.Clamp(cur_distance + (-v) * climbing_speed, 0.005f, max_hooking_distance);
            spring.distance = cur_distance;

            float dist = Vector2.Distance(owner.transform.position, this.transform.position);
            if (cur_distance + 0.2f < dist)
            {
                stuck_counter++;

                if (stuck_counter > 10)
                {
                    // This means we're pulling against the terrain and are stuck. Give us an upward push to get over the ledge
                    //owner.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 3000, ForceMode2D.Impulse);
                    owner.GetComponent<PlatformerCharacter2D>().AddJumpVelocity(false);
                    Debug.Log(cur_distance + " : " + dist);
                    stuck_counter = 0;
                }
            }
            else
            {
                stuck_counter = 0;
            }

            // Add a fixed joint to the rope segment we should be using so it follows the player
            // Calculate how far down the rope we are
            int cur_segment = Mathf.Clamp((int) (cur_distance * 5), 1, 39);
            cur_obj = rope_links[cur_segment];

            if (prev_obj != null && prev_obj != cur_obj)
            {
                //prev_obj.GetComponent<TargetJoint2D>().enabled = false;
                prev_obj.GetComponent<Rigidbody2D>().isKinematic = false;
            }
            cur_obj.GetComponent<Rigidbody2D>().isKinematic = true;

            //cur_obj.transform.position = owner.transform.position;
            cur_obj.transform.position = Vector3.Lerp(cur_obj.transform.position, owner.transform.position, 0.5f);
            /*
            TargetJoint2D j = cur_obj.GetComponent<TargetJoint2D>();
            if (j == null)
            {
                j = cur_obj.AddComponent<TargetJoint2D>();
                Debug.Log(cur_segment + " " + cur_obj.transform.position + " " + cur_distance);
            }
            j.autoConfigureTarget = false;
            j.enabled = true;
            j.frequency = 10000;
            j.dampingRatio = 1;
            j.target = owner.transform.position;*/

            prev_obj = cur_obj;
        }
    }
    GameObject cur_obj;
    GameObject prev_obj = null;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain")
            && Vector2.Distance(owner.transform.position, this.transform.position) < max_hooking_distance)
        {
            hooked = true;
            this.GetComponent<Rigidbody2D>().isKinematic = true;

            // Activate the springjoint on this object, and set the distance 
            SpringJoint2D joint = this.GetComponent<SpringJoint2D>();
            joint.enabled = true;
            joint.connectedBody = owner.GetComponent<Rigidbody2D>();
            cur_distance = Vector2.Distance(this.transform.position, owner.transform.position);
            joint.distance = cur_distance;
            //lineenabled = true;
        }
    }
    // Detaches player from grappling hook
    void Detach()
    {
        hooked = false;
        this.GetComponent<SpringJoint2D>().enabled = false;
        owner.GetComponent<PlatformerCharacter2D>().AddJumpVelocity(false);
        prev_obj.GetComponent<Rigidbody2D>().isKinematic = false;
        cur_obj.GetComponent<Rigidbody2D>().isKinematic = false;
        //lineenabled = false;
        //owner.GetComponent<Rigidbody2D>().gravityScale = 1;
    }
}
