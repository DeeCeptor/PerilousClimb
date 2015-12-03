using UnityEngine;
using System.Collections;

// When the player enters this trigger region, add a force to the player, like jumping.
public class BouncePlatform : MonoBehaviour
{
    public Vector2 force_added;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<Rigidbody2D>().AddForce(force_added);
            other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(other.gameObject.GetComponent<Rigidbody2D>().velocity.x,
                                                                                15);
        }
    }
}
