using UnityEngine;
using System.Collections;

public class CrumbleScript : MonoBehaviour {
    //This script is for crumbling blocks they will become a bunch of debris which will damage the player if the debris falls on them
    public GameObject debris;
    public int debrisNumber;
    public float velocityThreshold = 5f;
    //layer player is on
    public int playerLayer = 8;
    public int ropeLayer = 9;
    //velocity for the debris that spawns
    public float debrisVelocity = 1f;
    //amount of debris to spawn
    public int debrisToSpawn = 4;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == playerLayer || (collision.collider.gameObject.layer == ropeLayer))
        {
            
            for (int i = 0; i < debrisToSpawn; i++)
            {
                GameObject smallDebris = (GameObject)Instantiate(debris, transform.position, transform.rotation);
                Vector2 newVelocity = new Vector2(Random.Range(-debrisVelocity, debrisVelocity), Random.Range(-debrisVelocity,0 ));
                smallDebris.GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
            Destroy(gameObject);
        }
        
    }
}
