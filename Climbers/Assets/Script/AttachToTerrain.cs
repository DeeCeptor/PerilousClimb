using UnityEngine;
using System.Collections;

public class AttachToTerrain : MonoBehaviour 
{
    public bool attach_to_thrower_if_in_range = false;
    public float attach_range;
    public GameObject thrower;

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
            Attach();
        }
    }

    // Attached to the terrain. Is now kinematic
    void Attach()
    {
        this.GetComponent<Rigidbody2D>().isKinematic = true;

        if (attach_to_thrower_if_in_range
            && thrower != null
            && Vector2.Distance(thrower.transform.position, this.transform.position) <= attach_range)
            AttachToThrower();

        Instantiate(Resources.Load("AttachmentTerrainParticles"), this.transform.position, Quaternion.identity);

        RopeCombiner rope_attacher = this.GetComponentInChildren<RopeCombiner>();
        if (rope_attacher != null)
            GameObject.Destroy(rope_attacher);
    }
    void AttachToThrower()
    {
        PlatformerCharacter2D player = thrower.GetComponent<PlatformerCharacter2D>();

        player.AttachToRope(this.GetComponent<Link>());
    }
}
