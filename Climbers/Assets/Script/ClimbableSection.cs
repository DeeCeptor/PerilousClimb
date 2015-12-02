using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class ClimbableSection : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlatformerCharacter2D>().can_climb = true;
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlatformerCharacter2D>().can_climb = true;
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlatformerCharacter2D>().can_climb = false;
            other.gameObject.GetComponent<PlatformerCharacter2D>().StopClimbing();
        }
    }
}
