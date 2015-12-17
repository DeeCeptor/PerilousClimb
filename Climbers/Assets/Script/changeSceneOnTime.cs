using UnityEngine;
using System.Collections;

public class changeSceneOnTime : MonoBehaviour {
    public float timeUntilChange = 30f;
    public string scene;
	// Use this for initialization
	void Start () {
        timeUntilChange = timeUntilChange + Time.time;
	}
	
	// Update is called once per frame
	void Update () {
	    if(timeUntilChange < Time.time)
        {
            Application.LoadLevel(scene);
        }
	}
}
