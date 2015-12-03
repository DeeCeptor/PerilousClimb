using UnityEngine;
using System.Collections;

// Takes the camera's position, and adjusts the position of this object based on its parallax_movement variable
// Place this on a parent or an object with an image
public class ParallaxMovement : MonoBehaviour
{
    [Range(0, 1)] public float parralax_x_movement;   // Affects how much the background moves with the camera. 0 means no movement. 1 is perfect synchronisation of camera
    [Range(0, 1)] public float parralax_y_movement;
    private float initial_z;


	void Awaje ()
    {
        initial_z = this.transform.position.z;
    }
	

	void Update ()
    {
        this.transform.position = new Vector3(Camera.main.transform.position.x * parralax_x_movement, 
                                            Camera.main.transform.position.y * parralax_y_movement, 
                                            initial_z);
	}
}
