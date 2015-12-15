using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {
    //determines the speed at witch the player must be falling to take damage(should be a positive value for falling
    public float fallthreshold = 5f;
    public float damage = 1f;
    public int playerLayer = 8;

	// Use this for initialization
	void Start () {
	}

    void Update()
    {
    }
	
	void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerLayer && collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < -fallthreshold && collision.GetType() == typeof(CircleCollider2D))
        {
            PlatformerCharacter2D playerScript = collision.gameObject.GetComponent<PlatformerCharacter2D>();
            playerScript.AdjustHP(-damage);
            Debug.Log("hit");
        }
    }
}
