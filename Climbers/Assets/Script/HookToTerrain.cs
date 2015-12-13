using UnityEngine;
using System.Collections;

public class HookToTerrain : MonoBehaviour
{
    public GameObject thrower;    // Who threw the hook. The hook will attach the springjoint to this object
    public float attach_range = 10f;
    LineRenderer line;
    SpringJoint2D spring;


    void Awake()
    {
        line = this.GetComponent<LineRenderer>();
        line.SetVertexCount(2);
        line.enabled = false;
        spring = this.GetComponent<SpringJoint2D>();
    }


    void Update()
    {
        // Render our grappling hook if it's active
        if (spring.enabled)
        {
            line.enabled = true;
            line.SetPosition(0, this.gameObject.transform.position);
            line.SetPosition(1, this.thrower.transform.position);
        }
        else
        {
            line.enabled = false;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            this.GetComponent<Rigidbody2D>().isKinematic = true;

            if (thrower != null
                && Vector2.Distance(thrower.transform.position, this.transform.position) <= attach_range)
            {
                AttachToThrower();
            }
        }
    }


    void AttachToThrower()
    {
        PlatformerCharacter2D player = thrower.GetComponent<PlatformerCharacter2D>();

        player.AttachToGrapple(this.gameObject, attach_range);
    }
}
