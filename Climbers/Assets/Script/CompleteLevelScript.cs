using UnityEngine;
using System.Collections;

public class CompleteLevelScript : MonoBehaviour {
    public string levelToChangeTo;
    public int playerLayer = 8;
	// Use this for initialization
	void Start () {
	
	}
	
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == playerLayer)
        {
            Application.LoadLevel(levelToChangeTo);
        }
    }
}
