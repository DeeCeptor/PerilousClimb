using UnityEngine;
using System.Collections;

public class DieOverTime : MonoBehaviour
{
    private float timer;
    public float secondsToLive = 2f;

    void Start()
    {
        timer = Time.time + secondsToLive;
    }


    void FixedUpdate()
    {
        if (timer < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
