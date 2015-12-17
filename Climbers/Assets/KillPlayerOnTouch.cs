using UnityEngine;
using System.Collections;

public class KillPlayerOnTouch : MonoBehaviour {
    public int playerLayer = 8;
    public float damage = 1f;
	

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            PlatformerCharacter2D playerScript = collision.gameObject.GetComponent<PlatformerCharacter2D>();
            playerScript.AdjustHP(-damage);
            Debug.Log("hit");
        }
    }
}
