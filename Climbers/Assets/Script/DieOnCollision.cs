using UnityEngine;
using System.Collections;

public class DieOnCollision : MonoBehaviour {
    public int terrainLayer = 12;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == terrainLayer)
        {
            Destroy(gameObject);
        }
    }
}
