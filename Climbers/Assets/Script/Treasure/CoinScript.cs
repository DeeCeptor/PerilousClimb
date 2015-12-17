using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {
    //this script needs a treasure tracker script in the scene to function properly
    // Use this for initialization
    public int playerLayer = 8;

	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerLayer)
        {
            Player player = collision.gameObject.GetComponent<PlatformerCharacter2D>().player;
            TreasureTracker.scoreTrack.playerScores[player.player_number]++;
            Destroy(gameObject);
        }
    }
}
