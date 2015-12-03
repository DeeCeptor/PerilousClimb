using UnityEngine;
using System.Collections;

// Sets this game object to active on start
public class SetActive : MonoBehaviour
{
    void Awake()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
}
