using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
    public Transform object_to_follow;

    void Update ()
    {
        this.transform.position = object_to_follow.position;
    }
}
