using UnityEngine;
using System.Collections;

// https://www.youtube.com/watch?v=ukYbRmmlaTM
[ExecuteInEditMode]
public class GridSnap : MonoBehaviour
{
    public float cell_size = 1f;
    private float x, y, z;

    void Start()
    {
        x = 0f;
        y = 0f;
        z = 0f;
    }


    void Update()
    {
        if (this.transform.parent == null
            || (this.transform.parent.name == "Terrain"))
        {
            x = Mathf.Round(transform.position.x / cell_size) * cell_size;
            y = Mathf.Round(transform.position.y / cell_size) * cell_size;
            z = transform.position.z;
            transform.position = new Vector3(x, y, z);
        }
    }

}