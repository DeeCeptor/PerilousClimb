using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

// When a player touches a slope, turn off the player's Gravity scale so the player doesn't slide down the slope
// Turn gravity back on after leaving slope
public class Slope : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }


    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            float normal_gravity = other.gameObject.GetComponent<PlatformerCharacter2D>().normal_gravity_factor;
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = normal_gravity;
        }
    }
}
