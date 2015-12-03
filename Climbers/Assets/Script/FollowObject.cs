using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public Transform object_to_follow;
    private float starting_z;

    void Start()
    {
        starting_z = this.transform.position.z;
    }


    void Update ()
    {
        this.transform.position = new Vector3(object_to_follow.position.x, object_to_follow.position.y, starting_z);
    }
}
