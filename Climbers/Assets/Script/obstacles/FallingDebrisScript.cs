using UnityEngine;
using System.Collections;

public class FallingDebrisScript : MonoBehaviour {
    public GameObject smallerDebris;
   

    //multiply velocity by this to get damage
    public float damageMult = 0.01f;
    //the velocity required to do damage
    public float velocityThreshold = 5f;
    //layer player is on
    public int playerLayer = 8;
    public int terrainLayer = 12;
    //velocity for the debris that spawns
    public float debrisVelocity = 1f;
    //amount of debris to spawn
    public int debrisToSpawn = 4;
	void Start () {
	
	}
	

	void Update () {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == playerLayer && collision.relativeVelocity.magnitude > velocityThreshold)
        {
            PlatformerCharacter2D playerScript = collision.gameObject.GetComponent<PlatformerCharacter2D>();
            playerScript.AdjustHP(-collision.relativeVelocity.magnitude * damageMult);
            Debug.Log("hit");
            for (int i = 0; i < debrisToSpawn; i++)
            {
                GameObject smallDebris = (GameObject)Instantiate(smallerDebris, transform.position, transform.rotation);
                Vector2 newVelocity = new Vector2(Random.Range(-debrisVelocity, debrisVelocity), Random.Range(0, debrisVelocity));
                smallDebris.GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
            Destroy(gameObject);
        }

        else if (collision.collider.gameObject.layer == terrainLayer)
        {
            for (int i = 0; i < debrisToSpawn; i++)
            {
                GameObject smallDebris = (GameObject)Instantiate(smallerDebris, transform.position, transform.rotation);
                Vector2 newVelocity = new Vector2(Random.Range(-debrisVelocity, debrisVelocity), Random.Range(0, debrisVelocity));
                smallDebris.GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
            Destroy(gameObject);
        }
    }
}
