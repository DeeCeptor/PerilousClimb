using UnityEngine;
using System.Collections;

public class HookToTerrain : MonoBehaviour
{
    public GameObject owner;    // Who threw the hook. The hook will attach the springjoint to this object

    LineRenderer line;
    bool hooked = false;
    float max_hooking_distance = 10f;

    void Awake()
    {
        line = this.GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.enabled = false;
    }


    void Update()
    {
        line.SetPosition(0, this.gameObject.transform.position);
        line.SetPosition(1, this.owner.transform.position);

        if (hooked && Input.GetKeyDown(KeyCode.Space))
        {
            Detach();
        }

        if (hooked)
        {
            float h = Input.GetAxis("Horizontal");

            owner.GetComponent<Rigidbody2D>().AddForce(new Vector2(h * 100, 0));
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
            joint.distance = Vector2.Distance(this.transform.position, owner.transform.position);
            line.enabled = true;
        }
    }
    // Detaches player from grappling hook
    void Detach()
    {
        hooked = false;
        this.GetComponent<SpringJoint2D>().enabled = false;
        line.enabled = false;
        //owner.GetComponent<Rigidbody2D>().gravityScale = 1;
    }
}
