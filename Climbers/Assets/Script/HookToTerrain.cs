using UnityEngine;
using System.Collections;

public class HookToTerrain : MonoBehaviour
{
    public GameObject owner;    // Who threw the hook. The hook will attach the springjoint to this object

    LineRenderer line;
    SpringJoint2D spring;
    bool hooked = false;
    float max_hooking_distance = 15f;
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


    void Update()
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
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
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
        //lineenabled = false;
        //owner.GetComponent<Rigidbody2D>().gravityScale = 1;
    }
}
