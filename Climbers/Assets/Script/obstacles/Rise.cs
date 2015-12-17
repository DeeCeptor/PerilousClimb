using UnityEngine;
using System.Collections;

public class Rise : MonoBehaviour {
    public float speed = 0.5f;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0,speed);
    }
}
